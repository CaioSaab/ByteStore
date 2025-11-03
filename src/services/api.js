import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:3000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

const getAuthToken = () => {
  return localStorage.getItem('authToken') || '';
};

const setAuthToken = (token) => {
  if (token) {
    localStorage.setItem('authToken', token);
  } else {
    localStorage.removeItem('authToken');
  }
};

const getVendorToken = () => {
  return localStorage.getItem('vendorToken') || '';
};

const setVendorToken = (token) => {
  if (token) {
    localStorage.setItem('vendorToken', token);
  } else {
    localStorage.removeItem('vendorToken');
  }
};

api.interceptors.request.use(
  (config) => {
    const token = getAuthToken();
    const vendorToken = getVendorToken();
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else if (vendorToken) {
      config.headers.Authorization = `Bearer ${vendorToken}`;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      setAuthToken('');
      setVendorToken('');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export { api, getAuthToken, setAuthToken, getVendorToken, setVendorToken };

