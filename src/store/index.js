import { reactive } from 'vue';
import { login as apiLogin, register as apiRegister, logout as apiLogout, vendorLogin as apiVendorLogin, vendorRegister as apiVendorRegister, getCurrentUser, getCurrentVendor } from '@/services/authService';
import { getAllProducts, searchProducts, createProduct } from '@/services/productService';
import { getCart, addToCart as apiAddToCart, removeFromCart as apiRemoveFromCart, clearCart as apiClearCart, updateCartItem } from '@/services/cartService';
import { validateCoupon } from '@/services/couponService';
import { getAuthToken, getVendorToken, setAuthToken, setVendorToken } from '@/services/api';

const initializeAuth = () => {
  const token = getAuthToken();
  const vendorToken = getVendorToken();
  return {
    hasUserToken: !!token,
    hasVendorToken: !!vendorToken,
  };
};

const loadUserData = async () => {
  try {
    const result = await getCurrentUser();
    if (result.success) {
      return result.data;
    }
    return null;
  } catch (error) {
    console.error('Erro ao carregar dados do usuário:', error);
    return null;
  }
};

const loadVendorData = async () => {
  try {
    const result = await getCurrentVendor();
    if (result.success) {
      return result.data;
    }
    return null;
  } catch (error) {
    console.error('Erro ao carregar dados do vendedor:', error);
    return null;
  }
};

// Função para transformar produto do backend para o formato do frontend
const transformProductFromBackend = (product) => {
  // Pega a primeira variação (ou cria uma padrão se não houver)
  const firstVariation = product.variations && product.variations.length > 0 
    ? product.variations[0] 
    : null;
  
  // Pega a primeira imagem da primeira variação (ou imagem padrão)
  const firstImage = firstVariation?.imageUrls && firstVariation.imageUrls.length > 0
    ? firstVariation.imageUrls[0]
    : 'https://via.placeholder.com/300x300?text=Sem+Imagem';
  
  // Pega o preço da primeira variação (ou 0 se não houver)
  const price = firstVariation?.preco ? Number(firstVariation.preco) : 0;
  
  return {
    id: product.id,
    name: product.nome,
    description: product.descricao || '',
    category: product.categoria,
    price: price,
    image: firstImage,
    // Mantém os dados originais para uso futuro
    vendorId: product.vendedorId,
    vendorName: product.vendedorNome,
    variations: product.variations || []
  };
};

const loadProducts = async () => {
  const result = await getAllProducts();
  if (result.success) {
    // O backend retorna PageResponse com 'items' (não 'products')
    const items = result.data.items || result.data.products || result.data || [];
    // Transformar os dados do backend para o formato esperado pelo frontend
    return items.map(transformProductFromBackend);
  }
  return [];
};

const loadCart = async () => {
  const result = await getCart();
  if (result.success) {
    return result.data.items || result.data || [];
  }
  return [];
};

export const store = reactive({
  isAuthenticated: false,
  user: null,
  isVendorAuthenticated: false,
  vendor: null,
  products: [],
  searchTerm: '',
  selectedCategory: 'Todos',
  cart: [],
  appliedCoupon: null,
  discountPercentage: 0,
  couponMessage: '',
  isLoading: false,
  error: null,

  async initialize() {
    this.isLoading = true;
    try {
      const auth = initializeAuth();
      
      // Tentar restaurar autenticação do usuário cliente
      if (auth.hasUserToken) {
        const userData = await loadUserData();
        if (userData) {
          this.isAuthenticated = true;
          this.user = userData;
          if (userData.email) {
            localStorage.setItem('currentUserEmail', userData.email);
          }
          // Carregar carrinho apenas se o usuário foi autenticado com sucesso
          const cartItems = await loadCart();
          this.cart = cartItems;
        } else {
          // Se falhou ao carregar dados, o token pode estar inválido
          // Limpar token e manter deslogado
          setAuthToken('');
          localStorage.removeItem('currentUserEmail');
          this.isAuthenticated = false;
          this.user = null;
        }
      }
      
      // Tentar restaurar autenticação do vendedor
      if (auth.hasVendorToken) {
        const vendorData = await loadVendorData();
        if (vendorData) {
          this.isVendorAuthenticated = true;
          this.vendor = vendorData;
        } else {
          // Se falhou ao carregar dados, o token pode estar inválido
          // Limpar token e manter deslogado
          setVendorToken('');
          this.isVendorAuthenticated = false;
          this.vendor = null;
        }
      }

      // Carregar produtos independente de autenticação
      const products = await loadProducts();
      this.products = products;
    } catch (error) {
      console.error('Erro ao inicializar aplicação:', error);
      this.error = error.message || 'Erro ao inicializar aplicação';
    } finally {
      this.isLoading = false;
    }
  },

  setCategory(category) {
    this.selectedCategory = category;
    this.searchTerm = '';
  },

  async searchProducts() {
    if (!this.searchTerm.trim() && this.selectedCategory === 'Todos') {
      await this.loadProducts();
      return;
    }

    this.isLoading = true;
    try {
      const result = await searchProducts(this.searchTerm, this.selectedCategory);
      if (result.success) {
        // O backend pode retornar PageResponse com 'items' ou array direto
        const items = result.data.items || result.data.products || result.data || [];
        // Transformar os dados
        this.products = Array.isArray(items) ? items.map(transformProductFromBackend) : [];
      } else {
        this.error = result.error;
        // Se a busca remota falhar, tenta fazer busca local nos produtos já carregados
        // A busca local já é feita no computed fullyFilteredProducts no HomeView
      }
    } catch (error) {
      // Se der erro, carrega todos e deixa o filtro local fazer o trabalho
      console.warn('Erro na busca remota, usando busca local:', error);
      await this.loadProducts();
    } finally {
      this.isLoading = false;
    }
  },

  async loadProducts() {
    this.isLoading = true;
    try {
      // O backend aceita pageNumber e pageSize, mas não category como parâmetro
      // Por enquanto, vamos buscar todos e filtrar localmente se necessário
      const params = {
        pageNumber: 1,
        pageSize: 100 // Buscar muitos produtos de uma vez
      };
      
      const result = await getAllProducts(params);
      if (result.success) {
        console.log('Produtos recebidos do backend:', result.data);
        // O backend retorna PageResponse com 'items' (não 'products')
        const items = result.data.items || result.data.products || result.data || [];
        console.log('Items extraídos:', items);
        // Transformar os dados do backend para o formato esperado pelo frontend
        this.products = Array.isArray(items) ? items.map(transformProductFromBackend) : [];
        console.log('Produtos transformados:', this.products);
      } else {
        console.error('Erro ao carregar produtos:', result.error);
        this.error = result.error;
      }
    } catch (error) {
      console.error('Exceção ao carregar produtos:', error);
      this.error = error.message || 'Erro ao carregar produtos';
    } finally {
      this.isLoading = false;
    }
  },

  async addToCart(arg1, details) {
    if (!this.isAuthenticated) {
      this.error = 'Você precisa estar logado para adicionar itens ao carrinho';
      return;
    }

    try {
      // Suporta nova forma (objeto com productId/variationId) e antiga (product + details)
      let result;
      if (typeof arg1 === 'object' && arg1 && arg1.productId) {
        result = await apiAddToCart(arg1);
      } else {
        result = await apiAddToCart(arg1?.id || arg1, details);
      }

      if (result.success) {
        await this.refreshCart();
      } else {
        this.error = result.error;
      }
    } catch (error) {
      this.error = error.message || 'Erro ao adicionar ao carrinho';
    }
  },

  async removeFromCart(cartItemId) {
    if (!this.isAuthenticated) {
      return;
    }

    try {
      const result = await apiRemoveFromCart(cartItemId);
      if (result.success) {
        await this.refreshCart();
      } else {
        this.error = result.error;
      }
    } catch (error) {
      this.error = error.message || 'Erro ao remover do carrinho';
    }
  },

  async updateCartItemQuantity(cartItemId, quantity) {
    if (!this.isAuthenticated) {
      return;
    }

    try {
      const result = await updateCartItem(cartItemId, quantity);
      if (result.success) {
        await this.refreshCart();
      } else {
        this.error = result.error;
      }
    } catch (error) {
      this.error = error.message || 'Erro ao atualizar quantidade';
    }
  },

  async refreshCart() {
    if (!this.isAuthenticated) {
      return;
    }

    const cartItems = await loadCart();
    this.cart = cartItems;
  },

  async applyCoupon(couponCode) {
    try {
      const result = await validateCoupon(couponCode);
      if (result.success) {
        this.appliedCoupon = couponCode;
        this.discountPercentage = ((result.data.discountPercentage || 0) / 100);
        this.couponMessage = result.data.message || `Cupom "${couponCode}" aplicado com sucesso!`;
      } else {
        this.appliedCoupon = null;
        this.discountPercentage = 0;
        this.couponMessage = result.error || 'Cupom inválido ou expirado.';
      }
    } catch (error) {
      this.appliedCoupon = null;
      this.discountPercentage = 0;
      this.couponMessage = error.message || 'Erro ao validar cupom';
    }
  },

  async login(email, password) {
    this.isLoading = true;
    this.error = null;
    try {
      const result = await apiLogin(email, password);
      if (result.success) {
        // Token já foi salvo pelo authService
        // Carregar dados completos do usuário
        const userData = await loadUserData();
        if (userData) {
          this.isAuthenticated = true;
          this.user = userData;
          if (userData.email) {
            localStorage.setItem('currentUserEmail', userData.email);
          }
        } else {
          // Se não conseguiu carregar dados, usar os dados retornados do login
          this.isAuthenticated = true;
          this.user = result.data.user || { email };
          localStorage.setItem('currentUserEmail', (result.data?.user?.email) || email);
        }
        await this.refreshCart();
        return { success: true };
      } else {
        this.error = result.error;
        return { success: false, error: result.error };
      }
    } catch (error) {
      this.error = error.message || 'Erro ao fazer login';
      return { success: false, error: this.error };
    } finally {
      this.isLoading = false;
    }
  },

  async register(userData) {
    this.isLoading = true;
    this.error = null;
    try {
      const result = await apiRegister(userData);
      if (result.success) {
        // Token já foi salvo pelo authService
        // Carregar dados completos do usuário
        const userDataFromApi = await loadUserData();
        if (userDataFromApi) {
          this.isAuthenticated = true;
          this.user = userDataFromApi;
        } else {
          // Se não conseguiu carregar dados, usar os dados retornados do registro
          this.isAuthenticated = true;
          this.user = result.data.user || { email: userData.email };
        }
        await this.refreshCart();
        return { success: true };
      } else {
        this.error = result.error;
        return { success: false, error: result.error };
      }
    } catch (error) {
      this.error = error.message || 'Erro ao registrar';
      return { success: false, error: this.error };
    } finally {
      this.isLoading = false;
    }
  },

  async logout() {
    apiLogout();
    this.isAuthenticated = false;
    this.user = null;
    this.cart = [];
    this.appliedCoupon = null;
    this.discountPercentage = 0;
    this.couponMessage = '';
    localStorage.removeItem('currentUserEmail');
  },

    async vendorLogin(email, password) {
    this.isLoading = true;
    this.error = null;
    try {
      const result = await apiVendorLogin(email, password);
      if (result.success) {
        // Token já foi salvo pelo authService
        // Carregar dados completos do vendedor
        const vendorData = await loadVendorData();
        if (vendorData) {
          this.isVendorAuthenticated = true;
          this.vendor = vendorData;
          if (vendorData.email) {
            localStorage.setItem('currentUserEmail', vendorData.email);
          }
        } else {
          // Se não conseguiu carregar dados, usar os dados retornados do login
          this.isVendorAuthenticated = true;
          this.vendor = result.data.vendor || { email };
          localStorage.setItem('currentUserEmail', (result.data?.vendor?.email) || email);
        }
        return { success: true };
      } else {
        this.error = result.error;
        return { success: false, error: result.error };
      }
    } catch (error) {
      this.error = error.message || 'Erro ao fazer login como vendedor';        
      return { success: false, error: this.error };
    } finally {
      this.isLoading = false;
    }
  },

    async vendorRegister(vendorData) {
    this.isLoading = true;
    this.error = null;
    try {
      const result = await apiVendorRegister(vendorData);
      if (result.success) {
        // Token já foi salvo pelo authService
        // Carregar dados completos do vendedor
        const vendorDataFromApi = await loadVendorData();
        if (vendorDataFromApi) {
          this.isVendorAuthenticated = true;
          this.vendor = vendorDataFromApi;
        } else {
          // Se não conseguiu carregar dados, usar os dados retornados do registro
          this.isVendorAuthenticated = true;
          this.vendor = result.data.vendor || { email: vendorData.email };
        }
        return { success: true };
      } else {
        this.error = result.error;
        return { success: false, error: result.error };
      }
    } catch (error) {
      this.error = error.message || 'Erro ao registrar vendedor';
      return { success: false, error: this.error };
    } finally {
      this.isLoading = false;
    }
  },

  vendorLogout() {
    apiLogout();
    this.isVendorAuthenticated = false;
    this.vendor = null;
    localStorage.removeItem('currentUserEmail');
  },

  async addProduct(newProductData) {
    if (!this.isVendorAuthenticated) {
      this.error = 'Você precisa estar logado como vendedor para adicionar produtos';
      return { success: false, error: this.error };
    }

    this.isLoading = true;
    this.error = null;
    try {
      const result = await createProduct(newProductData);
      if (result.success) {
        await this.loadProducts();
        return { success: true, data: result.data };
      } else {
        this.error = result.error;
        return { success: false, error: result.error };
      }
    } catch (error) {
      this.error = error.message || 'Erro ao adicionar produto';
      return { success: false, error: this.error };
    } finally {
      this.isLoading = false;
    }
  },

  async clearCartAndCoupon() {
    if (!this.isAuthenticated) {
      return;
    }

    try {
      await apiClearCart();
      this.cart = [];
      this.appliedCoupon = null;
      this.discountPercentage = 0;
      this.couponMessage = '';
    } catch (error) {
      this.error = error.message || 'Erro ao limpar carrinho';
    }
  },
});
