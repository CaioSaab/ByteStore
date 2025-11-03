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

const handleLogin = async () => {
  errorMessage.value = '';
  
  if (!validateForm()) {
    return;
  }

  isLoading.value = true;
  const result = await store.login(email.value, password.value);
  isLoading.value = false;

  if (result.success) {
    router.push('/app/home');
  } else {
    errorMessage.value = result.error || 'Erro ao fazer login';
  }
};
</script>

<template>
  <div class="min-h-screen bg-[#0e101f] text-gray-200 flex items-center justify-center p-4 relative">
    <RouterLink to="/vendor/login" class="absolute top-4 right-4 bg-gray-700 text-white py-2 px-4 rounded-lg text-sm hover:bg-gray-600 transition-colors">
      Área do Vendedor
    </RouterLink>

    <div class="w-full max-w-4xl lg:grid lg:grid-cols-2 overflow-hidden rounded-2xl shadow-2xl shadow-blue-500/10">

      <div class="hidden lg:block relative overflow-hidden">
        <img
          src="https://img.freepik.com/fotos-gratis/desconto-para-a-temporada-de-compras-com-vendas_23-2150165924.jpg"
          alt="ByteStore Promotion"
          class="w-full h-full object-cover animate-slow-zoom"
        >
        <div class="absolute inset-0 bg-gradient-to-t from-[#0e101f] via-transparent to-transparent"></div>
      </div>

      <div class="bg-[#1a1a2e] p-8 sm:p-12 flex flex-col justify-center">
        <div class="text-center mb-10">
          <h1 class="text-4xl font-bold tracking-wider bg-gradient-to-r from-cyan-400 to-purple-500 bg-clip-text text-transparent">
            ByteStore
          </h1>
          <p class="text-gray-400 mt-2 text-sm">Bem-vindo(a) de volta.</p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-6">

          <div>
            <label for="email" class="block text-gray-400 text-xs font-bold mb-2 uppercase tracking-wide">Email</label>
            <div class="relative flex items-center border border-gray-700 rounded-lg focus-within:ring-2 focus-within:ring-blue-500 focus-within:border-transparent transition duration-200 bg-[#0e101f]">
              <span class="pl-3 pr-2 text-gray-500">
                <i class="fas fa-envelope"></i>
              </span>
              <input
                v-model="email"
                type="email"
                id="email"
                placeholder="seuemail@exemplo.com"
                class="w-full py-3 pr-4 bg-transparent text-white focus:outline-none placeholder-gray-500"
                required
              >
            </div>
          </div>

          <div>
             <label for="password" class="block text-gray-400 text-xs font-bold mb-2 uppercase tracking-wide">Senha</label>
            <div class="relative flex items-center border border-gray-700 rounded-lg focus-within:ring-2 focus-within:ring-blue-500 focus-within:border-transparent transition duration-200 bg-[#0e101f]">
               <span class="pl-3 pr-2 text-gray-500">
                <i class="fas fa-lock"></i>
              </span>
              <input
                v-model="password"
                type="password"
                id="password"
                placeholder="********"
                class="w-full py-3 pr-4 bg-transparent text-white focus:outline-none placeholder-gray-500"
                required
              >
            </div>
            <div class="text-right mt-2">
              <RouterLink to="/forgot-password" class="text-sm text-blue-400 hover:underline">Esqueceu a senha?</RouterLink>
            </div>
          </div>

          <div v-if="errorMessage" class="text-red-400 text-sm mb-2">
            {{ errorMessage }}
          </div>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed text-white font-bold py-3 px-4 rounded-lg transition-transform duration-200 hover:scale-105 shadow-lg shadow-blue-500/30">
            <span v-if="isLoading">Carregando...</span>
            <span v-else>Login</span>
          </button>
        </form>

        <p class="text-center text-gray-400 text-sm mt-8">
          Não tem uma conta?
          <RouterLink to="/register" class="text-blue-400 hover:underline font-bold">Crie uma agora</RouterLink>
        </p>
      </div>

    </div>
  </div>
</template>