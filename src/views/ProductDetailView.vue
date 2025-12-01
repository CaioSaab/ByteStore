<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { store } from '@/store/index.js'; // Certifique-se que o caminho da store está correto
import { useRouter, RouterLink } from 'vue-router';
// Certifique-se que o caminho para seu serviço está correto
import { getProductById, addQuestion, getProductQuestions } from '@/services/productService'; 

const props = defineProps({
  id: String
});

const router = useRouter();
const product = ref(null); // O objeto "Pai" (ex: { nome: "Camiseta", variations: [...] })
const productQuestions = ref([]);
const isLoading = ref(false);
const errorMessage = ref('');
const questionText = ref('');

// --- 1. ESTADOS REATIVOS PARA VARIAÇÃO ---
// Controlam qual variação está selecionada
const selectedColor = ref(null);
const selectedSize = ref(null);

// --- 2. LÓGICA DE CARREGAMENTO (CORRIGIDA) ---
const loadProduct = async () => {
  if (!props.id) return;
  isLoading.value = true;
  errorMessage.value = '';
  
  try {
    const result = await getProductById(props.id);
    if (result.success && result.data) {
      product.value = result.data; // ex: { nome: "...", variations: [...] }
      
      // Inicializa a seleção com a primeira variação disponível
      if (product.value.variations && product.value.variations.length > 0) {
        selectedColor.value = product.value.variations[0].cor;
        selectedSize.value = product.value.variations[0].tamanho;
      } else {
        errorMessage.value = "Este produto não possui variações cadastradas.";
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

// --- 3. COMPUTED PROPERTIES (A LÓGICA) ---

// Extrai as cores únicas da lista de variações
const availableColors = computed(() => {
  if (!product.value?.variations) return [];
  const colors = product.value.variations.map(v => v.cor).filter(Boolean); // Filtra nulos/vazios
  return [...new Set(colors)]; // Retorna um array de cores únicas
});

// Extrai os tamanhos únicos *baseado na cor já selecionada*
const availableSizes = computed(() => {
  if (!product.value?.variations) return [];
  
  // Filtra as variações pela cor selecionada
  const sizes = product.value.variations
    .filter(v => v.cor === selectedColor.value) 
    .map(v => v.tamanho)
    .filter(Boolean);
    
  return [...new Set(sizes)]; // Retorna tamanhos únicos para aquela cor
});

// Encontra a Variação (Filho) exata que o usuário selecionou
const selectedVariation = computed(() => {
  if (!product.value?.variations) return null;

  // Encontra a variação que bate com a cor E o tamanho
  return product.value.variations.find(v => {
    return v.cor === selectedColor.value && v.tamanho === selectedSize.value;
  });
});

const selectedImageIndex = ref(0);

watch(selectedVariation, (nv) => {
  selectedImageIndex.value = 0;
});

// --- 4. FUNÇÕES DE EVENTO (CORRIGIDAS) ---

// Quando o usuário clica em uma cor, reseta o tamanho
const selectColor = (color) => {
  selectedColor.value = color;
  
  // Pega o primeiro tamanho disponível para esta nova cor
  // Recalcula os tamanhos disponíveis na hora
  const sizesForNewColor = product.value.variations
    .filter(v => v.cor === color)
    .map(v => v.tamanho)
    .filter(Boolean);
  const uniqueSizes = [...new Set(sizesForNewColor)];
  
  selectedSize.value = uniqueSizes[0] || null;
};

const handleAddToCart = async () => {
  if (!selectedVariation.value) {
    alert("Por favor, selecione uma cor e tamanho válidos.");
    return;
  }
  
  if (!store.isAuthenticated) {
    router.push({ name: 'login' });
    return;
  }

  // Envia a VariaÇÃO (Filho) para o carrinho
  await store.addToCart({
    productId: product.value.id, // ID do Pai
    variationId: selectedVariation.value.id, // ID do Filho
    name: product.value.nome, // Nome do Pai
    price: selectedVariation.value.preco,
    image: selectedVariation.value.imageUrls[0],
    color: selectedVariation.value.cor,
    size: selectedVariation.value.tamanho,
  });
  
  router.push({ name: 'cart' });
};

// --- Funções de Pergunta e Mapa de Cores (Sem alteração crítica) ---
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

const colorNameToHex = {
  'Grafite': '#2c3e50',
  'Prata Claro': '#ecf0f1',
  'Azul': '#3498db',
  'Branco': '#ffffff',
  'Preto': '#000000',
  'Cinza': '#7f8c8d',
  'Prata': '#bdc3c7',
  'Amarelo': '#f1c40f',
  'Roxo': '#9b59b6',
  'Vermelho': '#e74c3c',
  'Verde Mar': '#16a085',
  'Turquesa': '#1abc9c',
  'Laranja': '#e67e22',
  'Laranja Escuro': '#d35400',
  'Transparente': 'transparent',
  'Azul Escuro': '#34495e',
  'Vermelho Escuro': '#c0392b',
  'Roxo Escuro': '#8e44ad',
  'Azul Médio': '#2980b9',
  'Dourado': '#f39c12',
  'Cinza Claro': '#95a5a6',
  'Verde': '#27ae60'
};

const normalizeColor = (c) => {
  if (!c) return '#7f8c8d';
  const isHex = /^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$/.test(c);
  if (isHex) return c;
  const lower = c.toLowerCase();
  for (const name in colorNameToHex) {
    if (name.toLowerCase() === lower) return colorNameToHex[name];
  }
  return '#7f8c8d';
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
      <div v-if="product && selectedVariation" class="bg-[#1a1a2e] rounded-lg shadow-xl overflow-hidden">

        <div class="md:flex">
          <div class="md:w-1/2 p-4">
            <div class="flex justify-center items-center">
              <img 
                :src="selectedVariation.imageUrls[selectedImageIndex] || selectedVariation.imageUrls[0]" 
                :alt="product.nome" 
                class="w-full h-auto max-h-[500px] object-contain rounded-lg"
              >
            </div>
            <div v-if="selectedVariation.imageUrls && selectedVariation.imageUrls.length > 1" class="mt-4 flex flex-wrap gap-2 justify-center">
              <button
                v-for="(img, idx) in selectedVariation.imageUrls"
                :key="idx"
                @click="selectedImageIndex = idx"
                class="border rounded-md overflow-hidden"
                :class="selectedImageIndex === idx ? 'border-blue-500' : 'border-gray-600'"
                style="width: 64px; height: 64px;"
              >
                <img :src="img" :alt="product.nome" class="w-full h-full object-cover">
              </button>
            </div>
          </div>

          <div class="p-8 md:w-1/2 flex flex-col justify-center">
            <h1 class="text-4xl font-bold mb-2">{{ product.nome }}</h1>
            <p class="text-gray-400 text-lg mb-4">{{ product.categoria }}</p>
            
            <p class="text-3xl font-light text-blue-400 mb-4">
              R$ {{ selectedVariation.preco.toFixed(2) }}
            </p>
            <p class="text-sm text-gray-400 mb-2">Estoque: {{ selectedVariation.estoque }}</p>
            <p v-if="selectedVariation.estoque <= 0" class="text-red-400 text-sm mb-4">Sem estoque</p>
            
            <p v-if="product.descricao" class="text-gray-300 mb-6 text-sm leading-relaxed">
              {{ product.descricao }}
            </p>

            <div class="mb-6" v-if="availableColors.length > 0">
              <h3 class="text-lg font-semibold mb-2">Cor: <span class="font-normal capitalize">{{ selectedColorName }}</span></h3>
              <div class="flex space-x-2">
                <button 
                  v-for="colorHex in availableColors" :key="colorHex" 
                  @click="selectColor(colorHex)"
                  class="w-8 h-8 rounded-full border-2 transition"
                  :style="{ backgroundColor: normalizeColor(colorHex) }"
                  :class="{'border-blue-400 ring-2 ring-blue-400': selectedColor === colorHex, 'border-gray-600': selectedColor !== colorHex}">
                </button>
              </div>
            </div>

            <div class="mb-6" v-if="availableSizes.length > 0">
              <h3 class="text-lg font-semibold mb-2">Tamanho:</h3>
              <div class="flex flex-wrap gap-2">
                <button 
                  v-for="size in availableSizes" :key="size" 
                  @click="selectedSize = size"
                  class="px-4 py-2 border rounded-md transition"
                  :class="{'bg-blue-600 border-blue-600 text-white': selectedSize === size, 'border-gray-600 hover:bg-gray-700': selectedSize !== size}">
                  {{ size }}
                </button>
              </div>
            </div>

            <button @click="handleAddToCart" :disabled="selectedVariation.estoque <= 0" class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300 disabled:opacity-50 disabled:cursor-not-allowed">
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

<style scoped>
/* Seu CSS original (não mudei nada) */
select.appearance-none {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}
</style>
