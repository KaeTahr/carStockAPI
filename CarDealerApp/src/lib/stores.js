import {writable, derived} from 'svelte/store';

export const token = writable(localStorage.getItem('token') || null);

token.subscribe(value => {
    if (value) {
        localStorage.setItem('token', value);
    } else {
        localStorage.removeItem('token');
    }
});

export const isAuthenticated = derived(token, $token => !!$token);  

export const dealerName = writable(localStorage.getItem('dealerName') || null);

dealerName.subscribe(value => {
    if (value) {
        localStorage.setItem('dealerName', value);
    } else {
        localStorage.removeItem('dealerName');
    }
});

export const logout = () => {
    token.set(null);
    dealerName.set(null);
};