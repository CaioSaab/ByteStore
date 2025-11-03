import { api } from './api';

const getCart = async () => {
  try {
    const response = await api.get('/cart');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar carrinho' 
    };
  }
};

const addToCart = async (productId, details) => {
  try {
    const response = await api.post('/cart', {
      productId,
      color: details.color,
      size: details.size,
      quantity: details.quantity || 1,
    });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao adicionar ao carrinho' 
    };
  }
};

const updateCartItem = async (cartItemId, quantity) => {
  try {
    const response = await api.put(`/cart/${cartItemId}`, { quantity });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao atualizar item do carrinho' 
    };
  }
};

const removeFromCart = async (cartItemId) => {
  try {
    const response = await api.delete(`/cart/${cartItemId}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao remover do carrinho' 
    };
  }
};

const clearCart = async () => {
  try {
    const response = await api.delete('/cart');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao limpar carrinho' 
    };
  }
};

export {
  getCart,
  addToCart,
  updateCartItem,
  removeFromCart,
  clearCart,
};

