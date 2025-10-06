<script setup>
import { RouterView } from 'vue-router'
import Header from './components/Header.vue';
</script>

<template>
  <div class="min-h-screen bg-[#0B1120] text-gray-200 font-sans">
    <Header />
    <main>
      <RouterView />
    </main>
  </div>
</template>