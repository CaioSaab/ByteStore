<script setup>
import { ref, onMounted } from 'vue';
import { RouterLink } from 'vue-router'; // Ainda pode ser usado para outros links se precisar
import { store } from '@/store/index.js';
import { getVendorProducts, toggleProductStatus, deleteProduct, updateProduct, getProductById, updateVariationStock } from '@/services/productService';

// IMPORTAÇÃO DO NOVO MODAL
import VendorProductModal from '@/components/VendorProductModal.vue';

const vendorProducts = ref([]);
const isLoading = ref(false);
const errorMessage = ref('');
const editingProductId = ref(null);
const editDraft = ref(null);
const editingLoading = ref(false);

// ESTADO PARA CONTROLAR A VISIBILIDADE DO MODAL
const showAddModal = ref(false);

const loadVendorProducts = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    const result = await getVendorProducts();
    if (result.success) {
      vendorProducts.value = result.data.products || result.data || [];
    } else {
      errorMessage.value = result.error || 'Erro ao carregar produtos';
    }
  } catch (error) {
    errorMessage.value = 'Erro ao carregar produtos do vendedor';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

// Callback executado quando o modal avisa que um produto foi adicionado
const handleProductAdded = () => {
  loadVendorProducts(); // Recarrega a lista
};

const handleToggleStatus = async (product) => {
  const newStatus = !product.active;
  try {
    const result = await toggleProductStatus(product.id, newStatus);
    if (result.success) {
      // Atualiza localmente para feedback instantâneo
      const p = vendorProducts.value.find(p => p.id === product.id);
      if (p) p.active = newStatus;
    } else {
      errorMessage.value = result.error || 'Erro ao alterar status do produto';
      setTimeout(() => { errorMessage.value = ''; }, 5000);
    }
  } catch (error) {
    errorMessage.value = 'Erro ao alterar status do produto';
    console.error(error);
    setTimeout(() => { errorMessage.value = ''; }, 5000);
  }
};

const handleDeleteProduct = async (product) => {
  if (!confirm(`Tem certeza que deseja excluir o produto "${product.name}"? Esta ação é irreversível.`)) {
    return;
  }

  isLoading.value = true;
  try {
    const result = await deleteProduct(product.id);
    if (result.success) {
      vendorProducts.value = vendorProducts.value.filter(p => p.id !== product.id);
    } else {
      errorMessage.value = result.error || 'Erro ao excluir produto';
    }
  } catch (error) {
    errorMessage.value = 'Erro ao excluir produto';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  loadVendorProducts();
});

const startEdit = (product) => {
  editingProductId.value = product.id;
  editingLoading.value = true;
  const buildDraft = (p) => {
    editDraft.value = JSON.parse(JSON.stringify({
      id: p.id,
      name: p.name,
      description: p.description || '',
      category: p.category,
      variations: Array.isArray(p.variations) ? p.variations.map(v => ({
        id: v.id,
        preco: Number(v.preco) || 0,
        estoque: Number(v.estoque) || 0,
        cor: v.cor || '#000000',
        tamanho: v.tamanho || 'U',
        imageUrls: v.imageUrls || v.images || []
      })) : []
    }));
  };
  if (!Array.isArray(product.variations) || product.variations.length === 0) {
    getProductById(product.id).then((res) => {
      if (res.success && res.data) {
        const data = res.data;
        const vars = Array.isArray(data.variations) ? data.variations : [];
        product.variations = vars.map(v => ({
          id: v.id,
          preco: Number(v.preco) || 0,
          estoque: Number(v.estoque) || 0,
          cor: v.cor || '#000000',
          tamanho: v.tamanho || 'U',
          imageUrls: v.imageUrls || v.images || []
        }));
      }
      buildDraft(product);
      editingLoading.value = false;
    }).catch(() => {
      buildDraft(product);
      editingLoading.value = false;
    });
  } else {
    buildDraft(product);
    editingLoading.value = false;
  }
};

const cancelEdit = () => {
  editingProductId.value = null;
  editDraft.value = null;
};

const saveEdit = async () => {
  if (!editDraft.value) return;
  isLoading.value = true;
  errorMessage.value = '';
  try {
    const productId = editDraft.value.id;
    const current = vendorProducts.value.find(p => p.id === productId) || {};
    const currentVars = Array.isArray(current.variations) ? current.variations : [];
    for (const v of (editDraft.value.variations || [])) {
      const old = currentVars.find(cv => cv.id === v.id);
      const oldQty = Number(old?.estoque ?? v.estoque);
      if (Number(v.estoque) !== Number(oldQty)) {
        const res = await updateVariationStock(v.id, Number(v.estoque), productId);
        if (!res.success) {
          throw new Error(res.error || 'Erro ao atualizar estoque');
        }
      }
    }
    const idx = vendorProducts.value.findIndex(p => p.id === productId);
    if (idx !== -1) {
      const updated = { ...vendorProducts.value[idx] };
      updated.variations = (updated.variations || []).map(v => {
        const nv = (editDraft.value.variations || []).find(ev => ev.id === v.id);
        return nv ? { ...v, estoque: nv.estoque } : v;
      });
      vendorProducts.value[idx] = updated;
    }
    cancelEdit();
  } catch (e) {
    errorMessage.value = e.message || 'Erro ao atualizar produto';
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="container mx-auto relative">
    <h1 class="text-3xl font-bold mb-4 text-white">Painel do Vendedor</h1>
    <p class="mb-8 text-gray-400">Bem-vindo(a), <span class="text-cyan-400">{{ store.vendor?.email }}</span>!</p>
    
    <div class="mb-6">
      <button 
        @click="showAddModal = true"
        class="bg-blue-600 text-white py-3 px-6 rounded-lg hover:bg-blue-700 text-lg font-semibold inline-flex items-center gap-2 transition-colors">
        <i class="fas fa-plus"></i>
        Adicionar Novo Anúncio
      </button>
    </div>

    <div v-if="errorMessage" class="mb-6 p-4 bg-red-900/50 border border-red-500 rounded-lg text-red-300">
      {{ errorMessage }}
    </div>

    <div class="mt-8">
      <h2 class="text-2xl font-bold mb-6 text-white">Gerenciar Anúncios</h2>
      
      <div v-if="isLoading && vendorProducts.length === 0" class="text-center py-20 text-gray-400">
        <div class="text-xl">Carregando seus anúncios...</div>
      </div>

      <div v-else-if="vendorProducts.length === 0" class="text-center py-20 text-gray-400 bg-[#1a1a2e] rounded-lg">
        <p class="text-xl mb-4">Você ainda não possui anúncios cadastrados.</p>
        <button @click="showAddModal = true" class="text-blue-400 hover:text-blue-300 underline">
          Criar seu primeiro anúncio
        </button>
      </div>

      <div v-else class="space-y-4">
        <div
          v-for="product in vendorProducts"
          :key="product.id"
          class="bg-[#1a1a2e] rounded-lg p-6 shadow-lg hover:shadow-xl transition-shadow">
          <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div class="flex-1 flex items-center gap-4">
              <img v-if="product.image || (product.variations && product.variations[0]?.images[0])" 
                   :src="product.image || product.variations[0].images[0]" 
                   :alt="product.name" 
                   class="w-20 h-20 object-cover rounded-lg bg-gray-800">
              
              <div class="flex-1">
                <h3 class="text-xl font-semibold text-white mb-1">{{ product.name }}</h3>
                <p class="text-gray-400 text-sm mb-2">{{ product.category }}</p>
                <p class="text-blue-400 font-bold">
                  R$ {{ (product.price || (product.variations && product.variations[0]?.preco) || 0).toFixed(2) }}
                </p>
              </div>
            </div>

            <div class="flex flex-col sm:flex-row items-stretch sm:items-center gap-2">
              <div class="flex items-center justify-center gap-2">
                <span
                  class="px-4 py-2 rounded-full text-sm font-semibold w-full sm:w-auto"
                  :class="product.active 
                    ? 'bg-green-900/50 text-green-300 border border-green-500' 
                    : 'bg-red-900/50 text-red-300 border border-red-500'">
                  <i :class="product.active ? 'fas fa-check-circle' : 'fas fa-times-circle'" class="mr-2"></i>
                  {{ product.active ? 'Ativo' : 'Inativo' }}
                </span>
              </div>

              <button
                @click="handleToggleStatus(product)"
                :disabled="isLoading"
                class="px-6 py-2 rounded-lg font-semibold transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                :class="product.active
                  ? 'bg-red-600 hover:bg-red-700 text-white'
                  : 'bg-green-600 hover:bg-green-700 text-white'">
                <i :class="product.active ? 'fas fa-ban' : 'fas fa-check'" class="mr-2"></i>
                {{ product.active ? 'Desativar' : 'Ativar' }}
              </button>
              
              <button
                @click="handleDeleteProduct(product)"
                :disabled="isLoading"
                class="px-6 py-2 rounded-lg font-semibold transition-all disabled:opacity-50 disabled:cursor-not-allowed bg-gray-600 hover:bg-gray-700 text-white">
                <i class="fas fa-trash mr-2"></i>
                Excluir
              </button>

              <button
                @click="startEdit(product)"
                :disabled="isLoading"
                class="px-6 py-2 rounded-lg font-semibold transition-all disabled:opacity-50 disabled:cursor-not-allowed bg-yellow-600 hover:bg-yellow-700 text-white">
                <i class="fas fa-edit mr-2"></i>
                Editar
              </button>
            </div>
          </div>
          <div class="mt-4 pt-4 border-t border-gray-700">
            <p v-if="product.description" class="text-gray-400 text-sm mb-4">{{ product.description }}</p>
            <div class="bg-[#0e101f] rounded-lg p-4 border border-gray-700">
              <h4 class="text-white font-semibold mb-3">Variações</h4>
              <div class="space-y-2" v-if="editingProductId !== product.id">
                <div v-for="v in product.variations || []" :key="v.id" class="flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-5 h-5 rounded-full border border-gray-500" :style="{ backgroundColor: v.cor || '#000000' }"></div>
                    <span class="text-sm text-gray-300">Tam: {{ v.tamanho || 'U' }}</span>
                    <span class="text-sm text-gray-400">Estoque: <span class="text-white">{{ Number(v.estoque) || 0 }}</span></span>
                    <span class="text-sm text-blue-300">R$ {{ (Number(v.preco) || 0).toFixed(2) }}</span>
                  </div>
                </div>
                <div v-if="(!product.variations || product.variations.length === 0)" class="text-gray-400 text-sm">
                  <button @click="startEdit(product)" class="mt-2 bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded">Carregar variações</button>
                </div>
              </div>

              <div v-else class="space-y-3">
                <div v-if="editingLoading" class="text-gray-400">Carregando...</div>
                <div v-if="editDraft && editDraft.variations && editDraft.variations.length" class="space-y-2">
                  <div v-for="(v, i) in editDraft.variations" :key="v.id || i" class="grid grid-cols-1 sm:grid-cols-3 gap-3 items-center">
                    <div class="flex items-center gap-3">
                      <div class="w-5 h-5 rounded-full border border-gray-500" :style="{ backgroundColor: v.cor || '#000000' }"></div>
                      <span class="text-sm text-gray-300">{{ v.tamanho || 'U' }}</span>
                    </div>
                    <div>
                      <label class="block text-xs text-gray-400">Estoque</label>
                      <input v-model.number="v.estoque" type="number" class="w-full bg-[#1f2937] border border-gray-600 rounded p-2 text-white" />
                    </div>
                    <div class="text-sm text-gray-400">ID: {{ v.id }}</div>
                  </div>
                </div>
                <div v-else class="text-gray-400">Sem variações para editar.</div>
                <div class="flex justify-end gap-2 pt-2">
                  <button @click="cancelEdit" class="px-4 py-2 rounded bg-gray-700 text-white hover:bg-gray-600">Cancelar</button>
                  <button @click="saveEdit" :disabled="isLoading" class="px-4 py-2 rounded bg-green-600 text-white hover:bg-green-700 disabled:opacity-50">Salvar</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <VendorProductModal 
      v-if="showAddModal" 
      @close="showAddModal = false"
      @product-added="handleProductAdded"
    />

  </div>
</template>
