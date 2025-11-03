import { api } from './api';

const validateCoupon = async (couponCode) => {
  try {
    const response = await api.post('/coupons/validate', { code: couponCode });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Cupom inválido ou expirado' 
    };
  }
};

const getAvailableCoupons = async () => {
  try {
    const response = await api.get('/coupons/available');
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar cupons disponíveis' 
    };
  }
};

export { validateCoupon, getAvailableCoupons };

