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
const getVendorCoupons = async () => {
  try {
    const response = await api.get('/vendedor/cupons');
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao buscar cupons do vendedor'
    };
  }
};

const createOrUpdateCoupon = async (coupon) => {
  try {
    const response = await api.post('/vendedor/cupons', coupon);
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao salvar cupom'
    };
  }
};

const toggleCouponActive = async (code, isActive) => {
  try {
    const response = await api.patch(`/vendedor/cupons/${encodeURIComponent(code)}/status`, { active: isActive });
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao alterar status do cupom'
    };
  }
};

const deleteCoupon = async (code) => {
  try {
    const response = await api.delete(`/vendedor/cupons/${encodeURIComponent(code)}`);
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao excluir cupom'
    };
  }
};

export { getVendorCoupons, createOrUpdateCoupon, toggleCouponActive, deleteCoupon };

