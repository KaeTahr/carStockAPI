<script>
  let { children } = $props();
</script>

<div class="min-h-screen bg-bg flex items-center justify-center relative overflow-hidden">
  <!-- purple glow top left -->
  <div class="absolute w-96 h-96 rounded-full -top-24 -left-24 pointer-events-none"
     style="background: radial-gradient(circle, rgba(139,92,246,0.08) 0%, transparent 70%)">
  </div>
  <!-- amber glow bottom right -->
  <div class="absolute w-80 h-80 rounded-full -bottom-12 -right-12 pointer-events-none"
     style="background: radial-gradient(circle, rgba(245,158,11,0.06) 0%, transparent 70%)">
  </div>

  <div class="relative z-10 bg-surface border border-border rounded-2xl p-10 w-full max-w-md shadow-2xl">
    <div class="flex items-center gap-2 mb-8">
      <span class="text-accent text-2xl">⬡</span>
      <span class="font-display font-extrabold text-xl text-text tracking-tight">CarStock</span>
    </div>
    {@render children()}
  </div>
</div>