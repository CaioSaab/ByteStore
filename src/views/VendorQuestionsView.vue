<script setup>
import { ref, onMounted, computed } from 'vue';
import { getVendorQuestions, answerVendorQuestion } from '@/services/productService';

const questions = ref([]);
const isLoading = ref(false);
const errorMessage = ref('');
const answerText = ref({});
const collapsed = ref({});
const openQuestions = computed(() => Array.isArray(questions.value) ? questions.value.filter(q => q && !q.answer) : []);
const answeredQuestions = computed(() => Array.isArray(questions.value) ? questions.value.filter(q => q && !!q.answer) : []);

const loadQuestions = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    const result = await getVendorQuestions();
    if (result.success) {
      questions.value = result.data || [];
    } else {
      errorMessage.value = result.error || 'Erro ao carregar perguntas';
    }
  } catch (e) {
    errorMessage.value = 'Erro ao carregar perguntas';
  } finally {
    isLoading.value = false;
  }
};

const handleAnswer = async (q) => {
  if (!answerText.value[q.id] || !answerText.value[q.id].trim()) return;
  const text = answerText.value[q.id].trim();
  const result = await answerVendorQuestion(q.id, text);
  if (result.success) {
    q.answer = text;
    answerText.value[q.id] = '';
  } else {
    alert(result.error || 'Erro ao responder pergunta');
  }
};

onMounted(loadQuestions);
</script>

<template>
  <div class="container mx-auto">
    <h1 class="text-3xl font-bold mb-8">Perguntas sobre seus Anúncios</h1>

    <div v-if="isLoading" class="text-center text-gray-400 py-10">Carregando...</div>
    <div v-else-if="errorMessage" class="text-center text-red-400 py-10">{{ errorMessage }}</div>

    <div v-else class="space-y-6">
      <div v-if="questions.length === 0" class="text-gray-400">Nenhuma pergunta recebida ainda.</div>
      <div v-for="q in openQuestions" :key="q.id" class="bg-[#1a1a2e] p-6 rounded-lg">
        <p class="text-sm text-gray-400">Produto: <span class="text-cyan-400">{{ q.productName }}</span></p>
        <p class="text-sm text-gray-400">Cliente: {{ q.clientName }}</p>
        <p class="text-lg my-2">"{{ q.question }}"</p>

        <div class="mt-2">
          <textarea v-model="answerText[q.id]" class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white" rows="2" placeholder="Digite sua resposta..."></textarea>
          <button @click="handleAnswer(q)" class="mt-2 bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-lg">Responder</button>
        </div>
      </div>

      <div v-for="q in answeredQuestions" :key="q.id" class="bg-[#1a1a2e] p-6 rounded-lg">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-400">Produto: <span class="text-cyan-400">{{ q.productName }}</span></p>
            <p class="text-sm text-gray-400">Cliente: {{ q.clientName }}</p>
          </div>
          <button @click="collapsed[q.id] = !collapsed[q.id]" class="text-gray-300 hover:text-white">
            <i :class="collapsed[q.id] ? 'fas fa-chevron-down' : 'fas fa-chevron-up'"></i>
          </button>
        </div>
        <div v-show="!collapsed[q.id]">
          <p class="text-lg my-2">"{{ q.question }}"</p>
          <div class="mt-2 text-green-300">
            <i class="fas fa-reply mr-2"></i>{{ q.answer }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
