<script setup>
import { computed, ref } from 'vue';
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';

const couponInput = ref('');
const shippingCost = 5.00;

const selectedPaymentMethod = ref('credit');

// Total (Sem desconto)
const subtotal = computed(() => {
  return store.cart.reduce((acc, item) => acc + (item.price * item.quantity), 0);
});

// Calcula desconto
const discountAmount = computed(() => {
  return subtotal.value * store.discountPercentage;
});

// Total com desconto
const grandTotal = computed(() => {
  const totalBeforeDiscount = subtotal.value + shippingCost;
  return totalBeforeDiscount - discountAmount.value;
});

// Aplicar
function handleApplyCoupon() {
  store.applyCoupon(couponInput.value);
}
</script>

<template>
  <div class="container mx-auto p-4 text-white">
    <h1 class="text-3xl font-bold mb-6">Checkout</h1>
    
    <div v-if="store.cart.length > 0" class="flex flex-col lg:flex-row gap-8">
      
      <div class="lg:w-2/3 bg-[#1a1a2e] p-6 rounded-lg">
        <h2 class="text-2xl font-bold mb-4">Informações de entrega</h2>

        <h2 class="text-2xl font-bold mt-8 mb-4">Método de pagamento</h2>
        <div class="space-y-4">

          <div 
            class="border rounded-lg p-4 cursor-pointer transition-colors"
            :class="selectedPaymentMethod === 'credit' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            @click="selectedPaymentMethod = 'credit'"
          >
            <h3 class="font-semibold"><i class="fas fa-credit-card mr-2"></i> Cartão de crédito</h3>
          </div>

          <div 
            class="border rounded-lg p-4 cursor-pointer transition-colors"
            :class="selectedPaymentMethod === 'debit' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            @click="selectedPaymentMethod = 'debit'"
          >
            <h3 class="font-semibold"><i class="fas fa-money-check-alt mr-2"></i> Cartão de débito</h3>
          </div>

          <div 
            class="border rounded-lg p-4 cursor-pointer transition-colors"
            :class="selectedPaymentMethod === 'pix' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            @click="selectedPaymentMethod = 'pix'"
           >
            <h3 class="font-semibold"><i class="fas fa-qrcode mr-2"></i> PIX</h3>
          </div>
        </div>
      </div>
      
      <div class="lg:w-1/3">
        <div class="bg-[#1a1a2e] rounded-lg shadow-lg p-6 sticky top-24">
          <h2 class="text-2xl font-bold mb-4">Seu pedido</h2>
          <div class="space-y-3 border-b border-gray-700 pb-4 mb-4">
            <div v-for="item in store.cart" :key="item.cartItemId" class="flex justify-between text-sm">
              <span class="truncate pr-2">{{ item.name }} <span class="text-gray-400">x{{ item.quantity }}</span></span>
              <span>${{ (item.price * item.quantity).toFixed(2) }}</span>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between">
              <span>Subtotal</span>
              <span>R${{ subtotal.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-gray-400">
              <span>Frete</span>
              <span>R${{ shippingCost.toFixed(2) }}</span>
            </div>
            <div v-if="store.appliedCoupon" class="flex justify-between text-green-400">
              <span>Desconto ({{ store.appliedCoupon }})</span>
              <span>-R${{ discountAmount.toFixed(2) }}</span>
            </div>
          </div>
          <div class="border-t border-gray-700 pt-4 mt-4 font-bold text-xl flex justify-between">
            <span>Total</span>
            <span>R${{ grandTotal.toFixed(2) }}</span>
          </div>
          <div class="mt-6">
            <label for="coupon" class="block text-sm font-medium mb-1">Cupom de desconto</label>
            <div class="flex">
              <input v-model="couponInput" type="text" id="coupon" class="w-full bg-[#0e101f] border border-gray-600 rounded-l-md p-2 focus:outline-none focus:border-blue-500" placeholder="10%Off">
              <button @click="handleApplyCoupon" class="bg-blue-600 text-white px-4 rounded-r-md hover:bg-blue-700">Aplicar</button>
            </div>
            <p v-if="store.couponMessage" class="text-sm mt-2" :class="store.appliedCoupon ? 'text-green-400' : 'text-red-400'">
              {{ store.couponMessage }}
            </p>
          </div>
          <button class="w-full mt-4 bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300">
            Fazer pedido
          </button>
        </div>
      </div>
    </div>
    
    <div v-else class="text-center bg-[#1a1a2e] p-10 rounded-lg">
        <h2 class="text-2xl">Seu carrinho está vazio.</h2>
        <RouterLink to="/" class="mt-4 inline-block bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
            Continuar comprando
        </RouterLink>
    </div>
  </div>
</template>