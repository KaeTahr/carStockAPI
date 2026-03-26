<script>
  import { login } from '../lib/api.js';
  import { token, dealerName } from '../lib/stores.js';
  import AuthLayout from '../components/AuthLayout.svelte';

  export let onNavigate;
  export let successMessage = '';

  let username = '';
  let password = '';
  let error = '';
  let loading = false;

  async function handleLogin() {
    error = '';
    loading = true;
    try {
      const res = await login(username, password);
      let data = null;
      const text = await res.text();
      if (text) data = JSON.parse(text);
      if (!res.ok) {
        error = 'Invalid username or password';
        return;
      }
      token.set(data.token);
      dealerName.set(data.name);
    } catch (e) {
      error = e.message;
    } finally {
      loading = false;
    }
  }
</script>

<AuthLayout>
  <h1 class="font-display font-bold text-3xl text-text tracking-tight mb-1">Welcome back</h1>
  <p class="text-muted text-sm mb-7">Sign in to manage your inventory</p>

  {#if successMessage}
    <div class="bg-emerald-500/10 border border-emerald-500/30 text-emerald-300 px-4 py-3 rounded-lg text-sm mb-4">
      {successMessage}
    </div>
  {/if}

  {#if error}
    <div class="bg-red-500/10 border border-red-500/30 text-red-300 px-4 py-3 rounded-lg text-sm mb-4">
      {error}
    </div>
  {/if}

  <form onsubmit={(e) => { e.preventDefault(); handleLogin(); }}>
    <div class="mb-4">
      <label for="username" class="block text-xs font-medium text-muted uppercase tracking-widest mb-1.5">
        Username
      </label>
      <input
        id="username"
        type="text"
        bind:value={username}
        placeholder="Enter your username"
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
        placeholder="Enter your password"
        class="w-full bg-surface2 border border-border rounded-lg px-3.5 py-2.5 text-text font-body text-sm outline-none focus:border-accent transition-colors placeholder:text-dim"
      />
    </div>

    <button
      type="submit"
      disabled={loading}
      class="w-full bg-accent hover:bg-accent-hover text-bg font-semibold py-2.5 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-sm"
    >
      {loading ? 'Signing in...' : 'Sign In'}
    </button>
  </form>

  <p class="text-center text-xs text-muted mt-5">
    Don't have an account?
    <button onclick={() => onNavigate('register')} class="text-accent underline underline-offset-2 cursor-pointer bg-transparent border-none text-xs">
      Register here
    </button>
  </p>
</AuthLayout>