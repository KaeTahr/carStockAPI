<script>
  import { onMount } from 'svelte';
  import { logout, dealerName } from '../lib/stores.js';
  import { getCars, addCar, deleteCar, updateStock, searchCars } from '../lib/api.js';
  import Modal from '../components/Modal.svelte';

  let { onLogout } = $props();

  let cars = $state([]);
  let loading = $state(true);
  let error = $state('');

  // search
  let searchMake = $state('');
  let searchModel = $state('');
  let searching = $state(false);

  // modals
  let showAddModal = $state(false);
  let showStockModal = $state(false);
  let selectedCar = $state(null);

  // add car form
  let newCar = $state({ make: '', model: '', year: new Date().getFullYear(), price: '', stock: '' });
  let addError = $state('');
  let addLoading = $state(false);

  // edit stock form
  let newStock = $state('');
  let stockError = $state('');
  let stockLoading = $state(false);

  // delete
  let deletingId = $state(null);

  onMount(loadCars);

  async function loadCars() {
    loading = true;
    error = '';
    try {
      const res = await getCars();
      const data = await res.json();
      if (!res.ok) { error = 'Failed to load cars'; return; }
      cars = data;
    } catch (e) {
      error = 'Could not connect to server';
    } finally {
      loading = false;
    }
  }

  async function handleSearch() {
    if (!searchMake && !searchModel) { return loadCars(); }
    searching = true;
    try {
      const res = await searchCars(searchMake, searchModel);
      const data = await res.json();
      if (!res.ok) { error = 'Search failed'; return; }
      cars = data;
    } catch (e) {
      error = 'Could not connect to server';
    } finally {
      searching = false;
    }
  }

  async function handleAddCar() {
    addError = '';
    addLoading = true;
    try {
      const res = await addCar({
        make: newCar.make,
        model: newCar.model,
        year: newCar.year.toString(),
        price: parseFloat(newCar.price),
        stock: parseInt(newCar.stock)
      });
      const text = await res.text();
      const data = text ? JSON.parse(text) : null;
      if (!res.ok) {
        addError = data?.errors ? Object.values(data.errors).flat().join(', ') : 'Failed to add car';
        return;
      }
      showAddModal = false;
      newCar = { make: '', model: '', year: new Date().getFullYear(), price: '', stock: '' };
      await loadCars();
    } catch (e) {
      addError = e.message;
    } finally {
      addLoading = false;
    }
  }

  async function handleDelete(id, make, model) {
    if (!confirm(`Delete ${make} ${model}? This action cannot be undone.`)) { return; }
    deletingId = id;
    try {
      const res = await deleteCar(id);
      if (!res.ok) { error = 'Failed to delete car'; return; }
      cars = cars.filter(c => c.id !== id);
    } catch (e) {
      error = 'Could not connect to server';
    } finally {
      deletingId = null;
    }
  }

  function openStockModal(car) {
    selectedCar = car;
    newStock = car.stock.toString();
    stockError = '';
    showStockModal = true;
  }

  async function handleUpdateStock() {
    stockError = '';
    stockLoading = true;
    try {
      const res = await updateStock(selectedCar.id, parseInt(newStock));
      const text = await res.text();
      const data = text ? JSON.parse(text) : null;
      if (!res.ok) {
        stockError = data?.errors ? Object.values(data.errors).flat().join(', ') : 'Failed to update stock';
        return;
      }
      showStockModal = false;
      await loadCars();
    } catch (e) {
      stockError = e.message;
    } finally {
      stockLoading = false;
    }
  }

  function handleLogout() {
    logout();
    onLogout();
  }
</script>

<!-- Header -->
<header class="bg-surface border-b border-border px-6 py-4 flex items-center justify-between">
  <div class="flex items-center gap-2">
    <span class="text-accent text-xl">⬡</span>
    <span class="font-display font-extrabold text-lg text-text tracking-tight">CarStock</span>
  </div>
  <div class="flex items-center gap-4">
    <span class="text-sm text-muted">Hello, <span class="text-text font-medium">{$dealerName}</span>!</span>
    <button
      onclick={handleLogout}
      class="text-muted hover:text-text text-sm transition-colors bg-transparent border-none cursor-pointer"
    >
      Sign out
    </button>
  </div>
</header>

<!-- Main -->
<main class="max-w-5xl mx-auto px-6 py-8">

  <!-- Title row -->
  <div class="flex items-center justify-between mb-6">
    <h1 class="font-display font-bold text-2xl text-text tracking-tight">My Inventory</h1>
    <button
      onclick={() => showAddModal = true}
      class="bg-accent hover:bg-accent-hover text-bg font-semibold text-sm px-4 py-2 rounded-lg transition-colors"
    >
      + Add Car
    </button>
  </div>

  <!-- Search -->
  <form onsubmit={(e) => { e.preventDefault(); handleSearch(); }} class="flex gap-3 mb-6">
    <input
      type="text"
      bind:value={searchMake}
      placeholder="Search by make..."
      class="bg-surface2 border border-border rounded-lg px-3.5 py-2 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim flex-1"
    />
    <input
      type="text"
      bind:value={searchModel}
      placeholder="Search by model..."
      class="bg-surface2 border border-border rounded-lg px-3.5 py-2 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim flex-1"
    />
    <button
      type="submit"
      disabled={searching}
      class="bg-surface2 border border-border hover:border-accent text-text text-sm px-4 py-2 rounded-lg transition-colors disabled:opacity-50"
    >
      {searching ? '...' : 'Search'}
    </button>
    <button
      type="button"
      onclick={() => { searchMake = ''; searchModel = ''; loadCars(); }}
      class="text-muted hover:text-text text-sm transition-colors bg-transparent border-none cursor-pointer"
    >
      Clear
    </button>
</form>

  <!-- Error -->
  {#if error}
    <div class="bg-red-500/10 border border-red-500/30 text-red-300 px-4 py-3 rounded-lg text-sm mb-4">
      {error}
    </div>
  {/if}

  <!-- Table -->
  {#if loading}
    <div class="text-muted text-sm text-center py-16">Loading inventory...</div>
  {:else if cars.length === 0}
    <div class="text-center py-16">
      <p class="text-muted text-sm mb-4">No cars in your inventory yet.</p>
      <button
        onclick={() => showAddModal = true}
        class="text-accent text-sm underline underline-offset-2 bg-transparent border-none cursor-pointer"
      >
        Add your first car
      </button>
    </div>
  {:else}
    <div class="bg-surface border border-border rounded-xl overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-border">
            <th class="text-left text-xs font-medium text-muted uppercase tracking-widest px-5 py-3">Make</th>
            <th class="text-left text-xs font-medium text-muted uppercase tracking-widest px-5 py-3">Model</th>
            <th class="text-left text-xs font-medium text-muted uppercase tracking-widest px-5 py-3">Year</th>
            <th class="text-left text-xs font-medium text-muted uppercase tracking-widest px-5 py-3">Price</th>
            <th class="text-left text-xs font-medium text-muted uppercase tracking-widest px-5 py-3">Stock</th>
            <th class="px-5 py-3"></th>
          </tr>
        </thead>
        <tbody>
          {#each cars as car (car.id)}
            <tr class="border-b border-border/50 hover:bg-surface2 transition-colors">
              <td class="px-5 py-3.5 text-text text-sm font-medium">{car.make}</td>
              <td class="px-5 py-3.5 text-text text-sm">{car.model}</td>
              <td class="px-5 py-3.5 text-muted text-sm">{car.year}</td>
              <td class="px-5 py-3.5 text-muted text-sm">${car.price.toLocaleString()}</td>
              <td class="px-5 py-3.5">
                <span class="bg-accent/10 text-accent text-xs font-semibold px-2.5 py-1 rounded-full">
                  {car.stock} in stock
                </span>
              </td>
              <td class="px-5 py-3.5 text-right">
                <button
                  onclick={() => openStockModal(car)}
                  class="text-muted hover:text-accent text-xs transition-colors bg-transparent border-none cursor-pointer mr-3"
                >
                  Edit Stock
                </button>
                <button
                  onclick={() => handleDelete(car.id, car.make, car.model)}
                  disabled={deletingId === car.id}
                  class="text-muted hover:text-red-400 text-xs transition-colors bg-transparent border-none cursor-pointer disabled:opacity-50"
                >
                  {deletingId === car.id ? '...' : 'Delete'}
                </button>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    </div>
  {/if}
</main>

<!-- Add Car Modal -->
{#if showAddModal}
  <Modal title="Add Car" onClose={() => showAddModal = false}>
    <form onsubmit={(e) => { e.preventDefault(); handleAddCar(); }}>
        {#if addError}
        <div class="bg-red-500/10 border border-red-500/30 text-red-300 px-4 py-3 rounded-lg text-sm mb-4">
            {addError}
        </div>
        {/if}

        <div class="mb-4">
        <label for="make" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">Make</label>
        <input id="make" type="text" bind:value={newCar.make} placeholder="e.g. Toyota"
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim" />
        </div>

        <div class="mb-4">
        <label for="model" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">Model</label>
        <input id="model" type="text" bind:value={newCar.model} placeholder="e.g. Corolla"
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim" />
        </div>

        <div class="grid grid-cols-3 gap-3 mb-6">
        <div>
            <label for="year" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">Year</label>
            <input id="year" type="number" bind:value={newCar.year}
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors" />
        </div>
        <div>
            <label for="price" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">Price</label>
            <input id="price" type="number" bind:value={newCar.price} placeholder="0"
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim" />
        </div>
        <div>
            <label for="stock" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">Stock</label>
            <input id="stock" type="number" bind:value={newCar.stock} placeholder="0"
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors placeholder:text-dim" />
        </div>
        </div>

        <button
        type="submit"
        disabled={addLoading}
        class="w-full bg-accent hover:bg-accent-hover text-bg font-semibold py-2.5 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm"
        >
        {addLoading ? 'Adding...' : 'Add Car'}
        </button>
    </form>
  </Modal>
{/if}

<!-- Edit Stock Modal -->
{#if showStockModal && selectedCar}
  <Modal title="Update Stock" onClose={() => showStockModal = false}>
    <form onsubmit={(e) => { e.preventDefault(); handleUpdateStock(); }}>
        <p class="text-muted text-sm mb-4">
        Updating stock for <span class="text-text font-medium">{selectedCar.make} {selectedCar.model} {selectedCar.year}</span>
        </p>

        {#if stockError}
        <div class="bg-red-500/10 border border-red-500/30 text-red-300 px-4 py-3 rounded-lg text-sm mb-4">
            {stockError}
        </div>
        {/if}

        <div class="mb-6">
        <label for="newstock" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">New Stock Level</label>
        <input id="newstock" type="number" bind:value={newStock}
            class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text text-sm outline-none focus:border-accent transition-colors" />
        </div>

        <button
        type="submit"
        disabled={stockLoading}
        class="w-full bg-accent hover:bg-accent-hover text-bg font-semibold py-2.5 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm"
        >
        {stockLoading ? 'Updating...' : 'Update Stock'}
        </button>
    </form>
  </Modal>
{/if}