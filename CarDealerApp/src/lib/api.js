import { get } from 'svelte/store'
import { token } from './stores';

const BASE_URL = 'http://localhost:5150';

const authHeaders = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${get(token)}`
});

const authHeadersNoBody = () => ({
    'Authorization': `Bearer ${get(token)}`
});

// Auth
export const register = (name, username, password) =>
    fetch(`${BASE_URL}/auth/register`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({ name, username, password })
    });

export const login = (username, password) =>
    fetch(`${BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({ username, password })
    });

// Cars
export const getCars = () =>
    fetch(`${BASE_URL}/cars`, {
        headers: authHeaders()
    });

export const getCar = (id) =>
    fetch(`${BASE_URL}/cars/${id}`, {
        headers: authHeaders()
    });

export const addCar = (car) =>
    fetch(`${BASE_URL}/cars`, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify(car)
    });

export const updateStock = (id, stock) =>
    fetch(`${BASE_URL}/cars/${id}/stock`, {
        method: 'PATCH',
        headers: authHeaders(),
        body: JSON.stringify({ stock })
    });

export const deleteCar = (id) =>
    fetch(`${BASE_URL}/cars/${id}`, {
        method: 'DELETE',
        headers: authHeaders()
    });

export const searchCars = (make, model) => {
    const params = new URLSearchParams();
    if (make) params.append('make', make);
    if (model) params.append('model', model);
    return fetch(`${BASE_URL}/cars/search?${params}`, {
        headers: authHeadersNoBody()
    });
};
