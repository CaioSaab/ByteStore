<script setup>
import { store } from '@/store/index.js';
</script>

<template>
  <aside class="w-full lg:w-1/4 xl:w-1/5 bg-[#1a1a2e] p-6 rounded-lg shadow-lg self-start">
    <h3 class="text-xl font-semibold mb-4 border-b border-gray-700 pb-2">Filtros</h3>
    <div>
      <h4 class="text-lg font-semibold mb-3">Categorias</h4>
      <ul class="space-y-2">
        <li>
          <a @click.prevent="store.setCategory('Todos')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Todos' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Todos</a>
        </li>
        <li>
          <a @click.prevent="store.setCategory('Eletrônicos')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Eletrônicos' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Eletrônicos</a>
        </li>
        <li>
          <a @click.prevent="store.setCategory('Roupas')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Roupas' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Roupas</a>
        </li>
        <li>
          <a @click.prevent="store.setCategory('Casa')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Casa' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Casa</a>
        </li>
         <li>
          <a @click.prevent="store.setCategory('Livros')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Livros' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Livros</a>
        </li>
        <li>
          <a @click.prevent="store.setCategory('Esportes e Lazer')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Esportes e Lazer' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Esportes e Lazer</a>
        </li>
        <li>
          <a @click.prevent="store.setCategory('Beleza e Cuidados')" href="#" class="block px-4 py-2 rounded transition-colors duration-200" :class="store.selectedCategory === 'Beleza e Cuidados' ? 'bg-blue-600 text-white' : 'hover:bg-gray-700'">Beleza e Cuidados</a>
        </li>
      </ul>
    </div>
  </aside>
</template>