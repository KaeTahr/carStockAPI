<script>
  let { title, onClose, children } = $props();
</script>

<!-- backdrop -->
<div
  class="fixed inset-0 bg-black/60 backdrop-blur-sm z-40 flex items-center justify-center"
  onclick={onClose}
  onkeydown={(e) => e.key === 'Escape' && onClose()}
  tabindex="0"
  role="dialog"
  aria-modal="true"
>
  <!-- modal card — stop click propagating to backdrop -->
  <div
    class="relative bg-surface border border-border rounded-2xl p-8 w-full max-w-md shadow-2xl z-50"
    onclick={(e) => e.stopPropagation()}
    role="presentation"
  >
    <div class="flex items-center justify-between mb-6">
      <h2 class="font-display font-bold text-xl text-text tracking-tight">{title}</h2>
      <button
        onclick={onClose}
        class="text-muted hover:text-text transition-colors text-xl leading-none bg-transparent border-none cursor-pointer"
      >
        ✕
      </button>
    </div>
    {@render children()}
  </div>
</div>