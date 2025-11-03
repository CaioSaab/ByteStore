<script setup>
import { ref } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { store } from '@/store/index.js';

const email = ref('');
const password = ref('');
const errorMessage = ref('');
const isLoading = ref(false);
const router = useRouter();

const validateForm = () => {
  if (!email.value || !password.value) {
    errorMessage.value = 'Por favor, preencha todos os campos';
    return false;
  }
  return true;
};

const handleVendorLogin = async () => {
  errorMessage.value = '';
  
  if (!validateForm()) {
    return;
  }

  isLoading.value = true;
  const result = await store.vendorLogin(email.value, password.value);
  isLoading.value = false;

  if (result.success) {
    router.push({ name: 'vendor-dashboard' });
  } else {
    errorMessage.value = result.error || 'Erro ao fazer login';
  }
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#0e101f] p-4 text-white">
    <div class="w-full max-w-4xl lg:grid lg:grid-cols-2 overflow-hidden rounded-2xl shadow-2xl bg-[#1a1a2e] shadow-purple-500/10">

      <div class="hidden lg:flex justify-center items-center p-8 bg-black/10">
        <img 
          src="https://img.freepik.com/vetores-gratis/participacao-nos-negocios-calculo-de-dividendos-relacao-percentual-tamanho-da-contribuicao-valor-do-deposito-contabilidade-e-auditoria-personagens-de-desenhos-animados-para-acionistas_335657-2986.jpg?ga=GA1.1.2063890307.1760399720&semt=ais_hybrid&w=740&q=80" 
          alt="Ilustração de Negócios e Vendas" 
          class="max-h-[400px] w-auto object-contain rounded-lg"
        >
      </div>

      <div class="p-8 sm:p-12 flex flex-col justify-center">
        <h2 class="text-3xl font-bold text-center text-white mb-6">Portal do Vendedor</h2>
        <form @submit.prevent="handleVendorLogin" class="space-y-6">
          <div class="relative">
             <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500"><i class="fas fa-envelope"></i></span>
            <input 
              v-model="email" 
              type="email" 
              id="email" 
              placeholder="Email de Vendedor"
              class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-3 px-10 text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition" 
              required
            >
          </div>
          <div class="relative">
             <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500"><i class="fas fa-lock"></i></span>
            <input 
              v-model="password" 
              type="password" 
              id="password" 
              placeholder="Senha"
              class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-3 px-10 text-white focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition" 
              required
            >
          </div>
          <div v-if="errorMessage" class="text-red-400 text-sm mb-2">
            {{ errorMessage }}
          </div>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="w-full bg-purple-600 hover:bg-purple-700 disabled:opacity-50 disabled:cursor-not-allowed text-white font-bold py-3 px-4 rounded-lg transition-transform duration-200 hover:scale-105">
            <span v-if="isLoading">Carregando...</span>
            <span v-else>Entrar</span>
          </button>
        </form>
        <p class="text-center text-gray-400 text-sm mt-6">
          Ainda não é um vendedor? 
          <RouterLink to="/vendor/register" class="text-blue-400 hover:underline font-bold">Cadastre sua loja</RouterLink>
        </p>
         <p class="text-center text-gray-400 text-sm mt-4">
          <RouterLink to="/login" class="hover:underline">Voltar para a loja</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>