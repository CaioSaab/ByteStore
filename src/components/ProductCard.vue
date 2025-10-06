<script setup>
import { RouterLink } from 'vue-router'
import { addToCart } from '@/cart.js'

defineProps({
  product: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div class="bg-[#10182B] rounded-lg overflow-hidden group">
    <RouterLink :to="`/product/${product.id}`">
      <div class="w-full h-48 bg-gray-700">
          <img :src="product.image" :alt="product.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300">
      </div>
      <div class="p-4">
        <h2 class="text-lg font-semibold text-white truncate">{{ product.name }}</h2>
        <p class="text-sm text-gray-400 capitalize mb-3">{{ product.category }}</p>
        <p class="text-xl font-bold text-white">R$ {{ product.price.toFixed(2) }}</p>
      </div>
    </RouterLink>
    </div>
</template> 