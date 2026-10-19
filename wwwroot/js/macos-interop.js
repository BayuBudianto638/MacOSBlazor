// Minimal drag interop: Blazor owns state, JS only translates pointer movement.
window.macosDrag = {
  start(id, dotNetRef, startX, startY, origLeft, origTop) {
    const move = (e) => {
      dotNetRef.invokeMethodAsync('OnDragMove',
        Math.round(origLeft + (e.clientX - startX)),
        Math.round(origTop + (e.clientY - startY)));
    };
    const up = () => {
      document.removeEventListener('pointermove', move);
      document.removeEventListener('pointerup', up);
    };
    document.addEventListener('pointermove', move);
    document.addEventListener('pointerup', up);
  }
};
