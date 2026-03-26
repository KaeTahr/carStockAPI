<script>
  import {isAuthenticated} from './lib/stores.js';
  import Login from './pages/Login.svelte';
  import Register from './pages/Register.svelte';
  import Dashboard from './pages/Dashboard.svelte';

  let currentPage = $state('login');
</script>

{#if $isAuthenticated}
  <Dashboard />
{:else if currentPage === 'register'}
  <Register onNavigate={(page) => currentPage = page} />
{:else}
  <Login onNavigate={(page) => currentPage = page} />
{/if}
