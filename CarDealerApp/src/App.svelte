<script>
  import {isAuthenticated} from './lib/stores.js';
  import Login from './pages/Login.svelte';
  import Register from './pages/Register.svelte';
  import Dashboard from './pages/Dashboard.svelte';

  let currentPage = $state('login');
  let successMessage = $state('');
</script>

{#if $isAuthenticated}
  <Dashboard onLogout={() => { successMessage = ''; currentPage = 'login'; }} />
{:else if currentPage === 'register'}
  <Register onNavigate={(page, msg = '') => { currentPage = page; successMessage = msg; }} />
{:else}
  <Login {successMessage}
  onNavigate={(page) => {successMessage = ''; currentPage = page}} />
{/if}
