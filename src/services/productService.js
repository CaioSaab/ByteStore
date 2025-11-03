import { api } from './api';

const getAllProducts = async (params = {}) => {
  try {
    const response = await api.get('/products', { params });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar produtos' 
    };
  }
};

const getProductById = async (id) => {
  try {
    const response = await api.get(`/products/${id}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar produto' 
    };
  }
};

const searchProducts = async (searchTerm, category = null) => {
  try {
    const params = { search: searchTerm };
    if (category && category !== 'Todos') {
      params.category = category;
    }
    const response = await api.get('/products/search', { params });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar produtos' 
    };
  }
};

const createProduct = async (productData) => {
  try {
    const formData = new FormData();
    
    Object.keys(productData).forEach((key) => {
      if (key === 'images' && Array.isArray(productData[key])) {
        productData[key].forEach((image) => {
          formData.append('images', image);
        });
      } else if (key !== 'images') {
        if (Array.isArray(productData[key])) {
          formData.append(key, JSON.stringify(productData[key]));
        } else {
          formData.append(key, productData[key]);
        }
      }
    });

    const response = await api.post('/products', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao criar produto' 
    };
  }
};

const updateProduct = async (id, productData) => {
  try {
    const response = await api.put(`/products/${id}`, productData);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao atualizar produto' 
    };
  }
};

const deleteProduct = async (id) => {
  try {
    const response = await api.delete(`/products/${id}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao deletar produto' 
    };
  }
};

const addQuestion = async (productId, question) => {
  try {
    const response = await api.post(`/products/${productId}/questions`, { question });
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao adicionar pergunta' 
    };
  }
};

const getProductQuestions = async (productId) => {
  try {
    const response = await api.get(`/products/${productId}/questions`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || 'Erro ao buscar perguntas' 
    };
  }
};

export {
  getAllProducts,
  getProductById,
  searchProducts,
  createProduct,
  updateProduct,
  deleteProduct,
  addQuestion,
  getProductQuestions,
};

