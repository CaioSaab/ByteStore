<script setup>
import { computed, ref, watch, onMounted } from 'vue';
import FilterSidebar from '@/components/FilterSidebar.vue';
import ProductCard from '@/components/ProductCard.vue';
import Pagination from '@/components/Pagination.vue';
import { store } from '@/store/index.js';

const currentPage = ref(1);
const itemsPerPage = ref(6);

const allProducts = computed(() => store.products || []);

onMounted(async () => {
  if (store.products.length === 0) {
    await store.loadProducts();
  }
});

watch(() => store.selectedCategory, async () => {
  await store.loadProducts();
  currentPage.value = 1;
});

watch(() => store.searchTerm, async () => {
  await store.searchProducts();
  currentPage.value = 1;
});

const fullyFilteredProducts = computed(() => {
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

const currentHeading = computed(() => store.selectedCategory === 'Todos' ? 'Todos os Produtos' : store.selectedCategory);

watch(fullyFilteredProducts, () => {
  currentPage.value = 1;
});

const totalPages = computed(() => {
  return Math.ceil(fullyFilteredProducts.value.length / itemsPerPage.value);
});

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return fullyFilteredProducts.value.slice(start, end);
});

const handlePageChange = (newPage) => {
  currentPage.value = newPage;
};
</script>

<template>
  <div class="container mx-auto flex flex-col lg:flex-row gap-8">
    <FilterSidebar />
    <div class="w-full">
      <h2 class="text-2xl font-bold mb-6 text-white">{{ currentHeading }}</h2>
      <div v-if="paginatedProducts.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 min-h-[500px]">
        <ProductCard v-for="product in paginatedProducts" :key="product.id" :product="product" />
      </div>
      <div v-else class="text-center text-gray-400 mt-10">
        <p class="text-2xl">Nenhum produto encontrado.</p>
        <p>Tente ajustar sua busca ou filtros.</p>
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