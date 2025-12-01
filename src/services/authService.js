import { api, setAuthToken, setVendorToken } from './api';

const login = async (email, password) => {
  try {
  const response = await api.post('/Auth/LoginComprador', { email, password });
  const token = typeof response.data === 'string' ? response.data : (response.data.token || '');
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
    const payload = {
      Nome: userData.name,
      Email: userData.email,
      Password: userData.password,
    };
    const response = await api.post('/Auth/RegistrarComprador', payload);
    return { success: true, data: response.data };

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
  const response = await api.post('/Auth/LoginVendedor', { email, password });
  const token = typeof response.data === 'string' ? response.data : (response.data.token || '');
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
    const payload = {
      Nome: vendorData.storeName,
      Email: vendorData.email,
      Password: vendorData.password,
    };
    const response = await api.post('/Auth/RegistrarVendedor', payload);
    return { success: true, data: response.data };
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
  const response = await api.get('/Auth/me');
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
  const response = await api.get('/Auth/vendor/me');
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

