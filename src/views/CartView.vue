<script setup>
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';

const shippingCost = 5.00;

// Propriedades computadas para calcular os totais
const subtotal = computed(() => {
  return store.cart.reduce((acc, item) => acc + (item.price * item.quantity), 0);
});

const total = computed(() => {
  return subtotal.value + shippingCost;
});

// Métodos para aumentar/diminuir quantidade
const increaseQuantity = (item) => {
  item.quantity++;
};

const decreaseQuantity = (item) => {
  if (item.quantity > 1) {
    item.quantity--;
  } else {
    // Se a quantidade for 1, remove o item ao clicar em "-"
    store.removeFromCart(item.cartItemId);
  }
};
</script>

<template>
  <div class="container mx-auto p-4 text-white">
    <h1 class="text-3xl font-bold mb-6">Seu Carrinho</h1>
    <div v-if="store.cart.length > 0" class="flex flex-col lg:flex-row gap-8">
      <div class="lg:w-2/3">
        <div class="bg-[#1a1a2e] rounded-lg shadow-lg p-4 sm:p-6 space-y-4">
          <div v-for="item in store.cart" :key="item.cartItemId" class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b border-gray-700 pb-4">
            <div class="flex items-center gap-4 mb-4 sm:mb-0">
              <img :src="item.image" :alt="item.name" class="w-20 h-20 object-cover rounded-md">
              <div>
                <h2 class="text-lg font-semibold">{{ item.name }}</h2>
                <p class="text-sm text-gray-400">Cor: {{ item.color }}, Tamanho: {{ item.size }}</p>
                <button @click="store.removeFromCart(item.cartItemId)" class="text-red-500 hover:text-red-400 text-sm mt-1">Remover</button>
              </div>
            </div>
            <div class="flex items-center gap-4 w-full sm:w-auto justify-between">
              <div class="flex items-center border border-gray-600 rounded">
                <button @click="decreaseQuantity(item)" class="px-3 py-1 hover:bg-gray-700">-</button>
                <span class="px-4">{{ item.quantity }}</span>
                <button @click="increaseQuantity(item)" class="px-3 py-1 hover:bg-gray-700">+</button>
              </div>
              <p class="text-lg font-semibold w-24 text-right">R${{ (item.price * item.quantity).toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>
      <div class="lg:w-1/3">
        <div class="bg-[#1a1a2e] rounded-lg shadow-lg p-6 sticky top-24">
          <h2 class="text-2xl font-bold mb-4 border-b border-gray-700 pb-2">Resumo do pedido</h2>
          <div class="flex justify-between mb-2">
            <span>Subtotal</span>
            <span>R${{ subtotal.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between mb-4 text-gray-400">
            <span>Frete</span>
            <span>R$5.00</span>
          </div>
          <div class="border-t border-gray-700 pt-4 mb-4">
            <div class="flex justify-between font-bold text-xl">
              <span>Total</span>
              <span>R${{ total.toFixed(2) }}</span>
            </div>
          </div>
          
          <RouterLink to="/checkout" class="w-full block text-center bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300">
            Resumo do pedido
          </RouterLink>
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