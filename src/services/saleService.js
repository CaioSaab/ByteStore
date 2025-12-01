import { api } from './api'; // Sua instância do Axios

export const getMySales = async () => {
  try {
    const response = await api.get('/vendedor/vendas'); 
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar vendas' 
    };
  }
};

export const updateOrderStatus = async (orderId, status) => {
  try {
    const response = await api.patch(`/vendedor/vendas/${orderId}/status`, { status });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao atualizar status do pedido' 
    };
  }
};

export const getMyPurchases = async () => {
  try {
    const response = await api.get('/Sale/minhas');
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao buscar minhas compras'
    };
  }
};