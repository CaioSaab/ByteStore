<script setup>
import { RouterLink } from 'vue-router'
import { cartItemCount } from '@/cart.js'
</script>

<template>
  <header class="bg-[#10182B] border-b border-gray-700/50">
    <div class="container mx-auto flex justify-between items-center p-4">
      <RouterLink to="/" class="text-2xl font-bold text-white tracking-wider">ByteStore</RouterLink>

      <nav class="hidden md:flex gap-8 text-gray-400">
        <a href="#" class="hover:text-white transition-colors">Eletrônicos</a>
        <a href="#" class="hover:text-white transition-colors">Roupas</a>
        <a href="#" class="hover:text-white transition-colors">Casa</a>
        <a href="#" class="hover:text-white transition-colors">Livros</a>
        <a href="#" class="hover:text-white transition-colors">Esporte e Lazer</a>
        <a href="#" class="hover:text-white transition-colors">Beleza e Cuidados</a>
      </nav>

      <div class="flex items-center gap-4">
        <div class="relative">
            <input type="text" placeholder="Procurar..." class="bg-[#1A233A] text-gray-300 rounded-md py-1.5 pl-3 pr-8 w-48 focus:outline-none focus:ring-2 focus:ring-blue-500">
            <i class="fas fa-search absolute right-2.5 top-1/2 -translate-y-1/2 text-gray-500"></i>
        </div>
        <RouterLink to="/cart" class="text-xl text-gray-400 hover:text-white transition-colors">
            🛒
            <span v-if="cartItemCount > 0" class="text-xs absolute top-2 right-14 bg-blue-500 text-white rounded-full px-1.5 py-0.5">{{ cartItemCount }}</span>
        </RouterLink>
        <button class="bg-blue-600 text-white font-semibold px-4 py-1.5 rounded-md hover:bg-blue-700 transition-colors">Login</button>
      </div>
    </div>
  </header>
</template>