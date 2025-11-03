import { api, setAuthToken, setVendorToken } from './api';

const login = async (email, password) => {
  try {
    const response = await api.post('/auth/login', { email, password });
    const token = response.data.token || '';
    setAuthToken(token);
    return { success: true, data: response.data, token };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao fazer login' 
    };
  }
};

const register = async (userData) => {
  try {
    const response = await api.post('/auth/register', userData);
    const token = response.data.token || '';
    setAuthToken(token);
    return { success: true, data: response.data, token };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao registrar' 
    };
  }
};

const logout = () => {
  setAuthToken('');
  setVendorToken('');
};

const vendorLogin = async (email, password) => {
  try {
    const response = await api.post('/auth/vendor/login', { email, password });
    const token = response.data.token || '';
    setVendorToken(token);
    return { success: true, data: response.data, token };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao fazer login como vendedor' 
    };
  }
};

const vendorRegister = async (vendorData) => {
  try {
    const response = await api.post('/auth/vendor/register', vendorData);
    const token = response.data.token || '';
    setVendorToken(token);
    return { success: true, data: response.data, token };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao registrar vendedor' 
    };
  }
};

const forgotPassword = async (email) => {
  try {
    const response = await api.post('/auth/forgot-password', { email });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao solicitar recuperação de senha' 
    };
  }
};

const getCurrentUser = async () => {
  try {
    const response = await api.get('/auth/me');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao obter dados do usuário' 
    };
  }
};

const getCurrentVendor = async () => {
  try {
    const response = await api.get('/auth/vendor/me');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao obter dados do vendedor' 
    };
  }
};

export {
  login,
  register,
  logout,
  vendorLogin,
  vendorRegister,
  forgotPassword,
  getCurrentUser,
  getCurrentVendor,
};

