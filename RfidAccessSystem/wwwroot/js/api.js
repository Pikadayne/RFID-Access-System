// API Helper
const API_BASE_URL = 'http://localhost:5000'; // Adjust based on your API port
const API_KEY = 'RFID_ABC123XYZ789'; // Match the API key in database

async function apiCall(method, endpoint, data = null) {
    const headers = {
        'X-API-Key': API_KEY,
        'Content-Type': 'application/json'
    };

    const options = {
        method: method,
        headers: headers
    };

    if (data && (method === 'POST' || method === 'PUT')) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(API_BASE_URL + endpoint, options);

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        return result;
    } catch (error) {
        console.error(`API Error (${method} ${endpoint}):`, error);
        throw error;
    }
}
