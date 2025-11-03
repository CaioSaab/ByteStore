<script setup>
import { ref } from 'vue';
import { store } from '@/store/index.js';
import { useRouter } from 'vue-router';

const router = useRouter();

// --- NOVO: Lista predefinida de categorias ---
// Deve corresponder EXATAMENTE às categorias usadas no FilterSidebar e nos produtos
const availableCategories = [
  'Eletrônicos',
  'Roupas',
  'Casa',
  'Livros',
  'Esportes e Lazer',
  'Beleza e Cuidados'
];

// Estado do formulário
const name = ref('');
const category = ref('');
const price = ref(null);
const stock = ref(0);
const description = ref('');
const images = ref([]);
const imagePreviews = ref([]);
const colors = ref([]);
const newColor = ref('#000000');
const sizes = ref([]);
const newSize = ref('');

// Funções handleImageUpload, addColor, removeColor, addSize, removeSize (sem alterações)
const handleImageUpload = (event) => {
  const files = Array.from(event.target.files);
  if (files.length === 0) return;
  
  images.value = files;
  imagePreviews.value = files.map(file => URL.createObjectURL(file));
};
function addColor() {
  if (newColor.value && !colors.value.includes(newColor.value)) {
    colors.value.push(newColor.value);
  }
}
function removeColor(colorToRemove) {
  colors.value = colors.value.filter(color => color !== colorToRemove);
}
function addSize() {
  const sizeToAdd = newSize.value.trim().toUpperCase();
  if (sizeToAdd && !sizes.value.includes(sizeToAdd)) {
    sizes.value.push(sizeToAdd);
    newSize.value = '';
  }
}
function removeSize(sizeToRemove) {
  sizes.value = sizes.value.filter(size => size !== sizeToRemove);
}

const validateForm = () => {
  if (!name.value || !category.value || !price.value || images.value.length === 0) {
    alert("Erro: Preencha todos os campos obrigatórios e adicione pelo menos uma imagem.");
    return false;
  }
  return true;
};

const handleSubmit = async () => {
  if (!validateForm()) {
    return;
  }

  const productData = {
    name: name.value,
    category: category.value,
    price: parseFloat(price.value),
    stock: parseInt(stock.value) || 0,
    description: description.value,
    images: images.value,
    colors: colors.value,
    sizes: sizes.value,
  };

  const result = await store.addProduct(productData);
  
  if (result.success) {
    alert('Produto adicionado com sucesso!');
    router.push({ name: 'vendor-dashboard' });
  } else {
    alert(result.error || 'Erro ao adicionar produto');
  }
};
</script>

<template>
  <div class="container mx-auto text-white">
    <h1 class="text-3xl font-bold mb-8">Adicionar Novo Anúncio</h1>

    <form @submit.prevent="handleSubmit" class="max-w-2xl mx-auto space-y-6 bg-[#1a1a2e] p-8 rounded-lg">

      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Nome do Produto *</label>
        <input v-model="name" type="text"
               class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
               required>
      </div>

      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Categoria *</label>
        <select v-model="category"
                class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition appearance-none"
                required>
            <option disabled value="">-- Selecione uma categoria --</option>
            <option v-for="cat in availableCategories" :key="cat" :value="cat">
                {{ cat }}
            </option>
        </select>
      </div>
      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Descrição</label>
        <textarea v-model="description"
                  class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                  rows="4"></textarea>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-gray-300 text-sm font-bold mb-2">Preço (R$) *</label>
          <input v-model="price" type="number" step="0.01" min="0"
                 class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                 required>
        </div>
        <div>
          <label class="block text-gray-300 text-sm font-bold mb-2">Estoque (Quantidade) *</label>
          <input v-model="stock" type="number" min="0"
                 class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
                 required>
        </div>
      </div>

      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Imagens (selecione uma ou mais) *</label>
        <input
          type="file"
          multiple
          accept="image/*"
          @change="handleImageUpload"
          class="block w-full text-sm text-gray-400 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-sm file:font-semibold file:bg-blue-600 file:text-white hover:file:bg-blue-700 cursor-pointer"
          required
        >
        <div v-if="imagePreviews.length > 0" class="mt-4 flex flex-wrap gap-4">
          <img v-for="(preview, index) in imagePreviews" :key="index" :src="preview" class="w-24 h-24 object-cover rounded-lg border border-gray-600">
        </div>
      </div>

      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Cores Disponíveis</label>
        <div class="flex items-center gap-2 mb-2">
          <input v-model="newColor" type="color" class="p-1 h-10 w-14 block bg-gray-700 border border-gray-600 cursor-pointer rounded-lg">
          <button @click.prevent="addColor" class="btn-secondary">Adicionar Cor</button>
        </div>
        <div class="flex flex-wrap gap-2">
          <span v-for="color in colors" :key="color" class="flex items-center gap-1 text-xs px-2 py-1 rounded-full border border-gray-600">
            <span class="block w-3 h-3 rounded-full border border-gray-500" :style="{ backgroundColor: color }"></span>
            {{ color }}
            <button @click.prevent="removeColor(color)" class="ml-1 text-red-500 hover:text-red-400">&times;</button>
          </span>
        </div>
      </div>

      <div>
        <label class="block text-gray-300 text-sm font-bold mb-2">Tamanhos Disponíveis</label>
        <div class="flex items-center gap-2 mb-2">
           <input v-model="newSize" type="text" placeholder="Ex: P, M, 42"
                  class="w-full bg-[#0e101f] border border-gray-700 rounded-lg py-2 px-3 text-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition flex-grow">
          <button @click.prevent="addSize" class="btn-secondary">Adicionar Tamanho</button>
        </div>
        <div class="flex flex-wrap gap-2">
          <span v-for="size in sizes" :key="size" class="flex items-center text-xs px-2 py-1 rounded-full border border-gray-600 bg-gray-700">
            {{ size }}
            <button @click.prevent="removeSize(size)" class="ml-2 text-red-500 hover:text-red-400">&times;</button>
          </span>
        </div>
      </div>

      <button type="submit" class="w-full bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-4 rounded-lg text-lg">
        Publicar Anúncio
      </button>
    </form>
  </div>
</template>

<style scoped>
select.appearance-none {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}
</style>