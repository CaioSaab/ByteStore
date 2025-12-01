<script setup>
import { onMounted, ref } from 'vue'
import { getMyPurchases } from '@/services/saleService.js'

const purchases = ref([])
const isLoading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    const result = await getMyPurchases()
    if (result.success) {
      purchases.value = result.data || []
    } else {
      error.value = result.error || 'Erro ao carregar suas compras'
    }
  } catch (e) {
    error.value = e.message || 'Erro ao carregar suas compras'
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="container mx-auto p-4 text-white">
    <h1 class="text-3xl font-bold mb-6">Meus Pedidos</h1>

    <div v-if="isLoading" class="bg-[#1a1a2e] p-6 rounded-lg">Carregando...</div>
    <div v-else-if="error" class="bg-[#1a1a2e] p-6 rounded-lg text-red-400">{{ error }}</div>

    <div v-else>
      <div v-if="purchases.length > 0" class="space-y-6">
        <div v-for="order in purchases" :key="order.id" class="bg-[#1a1a2e] p-6 rounded-lg">
          <div class="flex justify-between items-center border-b border-gray-700 pb-4 mb-4">
            <div>
              <h2 class="text-xl font-semibold">Pedido {{ order.pedidoId }}</h2>
              <p class="text-gray-400 text-sm">Data: {{ new Date(order.dataDaVenda).toLocaleString() }}</p>
            </div>
            <div class="text-right">
              <p class="text-lg font-bold">Total: R${{ Number(order.value).toFixed(2) }}</p>
              <span class="text-xs text-gray-400">{{ order.status }}</span>
            </div>
          </div>

          <div class="space-y-3">
            <div v-for="item in order.items" :key="item.name + item.size + item.color" class="flex justify-between">
              <div>
                <p class="font-medium">{{ item.name }}</p>
                <p class="text-gray-400 text-sm">Cor: {{ item.color || '-' }}, Tamanho: {{ item.size || '-' }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm">Qtd: {{ item.quantity }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="bg-[#1a1a2e] p-6 rounded-lg text-center">
        <p>Você ainda não possui pedidos.</p>
      </div>
    </div>
  </div>
</template>