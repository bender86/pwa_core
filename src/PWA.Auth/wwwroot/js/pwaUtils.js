// Pull to refresh
let startY = 0;
let pulling = false;
let indicator = null;
const THRESHOLD = 80;

export function initPullToRefresh(dotnetHelper) {
    indicator = document.createElement('div');
    indicator.className = 'pull-indicator';
    indicator.innerHTML = '↓ Tirer pour rafraîchir';
    document.body.prepend(indicator);

    document.addEventListener('touchstart', (e) => {
        if (window.scrollY === 0) {
            startY = e.touches[0].clientY;
            pulling = true;
        }
    }, { passive: true });

    document.addEventListener('touchmove', (e) => {
        if (!pulling) return;
        const distance = e.touches[0].clientY - startY;
        if (distance > 0 && distance < THRESHOLD * 1.5) {
            indicator.style.height = `${Math.min(distance, THRESHOLD)}px`;
            indicator.style.opacity = `${Math.min(distance / THRESHOLD, 1)}`;
            indicator.innerHTML = distance >= THRESHOLD 
                ? '↑ Relâcher pour rafraîchir' 
                : '↓ Tirer pour rafraîchir';
        }
    }, { passive: true });

    document.addEventListener('touchend', async (e) => {
        if (!pulling) return;
        const distance = e.changedTouches[0].clientY - startY;
        indicator.style.height = '0';
        indicator.style.opacity = '0';
        indicator.innerHTML = '↓ Tirer pour rafraîchir';
        pulling = false;
        if (distance >= THRESHOLD) {
            await dotnetHelper.invokeMethodAsync('OnPullToRefresh');
        }
    }, { passive: true });
}

export function disposePullToRefresh() {
    indicator?.remove();
}