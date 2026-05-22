export const authRoutePrefixes = [
    '/identity/account/login',
    '/identity/account/register',
    '/identity/account/forgotpassword',
    '/identity/account/resendemailconfirmation',
    '/identity/account/registerconfirmation',
    '/identity/account/forgotpasswordconfirmation',
    '/identity/account/resetpassword',
    '/identity/account/resetpasswordconfirmation'
];

export function isAuthModalUrl(value) {
    if (!value) {
        return false;
    }

    const url = new URL(value, window.location.origin);

    if (url.origin !== window.location.origin) {
        return false;
    }

    const normalizedPath = url.pathname.toLowerCase();
    return authRoutePrefixes.some((prefix) => normalizedPath.startsWith(prefix));
}