<script setup>
import { ref } from 'vue';
import { RouterLink } from 'vue-router';

const email = ref('');

function handlePasswordReset() {
  alert(`Enviando link de recuperação para: ${email.value}`);
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#0e101f] p-4">
    <div class="w-full max-w-md">
      <div class="bg-[#1a1a2e] p-8 rounded-2xl shadow-lg">
        <h2 class="text-3xl font-bold text-center text-white mb-4">Recuperar Senha</h2>
        <p class="text-center text-gray-400 text-sm mb-6">
          Digite seu e-mail e enviaremos um link para você voltar a acessar sua conta.
        </p>
        <form @submit.prevent="handlePasswordReset">
          <div class="mb-6">
            <label for="email" class="block text-gray-300 text-sm font-bold mb-2">Email</label>
            <input v-model="email" type="email" id="email" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:border-blue-500" required>
          </div>
          <button type="submit" class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-lg transition-colors">
            Enviar Link de Recuperação
          </button>
        </form>
        <p class="text-center text-gray-400 text-sm mt-6">
          Lembrou a senha? 
          <RouterLink to="/login" class="text-blue-400 hover:underline font-bold">Voltar para o login</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>