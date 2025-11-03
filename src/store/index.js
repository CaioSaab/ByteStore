import { reactive } from 'vue';
import { login as apiLogin, register as apiRegister, logout as apiLogout, vendorLogin as apiVendorLogin, vendorRegister as apiVendorRegister, getCurrentUser, getCurrentVendor } from '@/services/authService';
import { getAllProducts, searchProducts, createProduct } from '@/services/productService';
import { getCart, addToCart as apiAddToCart, removeFromCart as apiRemoveFromCart, clearCart as apiClearCart, updateCartItem } from '@/services/cartService';
import { validateCoupon } from '@/services/couponService';
import { getAuthToken, getVendorToken } from '@/services/api';

const initializeAuth = () => {
  const token = getAuthToken();
  const vendorToken = getVendorToken();
  return {
    hasUserToken: !!token,
    hasVendorToken: !!vendorToken,
  };
};

const loadUserData = async () => {
  const result = await getCurrentUser();
  if (result.success) {
    return result.data;
  }
  return null;
};

const loadVendorData = async () => {
  const result = await getCurrentVendor();
  if (result.success) {
    return result.data;
  }
  return null;
};

const loadProducts = async () => {
  const result = await getAllProducts();
  if (result.success) {
    return result.data.products || result.data || [];
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
      
      if (auth.hasUserToken) {
        this.isAuthenticated = true;
        const userData = await loadUserData();
        this.user = userData;
      }
      
      if (auth.hasVendorToken) {
        this.isVendorAuthenticated = true;
        const vendorData = await loadVendorData();
        this.vendor = vendorData;
      }

      const products = await loadProducts();
      this.products = products;

      if (this.isAuthenticated) {
        const cartItems = await loadCart();
        this.cart = cartItems;
      }
    } catch (error) {
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
        this.products = result.data.products || result.data || [];
      } else {
        this.error = result.error;
      }
    } catch (error) {
      this.error = error.message || 'Erro ao buscar produtos';
    } finally {
      this.isLoading = false;
    }
  },

  async loadProducts() {
    this.isLoading = true;
    try {
      const result = await getAllProducts({ category: this.selectedCategory !== 'Todos' ? this.selectedCategory : null });
      if (result.success) {
        this.products = result.data.products || result.data || [];
      } else {
        this.error = result.error;
      }
    } catch (error) {
      this.error = error.message || 'Erro ao carregar produtos';
    } finally {
      this.isLoading = false;
    }
  },

  async addToCart(product, details) {
    if (!this.isAuthenticated) {
      this.error = 'Você precisa estar logado para adicionar itens ao carrinho';
      return;
    }

    try {
      const result = await apiAddToCart(product.id, {
        color: details.color,
        size: details.size,
        quantity: details.quantity || 1,
      });

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
        this.discountPercentage = result.data.discountPercentage || 0;
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
        this.isAuthenticated = true;
        this.user = result.data.user || { email };
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
        this.isAuthenticated = true;
        this.user = result.data.user || { email: userData.email };
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
  },

  async vendorLogin(email, password) {
    this.isLoading = true;
    this.error = null;
    try {
      const result = await apiVendorLogin(email, password);
      if (result.success) {
        this.isVendorAuthenticated = true;
        this.vendor = result.data.vendor || { email };
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
        this.isVendorAuthenticated = true;
        this.vendor = result.data.vendor || { email: vendorData.email };
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
