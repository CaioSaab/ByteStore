<script setup>
import { ref, computed, onMounted } from 'vue';
import { store } from '@/store/index.js';
import { useRouter, RouterLink } from 'vue-router';
import { getProductById, addQuestion, getProductQuestions } from '@/services/productService';

const props = defineProps({
  id: String
});

const router = useRouter();
const product = ref(null);
const productQuestions = ref([]);
const selectedColor = ref('');
const selectedSize = ref('');
const questionText = ref('');
const isLoading = ref(false);
const errorMessage = ref('');

const colorNameMap = {
  '#2c3e50': 'Grafite', '#ecf0f1': 'Prata Claro', '#3498db': 'Azul', '#ffffff': 'Branco', '#000000': 'Preto',
  '#7f8c8d': 'Cinza', '#bdc3c7': 'Prata', '#f1c40f': 'Amarelo', '#9b59b6': 'Roxo', '#e74c3c': 'Vermelho',
  '#16a085': 'Verde Mar', '#1abc9c': 'Turquesa', '#e67e22': 'Laranja', '#d35400': 'Laranja Escuro', 'transparent': 'Transparente',
  '#34495e': 'Azul Escuro', '#c0392b': 'Vermelho Escuro', '#8e44ad': 'Roxo Escuro', '#2980b9': 'Azul Médio', '#f39c12': 'Dourado',
  '#95a5a6': 'Cinza Claro', '#27ae60': 'Verde'
};

const selectedColorName = computed(() => {
  return colorNameMap[selectedColor.value] || selectedColor.value;
});

const loadProduct = async () => {
  if (!props.id) return;
  
  isLoading.value = true;
  errorMessage.value = '';
  
  try {
    const result = await getProductById(props.id);
    if (result.success) {
      product.value = result.data;
      
      if (product.value?.variations) {
        selectedColor.value = product.value.variations.colors?.[0] || '';
        selectedSize.value = product.value.variations.sizes?.[0] || '';
      }
      
      await loadQuestions();
    } else {
      errorMessage.value = result.error || 'Produto não encontrado';
    }
  } catch (error) {
    errorMessage.value = error.message || 'Erro ao carregar produto';
  } finally {
    isLoading.value = false;
  }
};

const loadQuestions = async () => {
  if (!props.id) return;
  
  try {
    const result = await getProductQuestions(props.id);
    if (result.success) {
      productQuestions.value = result.data.questions || result.data || [];
    }
  } catch (error) {
    console.error('Erro ao carregar perguntas:', error);
  }
};

onMounted(async () => {
  await loadProduct();
});

const handleAddToCart = async () => {
  if (!product.value) return;
  
  if (!store.isAuthenticated) {
    router.push({ name: 'login' });
    return;
  }
  
  await store.addToCart(product.value, {
    color: selectedColor.value,
    size: selectedSize.value,
  });
  
  router.push({ name: 'cart' });
};

const handleSendQuestion = async () => {
  if (!product.value || !questionText.value.trim()) {
    errorMessage.value = 'Por favor, digite sua pergunta.';
    return;
  }

  if (!store.isAuthenticated) {
    errorMessage.value = 'Você precisa estar logado para fazer uma pergunta.';
    return;
  }

  try {
    const result = await addQuestion(product.value.id, questionText.value);
    if (result.success) {
      questionText.value = '';
      errorMessage.value = '';
      await loadQuestions();
    } else {
      errorMessage.value = result.error || 'Erro ao enviar pergunta';
    }
  } catch (error) {
    errorMessage.value = error.message || 'Erro ao enviar pergunta';
  }
};
</script>

<template>
  <div class="container mx-auto text-white p-4">
    <div v-if="isLoading" class="text-center text-gray-400 mt-10">
      Carregando detalhes do produto...
    </div>
    <div v-else-if="errorMessage && !product" class="text-center text-red-400 mt-10">
      {{ errorMessage }}
    </div>
    <div v-else-if="product !== null">
      <div v-if="product" class="bg-[#1a1a2e] rounded-lg shadow-xl overflow-hidden">

        <div class="md:flex">
          <div class="md:w-1/2 p-4 flex justify-center items-center">
             <img :src="product.image || product.images?.[0]" :alt="product.name" class="w-full h-auto max-h-[500px] object-contain rounded-lg">
          </div>

          <div class="p-8 md:w-1/2 flex flex-col justify-center">
              <h1 class="text-4xl font-bold mb-2">{{ product.name }}</h1>
              <p class="text-gray-400 text-lg mb-4">{{ product.category }}</p>
              <p class="text-3xl font-light text-blue-400 mb-4">R${{ product.price.toFixed(2) }}</p>

              <p v-if="product.description" class="text-gray-300 mb-6 text-sm leading-relaxed">
                {{ product.description }}
              </p>
              <div class="mb-6" v-if="product.variations?.colors?.length > 0 && product.variations.colors[0] !== 'N/A' && product.variations.colors[0] !== 'transparent'">
                  <h3 class="text-lg font-semibold mb-2">Cor: <span class="font-normal capitalize">{{ selectedColorName }}</span></h3>
                  <div class="flex space-x-2">
                      <button v-for="colorHex in product.variations.colors" :key="colorHex" @click="selectedColor = colorHex"
                          class="w-8 h-8 rounded-full border-2 transition"
                          :style="{ backgroundColor: colorHex }"
                          :class="{'border-blue-400 ring-2 ring-blue-400': selectedColor === colorHex, 'border-gray-600': selectedColor !== colorHex}">
                      </button>
                  </div>
              </div>

              <div class="mb-6" v-if="product.variations?.sizes?.length > 0 && product.variations.sizes[0] !== 'N/A'">
                  <h3 class="text-lg font-semibold mb-2">Tamanho:</h3>
                  <div class="flex flex-wrap gap-2">
                      <button v-for="size in product.variations.sizes" :key="size" @click="selectedSize = size"
                          class="px-4 py-2 border rounded-md transition"
                          :class="{'bg-blue-600 border-blue-600 text-white': selectedSize === size, 'border-gray-600 hover:bg-gray-700': selectedSize !== size}">
                          {{ size }}
                      </button>
                  </div>
              </div>

              <button @click="handleAddToCart" class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300">
                  <i class="fas fa-cart-plus mr-2"></i> Adicionar ao carrinho
              </button>
          </div>
        </div>

        <div class="p-8 border-t border-gray-700">
          <h2 class="text-2xl font-bold mb-6">Perguntas e Respostas</h2>

          <div v-if="store.isAuthenticated" class="mb-8 p-4 bg-black/20 rounded-lg">
            <h3 class="text-xl font-semibold mb-3">Faça sua pergunta</h3>
            <div v-if="errorMessage" class="text-red-400 text-sm mb-2">
              {{ errorMessage }}
            </div>
            <textarea
              v-model="questionText"
              class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition mb-2"
              rows="3"
              placeholder="Digite sua dúvida..."
            ></textarea>
            <button @click="handleSendQuestion" class="w-full sm:w-auto bg-gray-600 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded-lg">
              Enviar Pergunta
            </button>
          </div>
          <div v-else class="mb-8 text-center text-gray-400 text-sm">
            <RouterLink :to="{ name: 'login' }" class="text-blue-400 hover:underline">Faça login</RouterLink> para fazer uma pergunta.
          </div>

          <h3 class="text-xl font-semibold mb-4">Últimas perguntas</h3>
          <div v-if="productQuestions.length > 0" class="space-y-6">
            <div v-for="qa in productQuestions" :key="qa.id || Date.now()" class="border-b border-gray-700 pb-4">
              <p class="mb-1 text-gray-300"><i class="fas fa-question-circle mr-2 text-blue-400"></i>{{ qa?.question }}</p>
              <div v-if="qa?.answer" class="ml-6 mt-2 text-gray-400">
                <p><i class="fas fa-reply mr-2 text-green-400"></i>{{ qa.answer }}</p>
              </div>
               <div v-else class="ml-6 mt-2 text-gray-500 text-sm">
                <p>Aguardando resposta do vendedor...</p>
              </div>
            </div>
          </div>
          <div v-else class="text-gray-500">
            Nenhuma pergunta feita ainda para este produto. Seja o primeiro!
          </div>
        </div>
      </div>
      <div v-else class="text-center text-xl mt-10">
        <h2 class="text-4xl font-bold">404 - Produto não encontrado</h2>
        <p class="mt-4 text-gray-400">O produto que você está procurando não existe.</p>
        <RouterLink :to="{ name: 'home' }" class="mt-6 inline-block bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Voltar para a loja
        </RouterLink>
      </div>
    </div>
    <div v-else class="text-center text-gray-500 mt-10">Carregando detalhes do produto...</div>
  </div>
</template>