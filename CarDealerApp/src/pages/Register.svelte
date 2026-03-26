<script>
  import { register } from '../lib/api.js';
  import AuthLayout from '../components/AuthLayout.svelte';

  let { onNavigate } = $props();
  
  let name = $state('');
  let username = $state('');
  let password = $state('');
  let error = $state('');
  let success = $state('');
  let loading = $state(false);

  async function handleRegister() {
    error = '';
    success = '';
    loading = true;
    try {
      const res = await register(name, username, password);
      let data = null;
      const text = await res.text();
      if (text) data = JSON.parse(text);
      if (!res.ok) {
        if (data?.errors) {
          error = Object.values(data.errors).flat().join(', ');
        } else {
          error = data?.message || 'Registration failed';
        }
        return;
      }
      onNavigate('login', 'Account created! You can now sign in.');
    } catch (e) {
      error = e.message;
    } finally {
      loading = false;
    }
  }
</script>

<AuthLayout>
  <h1 class="font-display font-bold text-3xl text-text tracking-tight mb-1">Create account</h1>
  <p class="text-muted text-sm mb-7">Set up your dealer inventory</p>

  {#if error}
    <div class="bg-red-500/10 border border-red-500/30 text-red-300 px-4 py-3 rounded-lg text-sm mb-4">
      {error}
    </div>
  {/if}

  <form onsubmit={(e) => { e.preventDefault(); handleRegister(); }}>
    <div class="mb-4">
        <label for="name" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">
        Dealer Name
        </label>
        <input
        id="name"
        type="text"
        bind:value={name}
        placeholder="e.g. Melbourne Auto Group"
        class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text font-body text-sm outline-none focus:border-accent transition-colors placeholder:text-dim"
        />
    </div>

    <div class="mb-4">
        <label for="username" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">
        Username
        </label>
        <input
        id="username"
        type="text"
        bind:value={username}
        placeholder="Choose a username"
        class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text font-body text-sm outline-none focus:border-accent transition-colors placeholder:text-dim"
        />
    </div>

    <div class="mb-6">
        <label for="password" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">
        Password
        </label>
        <input
        id="password"
        type="password"
        bind:value={password}
        placeholder="At least 6 characters"
        class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text font-body text-sm outline-none focus:border-accent transition-colors placeholder:text-dim"
        />
    </div>

    <button
        type="submit"
        disabled={loading}
        class="w-full bg-accent hover:bg-accent-hover text-bg font-semibold py-2.5 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm"
    >
        {loading ? 'Creating account...' : 'Create Account'}
    </button>
  </form>

  <p class="text-center text-xs text-muted mt-5">
    Already have an account?
    <button onclick={() => onNavigate('login')} class="text-accent underline underline-offset-2 cursor-pointer bg-transparent border-none text-xs">
      Sign in here
    </button>
  </p>
</AuthLayout>