<script setup>
import { computed } from 'vue';

// Este componente recebe a página atual e o total de páginas como "props"
const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  }
});

// Este componente "emite" um evento para o pai quando um botão é clicado
const emit = defineEmits(['page-changed']);

// Uma função simples para criar um array de números [1, 2, 3, ...]
const pages = computed(() => {
  const pageArray = [];
  for (let i = 1; i <= props.totalPages; i++) {
    pageArray.push(i);
  }
  return pageArray;
});

// Função que avisa o componente pai sobre a mudança de página
const changePage = (newPage) => {
  if (newPage > 0 && newPage <= props.totalPages) {
    emit('page-changed', newPage);
  }
};
</script>

<template>
  <div v-if="totalPages > 1" class="flex items-center justify-center space-x-2 text-white">
    <button 
      @click="changePage(currentPage - 1)" 
      :disabled="currentPage === 1"
      class="px-4 py-2 rounded bg-gray-700 hover:bg-blue-600 disabled:bg-gray-800 disabled:text-gray-500 disabled:cursor-not-allowed transition-colors"
    >
      &laquo;
    </button>
    
    <button 
      v-for="page in pages" 
      :key="page" 
      @click="changePage(page)"
      class="px-4 py-2 rounded transition-colors"
      :class="page === currentPage ? 'bg-blue-600 text-white' : 'bg-gray-700 hover:bg-blue-500'"
    >
      {{ page }}
    </button>
    
    <button 
      @click="changePage(currentPage + 1)" 
      :disabled="currentPage === totalPages"
      class="px-4 py-2 rounded bg-gray-700 hover:bg-blue-600 disabled:bg-gray-800 disabled:text-gray-500 disabled:cursor-not-allowed transition-colors"
    >
      &raquo;
    </button>
  </div>
</template>