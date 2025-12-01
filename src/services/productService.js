import { api } from './api';

const getAllProducts = async (params = {}) => {
  try {
    const response = await api.get('/Product', { params });
    return { success: true, data: response.data };
  } catch (error) {
    console.error('Erro ao buscar produtos:', error);
    console.error('URL chamada:', error.config?.url);
    console.error('Resposta do servidor:', error.response?.data);
    return { 
      success: false, 
      error: error.response?.data?.message || error.response?.data?.Message || 'Erro ao buscar produtos' 
    };
  }
};

const getProductById = async (id) => {
  try {
    const response = await api.get(`/Product/${id}`);
    return { success: true, data: response.data };
  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || error.response?.data?.Message || 'Erro ao buscar produto' 
    };
  }
};

const searchProducts = async (searchTerm, category = null) => {
  try {
    const params = {};
    if (searchTerm) {
      params.search = searchTerm;
    }
    if (category && category !== 'Todos') {
      params.category = category;
    }
    
    // Tenta usar o endpoint de listagem com parâmetros de busca
    // Se não funcionar, retorna erro para que o frontend faça busca local
    const response = await api.get('/Product', { params });
    return { success: true, data: response.data };
  } catch (error) {
    // Se der erro, retorna para que o frontend faça busca local
    console.warn('Busca remota falhou, será feita busca local:', error);
    return { 
      success: false, 
      error: error.response?.data?.message || error.response?.data?.Message || 'Erro ao buscar produtos' 
    };
  }
};

// Função auxiliar para fazer upload de uma imagem
// Tenta diferentes endpoints possíveis do backend C#
const uploadImage = async (imageFile) => {
  const formData = new FormData();
  formData.append('file', imageFile);
  
  // Lista de endpoints possíveis para upload
  const possibleEndpoints = [
    '/Upload',                    // PascalCase comum em C#
    '/upload',                    // minúsculo
    '/api/Upload',                 // com prefixo api
    '/api/upload',
    '/vendedor/Upload',            // endpoint específico do vendedor
    '/vendedor/upload',
    '/Images/Upload',             // plural
    '/images/upload',
    '/File/Upload',               // File controller
    '/file/upload'
  ];
  
  let lastError = null;
  
  for (const endpoint of possibleEndpoints) {
    try {
      console.log(`Tentando fazer upload no endpoint: ${endpoint}`);
      const response = await api.post(endpoint, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      
      console.log(`Upload bem-sucedido no endpoint: ${endpoint}`, response.data);
      
      // Retorna a URL da imagem enviada
      // Tenta diferentes formatos de resposta
      const imageUrl = response.data.url || 
                      response.data.imageUrl || 
                      response.data.path || 
                      response.data.fileUrl ||
                      response.data ||
                      (typeof response.data === 'string' ? response.data : null);
      
      if (imageUrl) {
        return imageUrl;
      }
      
      // Se não encontrou URL, tenta novamente com próximo endpoint
      throw new Error('URL da imagem não encontrada na resposta');
    } catch (error) {
      lastError = error;
      // Se não for 404, pode ser outro erro válido (401, 500, etc) - não continua tentando
      if (error.response?.status !== 404) {
        console.warn(`Endpoint ${endpoint} retornou erro ${error.response?.status}:`, error.response?.data);
        throw error;
      }
    }
  }
  
  // Se nenhum endpoint funcionou, lança o último erro
  console.error('Nenhum endpoint de upload funcionou. Último erro:', lastError);
  throw new Error(`Nenhum endpoint de upload encontrado. Verifique se o backend C# tem um endpoint para upload de imagens. Último erro: ${lastError?.response?.status || lastError?.message}`);
};

const createProduct = async (productData) => {
  try {
    // 1. Obter dados do vendedor da store ou buscar diretamente
    const { store } = await import('@/store/index.js');
    const { getCurrentVendor } = await import('@/services/authService.js');
    
    // Verifica se o vendedor está autenticado
    if (!store.isVendorAuthenticated) {
      return {
        success: false,
        error: 'Vendedor não autenticado. Faça login como vendedor primeiro.'
      };
    }
    
    // Verifica se o token existe no localStorage
    const { getVendorToken } = await import('@/services/api.js');
    const tokenCheck = getVendorToken();
    console.log('Verificação de token na criação do produto:', tokenCheck ? 'Token presente' : 'Token ausente');
    
    if (!tokenCheck) {
      return {
        success: false,
        error: 'Token do vendedor não encontrado. Por favor, faça login novamente.'
      };
    }
    
    // Tenta obter os dados do vendedor da store ou busca diretamente
    let vendor = store.vendor;
    
    // Se não tiver dados na store, busca do token JWT usando getCurrentVendor
    if (!vendor || !vendor.id) {
      console.log('Vendedor não encontrado na store ou sem ID, buscando do token JWT...');
      const vendorResult = await getCurrentVendor();
      if (vendorResult.success && vendorResult.data) {
        vendor = vendorResult.data;
        store.vendor = vendor; // Atualiza a store
        console.log('Dados do vendedor obtidos do token:', vendor);
      } else {
        console.error('Erro ao buscar dados do vendedor do token:', vendorResult.error);
        console.error('Detalhes completos:', vendorResult);
        return {
          success: false,
          error: `Não foi possível obter os dados do vendedor do token: ${vendorResult.error || 'Erro desconhecido'}. Tente fazer login novamente.`
        };
      }
    } else {
      console.log('Dados do vendedor da store:', vendor);
    }
    
    // Extrai ID e nome do vendedor
    const vendedorId = vendor.id || vendor.vendedorId || vendor.Id;
    const vendedorNome = vendor.nome || vendor.name || vendor.Nome || vendor.email || 'Vendedor';
    
    console.log('vendedorId encontrado:', vendedorId);
    console.log('vendedorNome encontrado:', vendedorNome);
    
    if (!vendedorId) {
      console.error('Estrutura completa do objeto vendor:', JSON.stringify(vendor, null, 2));
      return {
        success: false,
        error: 'ID do vendedor não encontrado no token JWT. O backend C# precisa incluir o ID do vendedor nas claims do token JWT. Verifique o console para mais detalhes.'
      };
    }

    // 2. Verifica se precisa fazer upload separado ou enviar tudo junto
    // Por padrão, tenta enviar tudo junto usando FormData (como o código original)
    // Se o backend precisar de upload separado, descomente a seção de upload acima
    
    const formData = new FormData();
    const allFiles = []; // Lista plana de todos os arquivos de imagem
    
    // 2.1. Monta o objeto JSON 'Data' que o C# pode esperar
    const jsonData = {
      nome: productData.name,
      descricao: productData.description || '',
      categoria: productData.category,
      vendedorId: vendedorId,
      vendedorNome: vendedorNome,
      variations: []
    };

    // 2.2. Processa cada variação e adiciona os arquivos
    for (const variation of productData.variations) {
      const fileIndexes = []; // Índices dos arquivos para ESTA variação

      // Adiciona os arquivos da variação à lista 'allFiles'
      for (const imageFile of variation.images) {
        allFiles.push(imageFile);
        fileIndexes.push(allFiles.length - 1); // Guarda o índice
      }

      // Adiciona os dados da variação ao JSON
      jsonData.variations.push({
        preco: parseFloat(variation.preco),
        estoque: parseInt(variation.estoque),
        cor: variation.cor || null,
        tamanho: variation.tamanho || null,
        fileIndexes: fileIndexes, // Índices dos arquivos para esta variação
      });
    }

    // 2.3. Adiciona o JSON 'Data' (como string) ao FormData
    formData.append('Data', JSON.stringify(jsonData));

    // 2.4. Adiciona todos os arquivos ao FormData
    allFiles.forEach((file) => {
      formData.append('Files', file);
    });

    // 3. Envia a requisição POST com FormData
    // O endpoint é '/vendedor/produtos' conforme informado
    const response = await api.post('/vendedor/produtos', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return { success: true, data: response.data };

  } catch (error) {
    return { 
      success: false, 
      error: error.response?.data?.message || error.message || 'Erro ao criar produto' 
    };
  }
};
const updateProduct = async (id, productData) => {
  const attempts = [
    { method: 'patch', url: `/vendedor/produtos/${id}` },
    { method: 'put', url: `/vendedor/produtos/${id}` },
    { method: 'put', url: `/Product/${id}` },
  ];

  let lastError = null;
  for (const { method, url } of attempts) {
    try {
      const response = await api[method](url, productData);
      return { success: true, data: response.data };
    } catch (error) {
      lastError = error;
      // Continua tentando próximos endpoints
    }
  }

  return {
    success: false,
    error: lastError?.response?.data?.message || lastError?.message || 'Erro ao atualizar produto'
  };
};

const deleteProduct = async (id) => {
  try {
    const response = await api.delete(`/vendedor/produtos/${id}`);
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

const getVendorQuestions = async () => {
  try {
    const response = await api.get('/vendedor/perguntas');
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao buscar perguntas do vendedor'
    };
  }
};

const answerVendorQuestion = async (questionId, answer) => {
  try {
    const response = await api.post(`/vendedor/perguntas/${questionId}/responder`, { answer });
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao responder pergunta'
    };
  }
};

const getFeaturedProducts = async () => {
  try {
    const response = await api.get('/products/featured');
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao buscar produtos em destaque'
    };
  }
};

const getVendorProducts = async () => {
  try {
    const response = await api.get('/vendedor/produtos');
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao buscar produtos do vendedor'
    };
  }
};

const toggleProductStatus = async (id, status) => {
  try {
    const response = await api.patch(`/vendedor/produtos/${id}/status`, { active: status });
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || 'Erro ao alterar status do produto'
    };
  }
};

const updateVariationStock = async (variationId, estoque, productId) => {
  try {
    const response = await api.patch(`/vendedor/produtos/${productId}/variacoes/${variationId}/estoque`, { estoque });
    return { success: true, data: response.data };
  } catch (error) {
    return {
      success: false,
      error: error.response?.data?.message || error.message || 'Erro ao atualizar estoque da variação'
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
  getVendorQuestions,
  answerVendorQuestion,
  getFeaturedProducts,
  getVendorProducts,
  toggleProductStatus,
  updateVariationStock,
};
