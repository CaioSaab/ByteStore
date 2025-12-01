<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { RouterLink } from 'vue-router';
import { store } from '@/store/index.js';

const featuredProducts = ref([]);
const currentSlide = ref(0);
const isLoading = ref(false);
const itemsPerView = ref(1);

const updateItemsPerView = () => {
  itemsPerView.value = 1;
};

const totalSlides = computed(() => {
  if (featuredProducts.value.length === 0) return 0;
  return Math.ceil(featuredProducts.value.length / itemsPerView.value);
});

const visibleProducts = computed(() => {
  const start = currentSlide.value * itemsPerView.value;
  const end = start + itemsPerView.value;
  return featuredProducts.value.slice(start, end);
});

const canGoNext = computed(() => {
  return currentSlide.value < totalSlides.value - 1;
});

const canGoPrev = computed(() => {
  return currentSlide.value > 0;
});

const computeFeaturedFromStore = () => {
  const products = Array.isArray(store.products) ? store.products : [];
  const byCategory = new Map();
  for (const p of products) {
    const cat = p.category;
    const prices = Array.isArray(p.variations) ? p.variations.map(v => Number(v.preco) || 0) : [];
    const maxPrice = prices.length > 0 ? Math.max(...prices) : (Number(p.price) || 0);
    const current = byCategory.get(cat);
    if (!current || maxPrice > current.maxPrice) {
      byCategory.set(cat, { product: p, maxPrice });
    }
  }
  featuredProducts.value = Array.from(byCategory.values()).map(x => x.product);
};

const nextSlide = () => {
  if (canGoNext.value) {
    currentSlide.value++;
  } else {
    currentSlide.value = 0; // Volta ao início
  }
};

const prevSlide = () => {
  if (canGoPrev.value) {
    currentSlide.value--;
  } else {
    currentSlide.value = totalSlides.value - 1; // Vai para o final
  }
};

const goToSlide = (slideIndex) => {
  if (slideIndex >= 0 && slideIndex < totalSlides.value) {
    currentSlide.value = slideIndex;
  }
};

onMounted(() => {
  updateItemsPerView();
  window.addEventListener('resize', updateItemsPerView);
  isLoading.value = true;
  computeFeaturedFromStore();
  isLoading.value = false;
});

watch(() => store.products, () => {
  computeFeaturedFromStore();
});
</script>

<template>
  <div class="mb-12 relative">
    <div class="flex items-center justify-between mb-6">
      <h2 class="text-3xl font-bold text-white">⭐ Produtos em Destaque</h2>
    </div>

    <div v-if="isLoading" class="flex justify-center items-center py-20">
      <div class="text-gray-400 text-xl">Carregando produtos em destaque...</div>
    </div>

    <div v-else-if="featuredProducts.length === 0" class="text-center py-20 text-gray-400">
      <p class="text-xl">Nenhum produto em destaque no momento.</p>
    </div>

    <div v-else class="relative">
      <!-- Botões de navegação -->
      <button
        v-if="totalSlides > 1"
        @click="prevSlide"
        class="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-4 z-10 bg-[#1a1a2e] text-white p-3 rounded-full shadow-lg hover:bg-[#2a2a3e] transition-all"
        aria-label="Slide anterior"
      >
        <i class="fas fa-chevron-left"></i>
      </button>

      <button
        v-if="totalSlides > 1"
        @click="nextSlide"
        class="absolute right-0 top-1/2 -translate-y-1/2 translate-x-4 z-10 bg-[#1a1a2e] text-white p-3 rounded-full shadow-lg hover:bg-[#2a2a3e] transition-all"
        aria-label="Próximo slide"
      >
        <i class="fas fa-chevron-right"></i>
      </button>

      <!-- Container do carrossel -->
      <div class="overflow-hidden mx-8">
        <div
          class="flex transition-transform duration-500 ease-in-out"
          :style="{ transform: `translateX(-${currentSlide * 100}%)` }"
        >
          <div
            v-for="product in featuredProducts"
            :key="product.id"
            class="flex-shrink-0 px-3"
            :style="{ width: `${100 / itemsPerView}%` }"
          >
            <RouterLink
              :to="{ name: 'product-detail', params: { id: product.id } }"
              class="block rounded-lg overflow-hidden border border-gray-700 hover:border-blue-500 transition-colors"
              :aria-label="`Abrir produto ${product.name}`"
            >
              <img
                :src="product.image"
                alt=""
                class="w-full h-72 sm:h-80 object-cover"
              />
            </RouterLink>
          </div>
        </div>
      </div>

      <!-- Indicadores (dots) -->
      <div v-if="totalSlides > 1" class="flex justify-center mt-6 gap-2">
        <button
          v-for="(slide, index) in totalSlides"
          :key="index"
          @click="goToSlide(index)"
          class="h-3 rounded-full transition-all"
          :class="index === currentSlide ? 'bg-blue-500 w-8' : 'bg-gray-600 hover:bg-gray-500 w-3'"
          :aria-label="`Ir para slide ${index + 1}`"
        ></button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.transition-transform {
  transition-property: transform;
}
</style>
