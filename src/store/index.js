import { reactive } from 'vue';

const validCoupons = {
  "10%Off": 0.10,
  "15%Off": 0.15,
  "20%Off": 0.20,
};

export const store = reactive({
  
  searchTerm: '',
  selectedCategory: 'Todos',
  cart: [],

  appliedCoupon: null,
  discountPercentage: 0,
  couponMessage: '',

  setCategory(category) {
    this.selectedCategory = category;
    this.searchTerm = '';
  },

  addToCart(product, details) {
    const existingItem = this.cart.find(item => 
        item.id === product.id && 
        item.color === details.color && 
        item.size === details.size
    );

    if (existingItem) {
      existingItem.quantity++;
    } else {
      this.cart.push({
        id: product.id,
        cartItemId: `${product.id}-${details.color}-${details.size}`,
        name: product.name,
        price: product.price,
        image: product.image,
        color: details.color,
        size: details.size,
        quantity: 1,
      });
    }
  },

  // Remove um item do carrinho
  removeFromCart(cartItemId) {
    const index = this.cart.findIndex(item => item.cartItemId === cartItemId);
    if (index !== -1) {
      this.cart.splice(index, 1);
    }
  },

  applyCoupon(couponCode) {
    if (validCoupons[couponCode]) {
      this.appliedCoupon = couponCode;
      this.discountPercentage = validCoupons[couponCode];
      this.couponMessage = `Cupom "${couponCode}" aplicado com sucesso!`;
    } else {
      this.appliedCoupon = null;
      this.discountPercentage = 0;
      this.couponMessage = 'Cupom inválido ou expirado.';
    }
  },
});
