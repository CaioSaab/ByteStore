<script setup>
import { computed, ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { store } from '@/store/index.js'
import { createOrder } from '@/services/checkoutService'

// --- ESTADO DO COMPONENTE ---
const couponInput = ref('')
const showSuccessPopup = ref(false)
const router = useRouter()
const shippingCost = computed(() => (subtotal.value * 0.05))

// --- ESTADO PARA PAGAMENTO COM CARTÃO ---
const selectedPaymentMethod = ref('credit')
const cardNumber = ref('')
const cardName = ref('')
const cardExpiry = ref('')
const cardCvv = ref('')

// Lista simulada de cartões salvos
const savedCards = ref([
  { id: 1, brand: 'Visa', last4: '1234', display: 'Visa **** **** **** 1234' }
])

// ID do cartão salvo selecionado
const selectedSavedCardId = ref(
  savedCards.value.length > 0 ? savedCards.value[0].id : null
)

// Controla se o formulário de novo cartão é visível
const showNewCardForm = computed(() => selectedSavedCardId.value === null)

// Número de parcelas
const installments = ref(1)

const discountAmount = computed(() => {
  if (!store.appliedCoupon || !store.discountPercentage) return 0
  return subtotal.value * store.discountPercentage
})

// --- PROPRIEDADES COMPUTADAS ---
const subtotal = computed(() =>
  store.cart.reduce((acc, item) => acc + item.price * item.quantity, 0)
)
const grandTotal = computed(
  () => subtotal.value + shippingCost.value - discountAmount.value
)

const installmentOptions = computed(() => {
  const total = grandTotal.value
  if (total <= 0) return [{ value: 1, text: '1x de R$ 0.00' }]
  const options = []
  const maxInstallments = 12
  for (let i = 1; i <= maxInstallments; i++) {
    const installmentValue = total / i
    const interestRate = i > 1 ? 0.0199 : 0
    const finalInstallmentValue = installmentValue * (1 + (i - 1) * interestRate)
    const totalWithInterest = finalInstallmentValue * i
    let text = `${i}x de R$ ${finalInstallmentValue.toFixed(2)}`
    if (i > 1) text += ` (Total R$ ${totalWithInterest.toFixed(2)})`
    else text += ' sem juros'
    options.push({ value: i, text })
    if (total < 50 && i >= 3) break
    if (total < 100 && i >= 6) break
  }
  return options
})

// --- WATCHERS ---
watch(selectedPaymentMethod, (newValue) => {
  if (newValue === 'debit') installments.value = 1
})

const handleApplyCoupon = async () => {
  const code = couponInput.value.toUpperCase().trim()
  if (!code) {
    return
  }

  await store.applyCoupon(code)
}

function saveNewCard() {
  if (
    !cardNumber.value ||
    cardNumber.value.length < 15 ||
    !cardName.value ||
    !cardExpiry.value ||
    !cardCvv.value
  ) {
    alert('Por favor, preencha todos os dados do cartão.')
    return
  }
  const newCard = {
    id: Date.now(),
    brand: 'Novo Cartão',
    last4: cardNumber.value.slice(-4),
    number: cardNumber.value,
    name: cardName.value,
    expiry: cardExpiry.value,
    display: `Novo **** **** **** ${cardNumber.value.slice(-4)}`
  }
  savedCards.value.push(newCard)
  selectedSavedCardId.value = newCard.id
  cardNumber.value = ''
  cardName.value = ''
  cardExpiry.value = ''
  cardCvv.value = ''
  alert('Cartão salvo com sucesso!')
}

const validatePaymentData = () => {
  if (selectedPaymentMethod.value === 'credit' || selectedPaymentMethod.value === 'debit') {
    if (selectedSavedCardId.value === null) {
      if (!cardNumber.value || !cardName.value || !cardExpiry.value || !cardCvv.value) {
        return { valid: false, error: 'Por favor, preencha ou selecione os dados do cartão.' };
      }
    } else {
      const selectedCard = savedCards.value.find((c) => c.id === selectedSavedCardId.value);
      if (!selectedCard) {
        return { valid: false, error: 'Erro: Cartão salvo não encontrado.' };
      }
    }
  }
  return { valid: true };
};

const handlePlaceOrder = async () => {
  const validation = validatePaymentData();
  if (!validation.valid) {
    alert(validation.error);
    return;
  }

  try {
    const orderData = {
      items: store.cart,
      paymentMethod: selectedPaymentMethod.value,
      installments: installments.value,
      coupon: store.appliedCoupon,
      shippingCost: shippingCost.value,
    };

    if (selectedPaymentMethod.value === 'credit' || selectedPaymentMethod.value === 'debit') {
      if (selectedSavedCardId.value) {
        orderData.cardId = selectedSavedCardId.value;
      } else {
        orderData.card = {
          number: cardNumber.value,
          name: cardName.value,
          expiry: cardExpiry.value,
          cvv: cardCvv.value,
        };
      }
    }

    const result = await createOrder(orderData);
    
    if (result.success) {
      showSuccessPopup.value = true;
    } else {
      alert(result.error || 'Erro ao finalizar pedido');
    }
  } catch (error) {
    alert(error.message || 'Erro ao finalizar pedido');
  }
};

onMounted(async () => {
  if (store.isAuthenticated && store.cart.length === 0) {
    await store.refreshCart();
  }
});

function confirmSuccess() {
  showSuccessPopup.value = false
  store.clearCartAndCoupon()
  router.push({ name: 'home' })
}
</script>

<template>
  <div class="relative">
    <div class="container mx-auto p-4 text-white">
      <h1 class="text-3xl font-bold mb-6">Checkout</h1>

      <div v-if="store.cart.length > 0" class="flex flex-col lg:flex-row gap-8">
        <!-- Coluna Esquerda -->
        <div class="lg:w-2/3 bg-[#1a1a2e] p-6 rounded-lg">
          <h2 class="text-2xl font-bold mb-4">Informações de Entrega</h2>
          <p class="text-gray-400 mb-8">Os campos do formulário de endereço iriam aqui...</p>

          <h2 class="text-2xl font-bold mt-8 mb-4">Método de Pagamento</h2>
          <div class="space-y-4">
            <!-- Cartão de Crédito -->
            <div
              @click="selectedPaymentMethod = 'credit'"
              class="border rounded-lg p-4 cursor-pointer transition-colors"
              :class="selectedPaymentMethod === 'credit' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            >
              <h3 class="font-semibold"><i class="fas fa-credit-card mr-2"></i> Cartão de Crédito</h3>
            </div>

            <!-- Cartão de Débito -->
            <div
              @click="selectedPaymentMethod = 'debit'"
              class="border rounded-lg p-4 cursor-pointer transition-colors"
              :class="selectedPaymentMethod === 'debit' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            >
              <h3 class="font-semibold"><i class="fas fa-money-check-alt mr-2"></i> Cartão de Débito</h3>
            </div>

            <!-- Seção do Cartão -->
            <div
              v-if="selectedPaymentMethod === 'credit' || selectedPaymentMethod === 'debit'"
              class="bg-black/20 p-4 rounded-lg space-y-4"
            >
              <!-- Selecionar Cartão Salvo -->
              <div v-if="savedCards.length > 0">
                <label for="savedCardSelect" class="block text-sm font-medium text-gray-300 mb-2">Usar Cartão Salvo</label>
                <select
                  v-model="selectedSavedCardId"
                  id="savedCardSelect"
                  class="select-arrow"
                >
                  <option v-for="card in savedCards" :key="card.id" :value="card.id">
                    {{ card.display }}
                  </option>
                  <option :value="null">-- Adicionar novo cartão --</option>
                </select>
              </div>

              <!-- Novo Cartão -->
              <div v-if="showNewCardForm" class="space-y-4 border-t border-gray-700 pt-4 mt-4">
                <h4 v-if="savedCards.length > 0" class="text-md font-semibold text-gray-300 mb-2">
                  Dados do Novo Cartão
                </h4>

                <div>
                  <label for="cardNumber" class="block text-sm font-medium text-gray-300 mb-1">Número do Cartão *</label>
                  <input v-model="cardNumber" type="text" id="cardNumber" placeholder="0000 0000 0000 0000" class="input-direct-style" />
                </div>

                <div>
                  <label for="cardName" class="block text-sm font-medium text-gray-300 mb-1">Nome no Cartão *</label>
                  <input v-model="cardName" type="text" id="cardName" placeholder="Seu nome completo" class="input-direct-style" />
                </div>

                <div class="flex flex-col sm:flex-row gap-4">
                  <div class="sm:w-1/2">
                    <label for="cardExpiry" class="block text-sm font-medium text-gray-300 mb-1">Validade (MM/AA) *</label>
                    <input v-model="cardExpiry" type="text" id="cardExpiry" placeholder="12/28" class="input-direct-style" />
                  </div>
                  <div class="sm:w-1/2">
                    <label for="cardCvv" class="block text-sm font-medium text-gray-300 mb-1">CVV *</label>
                    <input v-model="cardCvv" type="text" id="cardCvv" placeholder="123" class="input-direct-style" />
                  </div>
                </div>

                <div class="pt-2">
                  <button
                    @click.prevent="saveNewCard"
                    class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-lg text-sm"
                  >
                    Salvar Cartão
                  </button>
                </div>
              </div>

              <!-- Parcelamento (Crédito) -->
              <div v-if="selectedPaymentMethod === 'credit'" class="mt-4 border-t border-gray-700 pt-4">
                <label for="installments" class="block text-sm font-medium text-gray-300 mb-1">Parcelamento</label>
                <select v-model="installments" id="installments" class="select-arrow">
                  <option v-for="option in installmentOptions" :key="option.value" :value="option.value">
                    {{ option.text }}
                  </option>
                </select>
              </div>
            </div>

            <!-- PIX -->
            <div
              @click="selectedPaymentMethod = 'pix'"
              class="border rounded-lg p-4 cursor-pointer transition-colors"
              :class="selectedPaymentMethod === 'pix' ? 'border-blue-500 bg-blue-900/50' : 'border-gray-700 hover:border-gray-500'"
            >
              <h3 class="font-semibold"><i class="fas fa-qrcode mr-2"></i> PIX</h3>
              <div v-if="selectedPaymentMethod === 'pix'" class="mt-4 flex flex-col items-center">
                <p class="text-sm text-gray-300 mb-2">Aponte a câmera do seu celular para o QR Code</p>
                <img
                  src="https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=Pagamento PIX para ByteStore - Pedido #12345"
                  alt="Fake QR Code"
                  class="bg-white p-2 rounded-lg"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Coluna Direita -->
        <div class="lg:w-1/3">
          <div class="bg-[#1a1a2e] rounded-lg shadow-lg p-6 sticky top-24">
            <h2 class="text-2xl font-bold mb-4">Resumo do Pedido</h2>
            <div class="space-y-3 border-b border-gray-700 pb-4 mb-4">
              <div v-for="item in store.cart" :key="item.cartItemId" class="flex justify-between text-sm">
                <span class="truncate pr-2">{{ item.name }} <span class="text-gray-400">x{{ item.quantity }}</span></span>
                <span>R${{ (item.price * item.quantity).toFixed(2) }}</span>
              </div>
            </div>

            <div class="space-y-2">
              <div class="flex justify-between"><span>Subtotal</span><span>R${{ subtotal.toFixed(2) }}</span></div>
              <div class="flex justify-between text-gray-400"><span>Frete</span><span>R${{ shippingCost.toFixed(2) }}</span></div>

              <!-- CUPOM DE DESCONTO -->
              <div class="mt-4 flex gap-2">
                <input
                  v-model="couponInput"
                  type="text"
                  placeholder="Digite o cupom"
                  class="input-direct-style flex-1"
                />
                <button
                  @click.prevent="handleApplyCoupon"
                  class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-lg"
                >
                  Aplicar
                </button>
              </div>

              <div v-if="store.appliedCoupon" class="flex justify-between text-green-400 mt-2">
                <span>Desconto ({{ store.appliedCoupon }})</span>
                <span>-R${{ discountAmount.toFixed(2) }}</span>
              </div>
            </div>

            <div class="border-t border-gray-700 pt-4 mt-4 font-bold text-xl flex justify-between">
              <span>Total</span>
              <span>R${{ grandTotal.toFixed(2) }}</span>
            </div>

            <button
              @click="handlePlaceOrder"
              class="w-full mt-6 bg-green-600 hover:bg-green-700 text-white font-bold py-3 px-6 rounded-lg text-lg transition duration-300"
            >
              Finalizar Pedido
            </button>
          </div>
        </div>
      </div>

      <div v-else class="text-center bg-[#1a1a2e] p-10 rounded-lg">Seu carrinho está vazio.</div>

      <!-- POPUP DE SUCESSO -->
      <div
        v-if="showSuccessPopup"
        class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-60 z-50"
      >
        <div class="bg-[#1a1a2e] border border-blue-700 text-white p-8 rounded-2xl shadow-2xl w-80 text-center">
          <h3 class="text-2xl font-bold mb-3 text-green-400">Pedido Finalizado!</h3>
          <p class="text-gray-300 mb-6">Seu pedido foi processado com sucesso.</p>
          <button
            @click="confirmSuccess"
            class="bg-green-600 hover:bg-green-700 text-white font-bold py-2 px-6 rounded-lg"
          >
            OK
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.select-arrow {
  background-color: #13142b !important;
  border: 1px solid #2a2d4a !important;
  border-radius: 0.5rem;
  padding: 0.5rem 0.75rem;
  width: 100%;
  color: #fff !important;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
  appearance: none;
  transition: all 0.2s ease-in-out;
}

.input-direct-style {
  background-color: #13142b !important;
  border: 1px solid #2a2d4a !important;
  border-radius: 0.5rem;
  padding: 0.5rem 0.75rem;
  width: 100%;
  color: #fff !important;
  transition: all 0.2s ease-in-out;
}

.input-direct-style::placeholder {
  color: #6b7280;
}

.input-direct-style:focus,
.select-arrow:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.3);
}
</style>
