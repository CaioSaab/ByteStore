<script setup>
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';
</script>

<template>
  <div class="container mx-auto">
    <h1 class="text-3xl font-bold mb-4">Painel do Vendedor</h1>
    <p class="mb-8 text-gray-400">Bem-vindo(a), <span class="text-cyan-400">{{ store.vendor?.email }}</span>!</p>
    <RouterLink :to="{ name: 'add-product' }" class="bg-blue-600 text-white py-3 px-6 rounded-lg hover:bg-blue-700 text-lg font-semibold">
      + Adicionar Novo Anúncio
    </RouterLink>
  </div>
</template>