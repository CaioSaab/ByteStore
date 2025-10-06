<script setup>
import { computed, ref, watch } from 'vue';
import FilterSidebar from '@/components/FilterSidebar.vue';
import ProductCard from '@/components/ProductCard.vue';
import Pagination from '@/components/Pagination.vue';
import { products } from '@/data/products.js';
import { store } from '@/store/index.js';

const allProducts = ref(products);
const currentPage = ref(1);
const itemsPerPage = ref(6);

const fullyFilteredProducts = computed(() => {
  // A lógica de filtro que já funciona continua aqui
  const productsByCategory = store.selectedCategory === 'Todos'
    ? allProducts.value
    : allProducts.value.filter(p => p.category === store.selectedCategory);
  
  if (!store.searchTerm.trim()) {
    return productsByCategory;
  }
  
  const lowerCaseSearch = store.searchTerm.toLowerCase().trim();
  return productsByCategory.filter(product => 
    product.name.toLowerCase().includes(lowerCaseSearch)
  );
});

const totalPages = computed(() => {
  return Math.ceil(fullyFilteredProducts.value.length / itemsPerPage.value);
});

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return fullyFilteredProducts.value.slice(start, end);
});

// Função que escuta o evento do componente de paginação
const handlePageChange = (newPage) => {
  currentPage.value = newPage;
};

// Reseta para a primeira página sempre que um filtro é alterado
watch(fullyFilteredProducts, () => {
  currentPage.value = 1;
});
</script>

<template>
  <div class="container mx-auto flex flex-col lg:flex-row gap-8">
    <FilterSidebar />
    <div class="w-full">
      <h2 class="text-2xl font-bold mb-6 text-white">PRODUTOS</h2>
      
      <div v-if="paginatedProducts.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 min-h-[500px]">
        <ProductCard v-for="product in paginatedProducts" :key="product.id" :product="product" />
      </div>
      
      <div v-else class="text-center text-gray-400 mt-10">
        <p class="text-2xl">Nenhum produto encontrado.</p>
        <p>Tente ajustar seu pesquisar ou seus filtros.</p>
      </div>

      <div class="mt-10 flex justify-center">
        <Pagination
          :currentPage="currentPage"
          :totalPages="totalPages"
          @page-changed="handlePageChange"
        />
      </div>
    </div>
  </div>
</template>