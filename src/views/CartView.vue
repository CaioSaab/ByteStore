<script setup>
import { computed, ref, onMounted } from 'vue';
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';
import { calculateShipping } from '@/services/shippingService';

const cepInput = ref('');
const calculatedShippingCost = ref(null);
const isCalculatingShipping = ref(false);
const shippingError = ref('');

const subtotal = computed(() => {
  return store.cart.reduce((acc, item) => acc + (item.price * item.quantity), 0);
});

const total = computed(() => {
  const shipping = calculatedShippingCost.value ?? 0;
  const discount = store.discountPercentage ? subtotal.value * store.discountPercentage : 0;
  return subtotal.value + shipping - discount;
});

const validateCep = (cep) => {
  const cleanCep = cep.replace(/\D/g, '');
  return cleanCep.length === 8;
};

const handleCalculateShipping = async () => {
  if (!validateCep(cepInput.value)) {
    shippingError.value = 'Por favor, insira um CEP válido com 8 dígitos.';
    return;
  }

  isCalculatingShipping.value = true;
  shippingError.value = '';
  calculatedShippingCost.value = null;

  try {
    const result = await calculateShipping(cepInput.value);
    if (result.success) {
      calculatedShippingCost.value = result.data.cost || result.data.price || 0;
    } else {
      shippingError.value = result.error || 'Erro ao calcular frete';
    }
  } catch (error) {
    shippingError.value = error.message || 'Erro ao calcular frete';
  } finally {
    isCalculatingShipping.value = false;
  }
};

const increaseQuantity = async (item) => {
  await store.updateCartItemQuantity(item.cartItemId, item.quantity + 1);
};

const decreaseQuantity = async (item) => {
  if (item.quantity > 1) {
    await store.updateCartItemQuantity(item.cartItemId, item.quantity - 1);
  } else {
    await store.removeFromCart(item.cartItemId);
  }
};

onMounted(async () => {
  if (store.isAuthenticated && store.cart.length === 0) {
    await store.refreshCart();
  }
});
</script>

<template>
  <div class="container mx-auto p-4 text-white">
    <h1 class="text-3xl font-bold mb-6">Seu Carrinho</h1>
    <div v-if="store.cart.length > 0" class="flex flex-col lg:flex-row gap-8">
      <div class="lg:w-2/3">
        <div class="bg-[#1a1a2e] rounded-lg shadow-lg p-4 sm:p-6 space-y-4">
          <div v-for="item in store.cart" :key="item.cartItemId" class="flex flex-col sm:flex-row items-start sm:items-center justify-between border-b border-gray-700 pb-4">
            <div class="flex items-center gap-4 mb-4 sm:mb-0">
              <RouterLink :to="{ name: 'product-detail', params: { id: item.id } }">
                <img :src="item.image" :alt="item.name" class="w-20 h-20 object-cover rounded-md hover:opacity-80 transition-opacity">
              </RouterLink>
              <div>
                <RouterLink :to="{ name: 'product-detail', params: { id: item.id } }">
                  <h2 class="text-lg font-semibold hover:text-blue-400 transition-colors">{{ item.name }}</h2>
                </RouterLink>
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

          <div class="mb-4 pb-4 border-b border-gray-700">
            <label for="cep" class="block text-sm font-medium mb-1">Calcular Frete</label>
            <div class="flex">
              <input v-model="cepInput" type="text" id="cep" maxlength="9"
                     class="w-full bg-[#0e101f] border border-gray-600 rounded-l-md p-2 focus:outline-none focus:border-blue-500 placeholder-gray-500"
                     placeholder="00000-000"
                     @input="cepInput = cepInput.replace(/\D/g, '').replace(/^(\d{5})(\d)/,'$1-$2')">
              <button @click="handleCalculateShipping"
                      class="bg-blue-600 text-white px-4 rounded-r-md hover:bg-blue-700 disabled:opacity-50 flex items-center justify-center w-[100px]"
                      :disabled="isCalculatingShipping || !validateCep(cepInput)">
                <span v-if="!isCalculatingShipping">Calcular</span>
                <svg v-else class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              </button>
            </div>
            <div v-if="shippingError" class="mt-2 text-sm text-red-400">
              {{ shippingError }}
            </div>
            <div v-if="calculatedShippingCost !== null && !isCalculatingShipping" class="mt-2 text-sm">
              <span>Frete: </span>
              <span class="font-semibold text-blue-400">R$ {{ calculatedShippingCost.toFixed(2) }}</span>
            </div>
            <div v-if="isCalculatingShipping" class="mt-2 text-sm text-gray-400">
              Calculando frete...
            </div>
          </div>

          <div class="flex justify-between mb-2">
            <span>Subtotal</span>
            <span>R${{ subtotal.toFixed(2) }}</span>
          </div>
          <div v-if="store.discountPercentage > 0" class="flex justify-between mb-2 text-green-400">
            <span>Desconto ({{ (store.discountPercentage * 100).toFixed(0) }}%)</span>
            <span>-R${{ (subtotal * store.discountPercentage).toFixed(2) }}</span>
          </div>
          <div v-if="calculatedShippingCost !== null" class="flex justify-between mb-4 text-gray-400">
            <span>Frete</span>
            <span>R${{ calculatedShippingCost.toFixed(2) }}</span>
          </div>
          <div v-else class="flex justify-between mb-4 text-gray-500 text-sm">
            <span>Frete</span>
            <span>(A calcular)</span>
          </div>

          <div class="border-t border-gray-700 pt-4 mb-4">
            <div class="flex justify-between font-bold text-xl">
              <span>Total</span>
              <span>R${{ total.toFixed(2) }}</span>
            </div>
          </div>

          <RouterLink :to="{ name: 'checkout' }"
                      class="w-full block text-center bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300"
                      :class="{ 'opacity-50 cursor-not-allowed pointer-events-none': calculatedShippingCost === null }">
            Ir para o Checkout
          </RouterLink>
          <p v-if="calculatedShippingCost === null" class="text-xs text-center text-gray-500 mt-2">Calcule o frete para continuar</p>
        </div>
      </div>
    </div>
    <div v-else class="text-center bg-[#1a1a2e] p-10 rounded-lg">
      <h2 class="text-2xl">Seu carrinho está vazio.</h2>
      <RouterLink :to="{ name: 'home' }" class="mt-4 inline-block bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
        Continuar comprando
      </RouterLink>
    </div>
  </div>
</template>