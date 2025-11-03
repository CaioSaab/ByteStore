<script setup>
import { ref } from 'vue';

// --- MAPA DE CORES COMPLETO (BASEADO NOS SEUS PRODUTOS) ---
const colorNameMap = {
  // Cores de Eletrônicos
  '#2c3e50': 'Grafite',
  '#ecf0f1': 'Prata Claro',
  '#3498db': 'Azul',
  '#ffffff': 'Branco',
  '#000000': 'Preto',
  '#7f8c8d': 'Cinza Médio',
  '#bdc3c7': 'Prata',
  '#f1c40f': 'Amarelo',
  '#e74c3c': 'Vermelho',
  'transparent': 'Transparente', // Cor dos Lentes AI
  // Cores de Roupas
  '#34495e': 'Azul Marinho',
  '#2980b9': 'Azul Royal',
  '#c0392b': 'Vinho',
  // Cores de Casa
  // '#c0392b': 'Vermelho' (Já mapeado)
  // '#000000': 'Preto' (Já mapeado)
  // '#ffffff': 'Branco' (Já mapeado)
  // '#bdc3c7': 'Prata' (Já mapeado)
  // '#e74c3c': 'Vermelho' (Já mapeado)
  // Cores de Esportes e Lazer
  '#9b59b6': 'Roxo',
  // '#3498db': 'Azul' (Já mapeado)
  '#1abc9c': 'Turquesa',
  // '#000000': 'Preto' (Já mapeado)
  // '#e74c3c': 'Vermelho' (Já mapeado)
  // '#2c3e50': 'Grafite' (Já mapeado)
  '#27ae60': 'Verde',
  '#f39c12': 'Dourado',
  // '#ffffff': 'Branco' (Já mapeado)
  // Cores de Beleza e Cuidados
  // '#c0392b': 'Vinho' (Já mapeado)
  '#8e44ad': 'Violeta',
  // '#f1c40f': 'Amarelo' (Já mapeado)
  // '#bdc3c7': 'Prata' (Já mapeado)
  // '#000000': 'Preto' (Já mapeado)
  'N/A': '' // Para casos onde a cor não se aplica
};

// Função auxiliar (sem alterações)
function getColorName(hexCode) {
  return colorNameMap[hexCode] || hexCode;
}

// Dados de Exemplo (sem alterações na estrutura)
const recentOrders = ref([
   { id: '#8734', customer: 'Juliana Costa', value: 280.00, status: 'Enviado', items: [{ name: 'Moletom com Capuz', quantity: 1, color: '#7f8c8d', size: 'M' }] },
   { id: '#8733', customer: 'Marcos Andrade', value: 1850.00, status: 'A Enviar', items: [{ name: 'Smartwatch Geração 5', quantity: 1, color: '#000000', size: '44mm' }] },
   { id: '#8732', customer: 'Beatriz Lima', value: 99.90, status: 'Enviado', items: [{ name: 'Camiseta Básica Premium', quantity: 1, color: '#ffffff', size: 'P' }] },
   { id: '#8731', customer: 'Ricardo Alves', value: 5199.90, status: 'A Enviar', items: [
       { name: 'Smartphone Pro X', quantity: 1, color: '#2c3e50', size: '256GB' },
       { name: 'Headset com Cancelamento de Ruído', quantity: 1, color: '#000000', size: 'Único' }
   ]}
]);

const expandedOrderId = ref(null);

function toggleOrderDetails(orderId) {
  expandedOrderId.value = expandedOrderId.value === orderId ? null : orderId;
}
</script>

<template>
  <div class="container mx-auto">
    <h1 class="text-3xl font-bold mb-8">Gestão de Vendas</h1>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-[#1a1a2e] p-6 rounded-lg"><h3 class="text-gray-400 text-sm">Receita Faturada</h3><p class="text-3xl font-bold text-green-400">R$ 15.789,90</p></div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg"><h3 class="text-gray-400 text-sm">Pedidos Feitos</h3><p class="text-3xl font-bold">72</p></div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg"><h3 class="text-gray-400 text-sm">Pedidos a Enviar</h3><p class="text-3xl font-bold text-yellow-400">8</p></div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg"><h3 class="text-gray-400 text-sm">Pedidos Enviados</h3><p class="text-3xl font-bold text-blue-400">64</p></div>
    </div>

    <h2 class="text-2xl font-bold mb-4">Últimos Pedidos</h2>
    <div class="bg-[#1a1a2e] rounded-lg overflow-hidden">
      <div class="grid grid-cols-4 p-4 border-b border-gray-700 font-bold text-gray-400 text-sm hidden sm:grid">
        <div>Pedido ID</div>
        <div>Cliente</div>
        <div>Valor</div>
        <div class="text-right">Status</div>
      </div>
      
      <div v-for="order in recentOrders" :key="order.id" class="border-b border-gray-700 last:border-b-0">
        <div 
          class="grid grid-cols-2 sm:grid-cols-4 p-4 items-center cursor-pointer hover:bg-black/20 transition-colors"
          @click="toggleOrderDetails(order.id)"
        >
          <div class="font-bold sm:font-normal">
            <span class="sm:hidden text-gray-400 text-xs">ID: </span>{{ order.id }}
            <i class="fas ml-2 text-xs" :class="expandedOrderId === order.id ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
          </div>
          <div><span class="sm:hidden text-gray-400 text-xs">Cliente: </span>{{ order.customer }}</div>
          <div><span class="sm:hidden text-gray-400 text-xs">Valor: </span>R${{ order.value.toFixed(2) }}</div>
          <div class="text-right">
            <span class="text-xs font-semibold px-2 py-1 rounded-full" 
                  :class="{
                    'bg-green-500 text-white': order.status === 'Enviado',
                    'bg-yellow-500 text-white': order.status === 'A Enviar'
                  }">
              {{ order.status }}
            </span>
          </div>
        </div>

        <div v-if="expandedOrderId === order.id" class="bg-black/30 px-4 sm:px-8 py-4">
          <h4 class="text-sm font-semibold text-gray-300 mb-2">Itens do Pedido:</h4>
          <ul class="list-disc list-inside space-y-1 text-sm text-gray-400">
            <li v-for="(item, index) in order.items" :key="index">
              {{ item.quantity }}x {{ item.name }} 
              <span v-if="item.color || item.size">
                (
                <span v-if="item.color && item.color !== 'N/A'">Cor: <span class="capitalize">{{ getColorName(item.color) }}</span></span>
                <span v-if="item.color && item.size && item.color !== 'N/A' && item.size !== 'N/A'">, </span>
                <span v-if="item.size && item.size !== 'N/A'">Tamanho: {{ item.size }}</span>
                )
              </span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>