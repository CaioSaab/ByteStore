<script setup>
import { ref, onMounted } from 'vue'
import { getVendorCoupons, createOrUpdateCoupon, toggleCouponActive, deleteCoupon } from '@/services/couponService'

const form = ref({ code: '', discountPercentage: 0, isActive: true })
const coupons = ref([])
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const loadCoupons = async () => {
  isLoading.value = true
  errorMessage.value = ''
  try {
    const result = await getVendorCoupons()
    if (result.success) {
      coupons.value = result.data || []
    } else {
      errorMessage.value = result.error
    }
  } catch (e) {
    errorMessage.value = e.message || 'Erro ao carregar cupons'
  } finally {
    isLoading.value = false
  }
}

const handleSubmit = async () => {
  errorMessage.value = ''
  successMessage.value = ''
  const payload = {
    code: (form.value.code || '').toUpperCase().trim(),
    discountPercentage: Number(form.value.discountPercentage) || 0,
    isActive: !!form.value.isActive,
  }
  if (!payload.code) {
    errorMessage.value = 'Informe o código do cupom'
    return
  }
  try {
    const result = await createOrUpdateCoupon(payload)
    if (result.success) {
      successMessage.value = 'Cupom salvo com sucesso'
      form.value = { code: '', discountPercentage: 0, isActive: true }
      await loadCoupons()
    } else {
      errorMessage.value = result.error
    }
  } catch (e) {
    errorMessage.value = e.message || 'Erro ao salvar cupom'
  }
}

const startEdit = (c) => {
  form.value = {
    code: c.code,
    discountPercentage: Number(c.discountPercentage) || 0,
    isActive: !!c.isActive,
  }
}

const handleToggleActive = async (c) => {
  errorMessage.value = ''
  successMessage.value = ''
  try {
    const result = await toggleCouponActive(c.code, !c.isActive)
    if (result.success) {
      successMessage.value = 'Status do cupom atualizado'
      await loadCoupons()
    } else {
      errorMessage.value = result.error
    }
  } catch (e) {
    errorMessage.value = e.message || 'Erro ao atualizar status do cupom'
  }
}

const handleDelete = async (code) => {
  errorMessage.value = ''
  successMessage.value = ''
  try {
    const result = await deleteCoupon(code)
    if (result.success) {
      successMessage.value = 'Cupom excluído'
      await loadCoupons()
    } else {
      errorMessage.value = result.error
    }
  } catch (e) {
    errorMessage.value = e.message || 'Erro ao excluir cupom'
  }
}

onMounted(async () => {
  await loadCoupons()
})
</script>

<template>
  <div class="container mx-auto">
    <h1 class="text-3xl font-bold mb-4 text-white">Cupons do Vendedor</h1>

    <div v-if="errorMessage" class="mb-6 p-4 bg-red-900/50 border border-red-500 rounded-lg text-red-300">
      {{ errorMessage }}
    </div>
    <div v-if="successMessage" class="mb-6 p-4 bg-green-900/40 border border-green-500 rounded-lg text-green-300">
      {{ successMessage }}
    </div>

    <div class="bg-[#1a1a2e] rounded-lg p-6 shadow-lg mb-8">
      <h2 class="text-2xl font-bold mb-4 text-white">Criar ou Atualizar Cupom</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-gray-400 mb-1">Código</label>
          <input v-model="form.code" type="text" class="w-full p-2 rounded bg-gray-800 text-white border border-gray-700" placeholder="EXEMPLO10" />
        </div>
        <div>
          <label class="block text-gray-400 mb-1">Desconto (%)</label>
          <input v-model.number="form.discountPercentage" type="number" min="0" max="100" step="0.1" class="w-full p-2 rounded bg-gray-800 text-white border border-gray-700" />
        </div>
        <div class="flex items-end">
          <label class="inline-flex items-center gap-2 text-gray-300">
            <input v-model="form.isActive" type="checkbox" class="rounded" />
            Ativo
          </label>
        </div>
      </div>
      <div class="mt-4">
        <button @click="handleSubmit" class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">Salvar</button>
      </div>
    </div>

    <div class="bg-[#1a1a2e] rounded-lg p-6 shadow-lg">
      <h2 class="text-2xl font-bold mb-4 text-white">Meus Cupons</h2>
      <div v-if="isLoading" class="text-center py-10 text-gray-400">Carregando cupons...</div>
      <div v-else>
        <div v-if="!coupons || coupons.length === 0" class="text-gray-400">Nenhum cupom cadastrado.</div>
        <div v-else class="overflow-x-auto">
          <table class="min-w-full text-left text-gray-300">
            <thead>
              <tr class="border-b border-gray-700">
                <th class="py-2 px-3">Código</th>
                <th class="py-2 px-3">Desconto</th>
                <th class="py-2 px-3">Status</th>
                <th class="py-2 px-3" style="text-align: right;">Ações</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="c in coupons" :key="c.code" class="border-b border-gray-800">
                <td class="py-2 px-3 font-semibold text-white">{{ c.code }}</td>
                <td class="py-2 px-3">{{ Number(c.discountPercentage).toFixed(2) }}%</td>
                <td class="py-2 px-3">
                  <span :class="c.isActive ? 'text-green-400' : 'text-gray-500'">{{ c.isActive ? 'Ativo' : 'Inativo' }}</span>
                </td>
                <td class="py-2 px-3" style="text-align: right;">
                  <button @click="handleToggleActive(c)" class="bg-green-600 hover:bg-green-700 text-white font-bold py-1 px-3 rounded mr-2">{{ c.isActive ? 'Desativar' : 'Ativar' }}</button>
                  <button @click="startEdit(c)" class="bg-yellow-600 hover:bg-yellow-700 text-white font-bold py-1 px-3 rounded mr-2">Editar</button>
                  <button @click="handleDelete(c.code)" class="bg-red-600 hover:bg-red-700 text-white font-bold py-1 px-3 rounded">Excluir</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>