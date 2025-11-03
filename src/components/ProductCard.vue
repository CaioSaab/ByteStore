<template>
  <div class="bg-[#1a1a2e] rounded-lg overflow-hidden shadow-lg transform hover:-translate-y-2 transition-transform duration-300">
    
    <router-link :to="{ name: 'product-detail', params: { id: product.id } }">
      <img :src="product.image" :alt="product.name" class="w-full h-48 object-cover">
      <div class="p-4">
        <h3 class="text-lg font-semibold truncate">{{ product.name }}</h3>
        <p class="text-sm text-gray-400 mb-2">{{ product.category }}</p>
        <p class="text-xl font-bold text-blue-400">${{ product.price.toFixed(2) }}</p>
      </div>
    </router-link>

  </div>
</template>

<script>
export default {
  name: 'ProductCard',
  props: {
    product: {
      type: Object,
      required: true
    }
  }
}
</script>