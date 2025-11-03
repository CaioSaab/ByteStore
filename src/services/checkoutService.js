import { api } from './api';

const createOrder = async (orderData) => {
  try {
    const response = await api.post('/orders', orderData);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao criar pedido' 
    };
  }
};

const getOrderById = async (orderId) => {
  try {
    const response = await api.get(`/orders/${orderId}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar pedido' 
    };
  }
};

const getUserOrders = async () => {
  try {
    const response = await api.get('/orders');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar pedidos' 
    };
  }
};

const getVendorOrders = async () => {
  try {
    const response = await api.get('/vendor/orders');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar pedidos do vendedor' 
    };
  }
};

export {
  createOrder,
  getOrderById,
  getUserOrders,
  getVendorOrders,
};

