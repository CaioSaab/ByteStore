<script setup>
import { ref } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import { store } from '@/store/index.js';

const name = ref('');
const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const errorMessage = ref('');
const isLoading = ref(false);
const router = useRouter();

const validateForm = () => {
  if (!name.value || !email.value || !password.value || !confirmPassword.value) {
    errorMessage.value = 'Por favor, preencha todos os campos';
    return false;
  }

  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'As senhas não coincidem';
    return false;
  }

  if (password.value.length < 6) {
    errorMessage.value = 'A senha deve ter pelo menos 6 caracteres';
    return false;
  }

  return true;
};

const handleRegister = async () => {
  errorMessage.value = '';
  
  if (!validateForm()) {
    return;
  }

  isLoading.value = true;
  const result = await store.register({
    name: name.value,
    email: email.value,
    password: password.value,
  });
  isLoading.value = false;

  if (result.success) {
    router.push('/app/home');
  } else {
    errorMessage.value = result.error || 'Erro ao registrar';
  }
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#0e101f] p-4">
    <div class="w-full max-w-md">
      <div class="bg-[#1a1a2e] p-8 rounded-2xl shadow-lg">
        <h2 class="text-3xl font-bold text-center text-white mb-6">Criar Conta</h2>
        <form @submit.prevent="handleRegister">
          <div class="mb-4">
            <label for="name" class="block text-gray-300 text-sm font-bold mb-2">Nome</label>
            <input v-model="name" type="text" id="name" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:border-blue-500" required>
          </div>
          <div class="mb-4">
            <label for="email" class="block text-gray-300 text-sm font-bold mb-2">Email</label>
            <input v-model="email" type="email" id="email" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:border-blue-500" required>
          </div>
          <div class="mb-4">
            <label for="password" class="block text-gray-300 text-sm font-bold mb-2">Senha</label>
            <input v-model="password" type="password" id="password" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:border-blue-500" required>
          </div>
          <div class="mb-6">
            <label for="confirmPassword" class="block text-gray-300 text-sm font-bold mb-2">Confirmar Senha</label>
            <input v-model="confirmPassword" type="password" id="confirmPassword" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:border-blue-500" required>
          </div>
          <div v-if="errorMessage" class="text-red-400 text-sm mb-2">
            {{ errorMessage }}
          </div>
          <button 
            type="submit" 
            :disabled="isLoading"
            class="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed text-white font-bold py-2 px-4 rounded-lg transition-colors">
            <span v-if="isLoading">Cadastrando...</span>
            <span v-else>Cadastrar</span>
          </button>
        </form>
        <p class="text-center text-gray-400 text-sm mt-6">
          Já tem uma conta? 
          <RouterLink to="/login" class="text-blue-400 hover:underline font-bold">Faça login</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>