<script setup>
import { ref, onMounted, computed } from 'vue'; // 1. IMPORTAR 'computed'
import { getMySales, updateOrderStatus } from '@/services/saleService'; 

// --- MAPA DE CORES (Seu código original) ---
const colorNameMap = {
  // ... (seu mapa de cores) ...
  'N/A': ''
};

function getColorName(hexCode) {
  return colorNameMap[hexCode] || hexCode;
}

// --- DADOS REATIVOS ---
const sales = ref([]); 
const isLoading = ref(false);
const errorMessage = ref('');
const expandedOrderId = ref(null);

// --- FUNÇÃO PARA CARREGAR VENDAS (Seu código original) ---
const loadSales = async () => {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    const result = await getMySales();
    if (result.success) {
      sales.value = result.data;
    } else {
      errorMessage.value = result.error;
    }
  } catch (error) {
    errorMessage.value = 'Falha ao conectar com o servidor.';
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  loadSales();
});

const totalRevenue = computed(() => {
  return sales.value.reduce((sum, order) => sum + order.value, 0);
});

const totalOrders = computed(() => {
  return sales.value.length;
});

const pendingOrders = computed(() => {
  return sales.value.filter(order => order.status === 'A Enviar').length;
});

const shippedOrders = computed(() => {
  return sales.value.filter(order => order.status === 'Enviado').length;
});


function toggleOrderDetails(orderId) {
  expandedOrderId.value = expandedOrderId.value === orderId ? null : orderId;
}

async function markAsShipped(order) {
  const result = await updateOrderStatus(order.id, 1);
  if (result.success) {
    await loadSales();
  }
}
</script>

<template>
  <div class="container mx-auto">
    <h1 class="text-3xl font-bold mb-8">Gestão de Vendas</h1>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-[#1a1a2e] p-6 rounded-lg">
        <h3 class="text-gray-400 text-sm">Receita Faturada</h3>
        <p class="text-3xl font-bold text-green-400">R$ {{ totalRevenue.toFixed(2) }}</p>
      </div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg">
        <h3 class="text-gray-400 text-sm">Pedidos Feitos</h3>
        <p class="text-3xl font-bold">{{ totalOrders }}</p>
      </div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg">
        <h3 class="text-gray-400 text-sm">Pedidos a Enviar</h3>
        <p class="text-3xl font-bold text-yellow-400">{{ pendingOrders }}</p>
      </div>
      <div class="bg-[#1a1a2e] p-6 rounded-lg">
        <h3 class="text-gray-400 text-sm">Pedidos Enviados</h3>
        <p class="text-3xl font-bold text-blue-400">{{ shippedOrders }}</p>
      </div>
    </div>

    <h2 class="text-2xl font-bold mb-4">Últimos Pedidos</h2>
    
    <div v-if="isLoading" class="text-center text-gray-400 py-10">
      Carregando pedidos...
    </div>
    <div v-if="errorMessage" class="text-center text-red-400 py-10">
      {{ errorMessage }}
    </div>

    <div v-if="!isLoading && sales.length === 0 && !errorMessage" class="text-center text-gray-500 py-10">
      Nenhum pedido encontrado.
    </div>
    
    <div v-if="sales.length > 0" class="bg-[#1a1a2e] rounded-lg overflow-hidden">
      <div class="grid grid-cols-5 p-4 border-b border-gray-700 font-bold text-gray-400 text-sm hidden sm:grid">
        <div>Pedido ID</div>
        <div>Cliente</div>
        <div>Valor</div>
        <div>Método</div>
        <div class="text-right">Status</div>
      </div>
      
      <div v-for="order in sales" :key="order.id" class="border-b border-gray-700 last:border-b-0">
        <div 
          class="grid grid-cols-2 sm:grid-cols-5 p-4 items-center cursor-pointer hover:bg-black/20 transition-colors"
          @click="toggleOrderDetails(order.id)">
          <div class="font-bold sm:font-normal">
            <span class="sm:hidden text-gray-400 text-xs">ID: </span>{{ order.pedidoId }}
            <i class="fas ml-2 text-xs" :class="expandedOrderId === order.id ? 'fa-chevron-up' : 'fa-chevron-down'"></i>
          </div>
          <div><span class="sm:hidden text-gray-400 text-xs">Cliente: </span>{{ order.customer }}</div>
          <div><span class="sm:hidden text-gray-400 text-xs">Valor: </span>R${{ order.value.toFixed(2) }}</div>
          <div><span class="sm:hidden text-gray-400 text-xs">Pagamento: </span>{{ order.paymentMethod }}</div>
          <div class="text-right">
            <span class="text-xs font-semibold px-2 py-1 rounded-full" 
                  @click.stop="order.status === 'A Enviar' && markAsShipped(order)"
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