import { ref, computed } from 'vue'

// O 'ref' cria uma variável reativa. Qualquer mudança nela,
// atualizará todos os componentes que a estiverem usando.
export const cartItems = ref([])

// 'computed' cria um valor reativo que depende de outros.
// Ele recalcula automaticamente quando 'cartItems' muda.
export const cartItemCount = computed(() => {
  return cartItems.value.reduce((acc, item) => acc + item.quantity, 0)
})

export const cartTotal = computed(() => {
  return cartItems.value.reduce((acc, item) => acc + (item.price * item.quantity), 0)
})

// Funções para manipular o estado do carrinho
export function addToCart(product) {
  const existingItem = cartItems.value.find(item => item.id === product.id)

  if (existingItem) {
    existingItem.quantity++
  } else {
    cartItems.value.push({ ...product, quantity: 1 })
  }
}

export function removeFromCart(productId) {
  cartItems.value = cartItems.value.filter(item => item.id !== productId)
}

export function updateQuantity(productId, quantity) {
    const item = cartItems.value.find(item => item.id === productId)
    if (item) {
      item.quantity = quantity
      if (item.quantity <= 0) {
        removeFromCart(productId)
      }
    }
  }