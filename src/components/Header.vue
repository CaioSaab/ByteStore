<script setup>
import { computed } from 'vue';
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';

const cartItemCount = computed(() => {
  return store.cart.reduce((total, item) => total + item.quantity, 0);
});
</script>

<template>
  <header class="bg-[#1a1a2e] text-white shadow-lg sticky top-0 z-50">
    <div class="container mx-auto px-4 py-3 flex justify-between items-center">
<RouterLink to="/" @click="store.setCategory('Todos')" 
    class="text-2xl font-bold tracking-wider bg-gradient-to-r from-cyan-400 to-purple-500 bg-clip-text text-transparent animate-pulse">
    ByteStore
</RouterLink>      
      <nav class="hidden md:flex items-center space-x-6">
        <RouterLink to="/" @click="store.setCategory('Eletrônicos')" class="hover:text-blue-400">Eletrônicos</RouterLink>
        <RouterLink to="/" @click="store.setCategory('Roupas')" class="hover:text-blue-400">Roupas</RouterLink>
        <RouterLink to="/" @click="store.setCategory('Casa')" class="hover:text-blue-400">Casa</RouterLink>
        <RouterLink to="/" @click="store.setCategory('Livros')" class="hover:text-blue-400">Livros</RouterLink>
        <RouterLink to="/" @click="store.setCategory('Esportes e Lazer')" class="hover:text-blue-400">Esportes e Lazer</RouterLink>
        <RouterLink to="/" @click="store.setCategory('Beleza e Cuidados')" class="hover:text-blue-400">Beleza e Cuidados</RouterLink>
      </nav>
      
      <div class="flex items-center space-x-4">
        <div class="relative">
          <input 
            type="text" 
            placeholder="Procurar..." 
            class="bg-[#0e101f] border border-gray-700 rounded-full py-1 px-4 pl-10 focus:outline-none focus:border-blue-500 w-32 sm:w-auto"
            v-model="store.searchTerm"
          >
          <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500">
            <i class="fas fa-search"></i>
          </span>
        </div>
        <RouterLink to="/cart" class="relative text-xl hover:text-blue-400">
            <i class="fas fa-shopping-cart"></i>
            <span v-if="cartItemCount > 0" class="absolute -top-2 -right-2 bg-blue-500 text-xs rounded-full h-5 w-5 flex items-center justify-center">
              {{ cartItemCount }}
            </span>
        </RouterLink>
        <button class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Login
        </button>
      </div>
    </div>
  </header>
</template>