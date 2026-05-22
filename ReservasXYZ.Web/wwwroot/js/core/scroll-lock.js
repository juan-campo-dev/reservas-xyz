export function setBodyScrollLock(isLocked) {
    document.documentElement.style.overflow = isLocked ? 'hidden' : '';
    document.body.style.overflow = isLocked ? 'hidden' : '';
}