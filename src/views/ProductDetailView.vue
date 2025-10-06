<script setup>
import { ref, computed } from 'vue';
import { products } from '@/data/products.js';
import { store } from '@/store/index.js';
import { useRouter } from 'vue-router';

const props = defineProps({
  id: String
});

const router = useRouter();

const product = computed(() => products.find(p => p.id === parseInt(props.id)));

const selectedColor = ref('');
const selectedSize = ref('');

// Inicializa as opções quando o produto é carregado
if (product.value) {
  selectedColor.value = product.value.variations.colors[0];
  selectedSize.value = product.value.variations.sizes[0];
}

function handleAddToCart() {
  store.addToCart(product.value, {
    color: selectedColor.value,
    size: selectedSize.value,
  });
  // Opcional: navegar para o carrinho após adicionar
  router.push('/cart');
}
</script>

<template>
  <div class="container mx-auto text-white p-4">
    <div v-if="product" class="bg-[#1a1a2e] rounded-lg shadow-xl overflow-hidden md:flex">
      <div class="md:w-1/2">
          <img :src="product.image" :alt="product.name" class="w-full h-64 md:h-full object-cover">
      </div>
      <div class="p-8 md:w-1/2 flex flex-col justify-center">
          <h1 class="text-4xl font-bold mb-2">{{ product.name }}</h1>
          <p class="text-gray-400 text-lg mb-4">{{ product.category }}</p>
          <p class="text-3xl font-light text-blue-400 mb-6">R${{ product.price.toFixed(2) }}</p>
          
          <div class="mb-6" v-if="product.variations.colors.length > 0 && product.variations.colors[0] !== 'N/A' && product.variations.colors[0] !== 'transparent'">
              <h3 class="text-lg font-semibold mb-2">Cor: <span class="font-normal">{{ selectedColor }}</span></h3>
              <div class="flex space-x-2">
                  <button v-for="color in product.variations.colors" :key="color" @click="selectedColor = color"
                      class="w-8 h-8 rounded-full border-2 transition"
                      :style="{ backgroundColor: color }"
                      :class="{'border-blue-400 ring-2 ring-blue-400': selectedColor === color, 'border-gray-600': selectedColor !== color}">
                  </button>
              </div>
          </div>

          <div class="mb-6" v-if="product.variations.sizes.length > 0 && product.variations.sizes[0] !== 'N/A'">
              <h3 class="text-lg font-semibold mb-2">Tamanho:</h3>
              <div class="flex flex-wrap gap-2">
                  <button v-for="size in product.variations.sizes" :key="size" @click="selectedSize = size"
                      class="px-4 py-2 border rounded-md transition"
                      :class="{'bg-blue-600 border-blue-600 text-white': selectedSize === size, 'border-gray-600 hover:bg-gray-700': selectedSize !== size}">
                      {{ size }}
                  </button>
              </div>
          </div>

          <button @click="handleAddToCart" class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300">
              <i class="fas fa-cart-plus mr-2"></i> Adicionar ao carrinho
          </button>
      </div>
    </div>
    <div v-else class="text-center text-xl">
      <h2 class="text-4xl font-bold">404 - Produto não encontrado</h2>
      <p class="mt-4 text-gray-400">O produto que você está procurando não existe.</p>
      <RouterLink to="/" class="mt-6 inline-block bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
        Voltar para a loja
      </RouterLink>
    </div>
  </div>
</template>