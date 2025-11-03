<script setup>
import { RouterView } from 'vue-router';
import Header from '@/components/Header.vue';
</script>

<template>
  <div>
    <Header />
    <main class="p-4 sm:p-6 lg:p-8">
      <RouterView />
    </main>
  </div>
</template>