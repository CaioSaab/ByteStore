import { api } from './api';

const createOrder = async (orderData) => {
  try {
    const itens = (orderData.items || []).map(i => ({
      productVariationId: i.variationId,
      quantidade: i.quantity,
    }));

    const methodMap = {
      credit: 'Crédito',
      debit: 'Débito',
      pix: 'Pix'
    };

    const payload = {
      itens,
      shippingCost: Number(orderData.shippingCost || 0),
      couponCode: orderData.coupon || null,
      paymentMethod: methodMap[orderData.paymentMethod] || 'Crédito',
      installments: orderData.installments || null,
    };
    const response = await api.post('/sale', payload);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao finalizar compra' 
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

