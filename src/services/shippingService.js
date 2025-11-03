import { api } from './api';

const calculateShipping = async (cep) => {
  try {
    const cleanCep = cep.replace(/\D/g, '');
    if (cleanCep.length !== 8) {
      return { 
        success: false, 
        error: 'CEP inválido. Deve conter 8 dígitos.' 
      };
    }

    const response = await api.post('/shipping/calculate', { cep: cleanCep });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao calcular frete' 
    };
  }
};

export { calculateShipping };

