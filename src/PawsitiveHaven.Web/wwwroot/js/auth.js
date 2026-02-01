// Auth helper functions for localStorage persistence
window.authHelpers = {
    saveToken: function (token) {
        localStorage.setItem('authToken', token);
    },

    getToken: function () {
        return localStorage.getItem('authToken');
    },

    saveAuthState: function (state) {
        localStorage.setItem('authState', JSON.stringify(state));
    },

    getAuthState: function () {
        const state = localStorage.getItem('authState');
        return state ? JSON.parse(state) : null;
    },

    clearAuth: function () {
        localStorage.removeItem('authToken');
        localStorage.removeItem('authState');
    }
};
