<script setup>
import { ref } from 'vue';
import { store } from '@/store/index.js';

const emit = defineEmits(['close', 'product-added']);

const availableCategories = [
  'Eletrônicos', 'Roupas', 'Casa', 'Livros', 'Esportes e Lazer', 'Beleza e Cuidados'
];

const isLoading = ref(false);
const name = ref('');
const category = ref('');
const description = ref('');
const variations = ref([]);
const isSingleSize = ref(false);

// Setup da nova variação (Sem corNome)
const newVar = ref({
  id: null, 
  cor: '#000000', // Padrão
  tamanho: '',
  preco: null,
  estoque: 0,
  images: [],
  imagePreviews: [] 
});

const handleNewVarImages = (event) => {
  const files = Array.from(event.target.files);
  if (files.length === 0) return;
  newVar.value.images = files;
  newVar.value.imagePreviews = files.map(file => URL.createObjectURL(file));
};

const addVariation = () => {
  if (!newVar.value.preco || newVar.value.estoque < 0 || newVar.value.images.length === 0) {
    alert('Preencha Preço, Estoque e adicione Imagens.');
    return;
  }
  if (!isSingleSize.value && (!newVar.value.tamanho || !newVar.value.tamanho.toString().trim())) {
    alert('Informe o tamanho ou selecione Tamanho Único.');
    return;
  }

  const tamanho = isSingleSize.value ? 'U' : newVar.value.tamanho;
  const varToPush = { ...newVar.value, tamanho, id: Date.now() };
  variations.value.push(varToPush);

  newVar.value = { id: null, cor: '#000000', tamanho: '', preco: null, estoque: 0, images: [], imagePreviews: [] };
  isSingleSize.value = false;
};

const removeVariation = (id) => {
  variations.value = variations.value.filter(v => v.id !== id);
};

const handleSubmit = async () => {
  if (!name.value || !category.value) {
    alert("Erro: Preencha Nome e Categoria.");
    return;
  }
  if (variations.value.length === 0) {
    alert("Erro: Adicione pelo menos uma variação.");
    return;
  }

  isLoading.value = true;
  const productData = {
    name: name.value,
    category: category.value,
    description: description.value,
    variations: variations.value, 
  };

  try {
    const result = await store.addProduct(productData); 
    if (result.success) {
      alert('Produto adicionado!');
      emit('product-added'); 
      emit('close'); 
    } else {
      alert(result.error || 'Erro ao salvar');
    }
  } catch (error) {
    console.error(error);
    alert('Erro interno.');
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm p-4" @click.self="$emit('close')">
    <div class="bg-[#1a1a2e] w-full max-w-4xl max-h-[90vh] overflow-y-auto rounded-xl shadow-2xl border border-gray-700 relative flex flex-col">
      
      <div class="sticky top-0 bg-[#1a1a2e] z-10 px-6 py-4 border-b border-gray-700 flex justify-between items-center">
        <h2 class="text-2xl font-bold text-white">Novo Anúncio</h2>
        <button @click="$emit('close')" class="text-gray-400 hover:text-white"><i class="fas fa-times text-2xl"></i></button>
      </div>

      <div class="p-6 space-y-6">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          
          <div class="space-y-4">
            <h3 class="text-xl font-semibold text-blue-300 border-b border-gray-700 pb-2">1. Dados Básicos</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div><label class="label-style">Nome *</label><input v-model="name" type="text" class="input-style" required></div>
              <div>
                <label class="label-style">Categoria *</label>
                <select v-model="category" class="input-style select-style" required>
                  <option disabled value="">Selecione...</option>
                  <option v-for="cat in availableCategories" :key="cat" :value="cat">{{ cat }}</option>
                </select>
              </div>
            </div>
            <div><label class="label-style">Descrição</label><textarea v-model="description" class="input-style" rows="3"></textarea></div>
          </div>

          <div class="space-y-4">
            <h3 class="text-xl font-semibold text-blue-300 border-b border-gray-700 pb-2">2. Variações</h3>
            <div v-if="variations.length === 0" class="text-gray-400 text-sm italic">Nenhuma variação adicionada.</div>
            
            <div v-for="v in variations" :key="v.id" class="bg-[#0e101f] p-3 rounded border border-gray-600 flex justify-between items-center">
              <div class="flex items-center gap-4">
                <div class="w-8 h-8 rounded-full shadow-sm border border-gray-500" :style="{ backgroundColor: v.cor }"></div>
                <div>
                  <p class="font-bold text-white uppercase">{{ v.cor }}</p>
                  <p class="text-sm text-gray-400">Tam: {{ v.tamanho || 'U' }} | R$ {{ v.preco }} | Qtd: {{ v.estoque }}</p>
                </div>
              </div>
              <button @click.prevent="removeVariation(v.id)" class="text-red-400 font-bold text-sm hover:text-red-300">Remover</button>
            </div>
          </div>

          <div class="bg-[#0e101f] p-6 rounded-lg border border-dashed border-blue-500/50 space-y-4">
            <h4 class="text-lg font-medium text-white">Nova Variação</h4>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div><label class="label-style">Preço (R$) *</label><input v-model="newVar.preco" type="number" step="0.01" class="input-style"></div>
              <div><label class="label-style">Estoque *</label><input v-model="newVar.estoque" type="number" class="input-style"></div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 items-end">
              
              <div>
                <label class="label-style">Cor</label>
                <div class="relative flex items-center gap-3 bg-[#1f2937] border border-gray-600 rounded-lg p-2 cursor-pointer hover:border-blue-500 transition-colors group h-[42px]">
                  
                  <div class="w-6 h-6 rounded-full border border-gray-500 shadow-sm flex-shrink-0" 
                       :style="{ backgroundColor: newVar.cor }">
                  </div>
                  
                  <div class="flex-1 font-mono text-white text-sm tracking-widest uppercase">
                    {{ newVar.cor }}
                  </div>
                  
                  <div class="text-gray-400 pr-2 group-hover:text-blue-400">
                    <i class="fas fa-palette"></i>
                  </div>

                  <input 
                    type="color" 
                    v-model="newVar.cor" 
                    class="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10"
                    title="Clique para escolher a cor"
                  >
                </div>
              </div>

              <div>
                <label class="label-style">Tamanho *</label>
                <input v-model="newVar.tamanho" type="text" :placeholder="isSingleSize ? 'Único (U)' : 'Ex: P, M, 42'" class="input-style" :disabled="isSingleSize">
                <label class="inline-flex items-center gap-2 text-gray-300 mt-2">
                  <input v-model="isSingleSize" type="checkbox" class="rounded" />
                  Tamanho Único
                </label>
              </div>
            </div>
            
            <div>
              <label class="label-style">Imagens *</label>
              <input type="file" multiple accept="image/*" @change="handleNewVarImages" class="input-file-style">
              <div v-if="newVar.imagePreviews.length" class="mt-2 flex flex-wrap gap-2">
                <img v-for="(img, i) in newVar.imagePreviews" :key="i" :src="img" class="w-16 h-16 rounded object-cover border border-gray-600">
              </div>
            </div>

            <button @click.prevent="addVariation" class="w-full bg-blue-600/20 hover:bg-blue-600/40 text-blue-300 py-2 rounded border border-blue-500 transition-colors">
              + Adicionar Variação
            </button>
          </div>

          <div class="flex justify-end gap-4 pt-4 border-t border-gray-700 sticky bottom-0 bg-[#1a1a2e] pb-2">
            <button type="button" @click="$emit('close')" class="px-6 py-2 rounded text-gray-300 hover:bg-white/10">Cancelar</button>
            <button type="submit" :disabled="isLoading" class="bg-green-600 hover:bg-green-700 text-white font-bold py-2 px-8 rounded shadow-lg disabled:opacity-50">
              {{ isLoading ? 'Enviando...' : 'Finalizar Anúncio' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.label-style { display: block; color: #d1d5db; font-size: 0.875rem; font-weight: 700; margin-bottom: 0.25rem; }
.input-style { width: 100%; background-color: #1f2937; border: 1px solid #4b5563; border-radius: 0.5rem; padding: 0.5rem 0.75rem; color: white; height: 42px; /* Altura fixa para alinhar com o botão de cor */ }
.input-style:focus { outline: none; border-color: #3b82f6; box-shadow: 0 0 0 1px #3b82f6; }
.select-style { appearance: none; background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e"); background-position: right 0.5rem center; background-repeat: no-repeat; background-size: 1.5em 1.5em; padding-right: 2.5rem; }
.input-file-style { display: block; width: 100%; font-size: 0.875rem; color: #9ca3af; }
.input-file-style::file-selector-button { margin-right: 1rem; padding: 0.5rem 1rem; border-radius: 0.5rem; border: 0; font-size: 0.875rem; font-weight: 600; background-color: #2563eb; color: white; cursor: pointer; }
.input-file-style::file-selector-button:hover { background-color: #1d4ed8; }
</style>