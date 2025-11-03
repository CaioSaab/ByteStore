<script setup>
import { RouterView, RouterLink, useRouter } from 'vue-router';
import { store } from '@/store/index.js';

const router = useRouter();

function handleVendorLogout() {
  store.vendorLogout();
  router.push('/vendor/login');
}
</script>

<template>
  <div>
    <header class="bg-[#1a1a2e] text-white shadow-lg sticky top-0 z-50">
      <div class="container mx-auto px-4 py-3 flex justify-between items-center">
        <span class="text-2xl font-bold tracking-wider text-purple-400">
          ByteStore <span class="text-sm font-light text-gray-400">- Portal do Vendedor</span>
        </span>
        <nav class="flex items-center space-x-4 md:space-x-6">
          <RouterLink :to="{ name: 'vendor-dashboard' }" class="hover:text-blue-400 text-sm md:text-base">Painel</RouterLink>
          <RouterLink :to="{ name: 'add-product' }" class="hover:text-blue-400 text-sm md:text-base">Adicionar Anúncio</RouterLink>
          <RouterLink :to="{ name: 'vendor-questions' }" class="hover:text-blue-400 text-sm md:text-base">Perguntas</RouterLink>
          <RouterLink :to="{ name: 'vendor-sales' }" class="hover:text-blue-400 text-sm md:text-base">Gestão de Vendas</RouterLink>
          
          <button @click="handleVendorLogout" class="bg-red-600 hover:bg-red-700 text-white font-bold py-2 px-4 rounded-lg text-sm">
            Logout
          </button>
        </nav>
      </div>
    </header>

    <main class="p-4 sm:p-6 lg:p-8">
      <RouterView />
    </main>
  </div>
</template>