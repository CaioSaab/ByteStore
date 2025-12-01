import { createRouter, createWebHistory } from 'vue-router'
import { store } from '@/store/index.js'

import HomeView from '../views/HomeView.vue'
import ProductDetailView from '../views/ProductDetailView.vue'
import CartView from '../views/CartView.vue'
import CheckoutView from '../views/CheckoutView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
import MainLayout from '../layouts/MainLayout.vue'
import VendorLoginView from '../views/VendorLoginView.vue'
import VendorDashboardView from '../views/VendorDashboardView.vue'

// --- NOVOS IMPORTS ---
// Importando os novos arquivos que vamos criar/usar
import VendorLayout from '../layouts/VendorLayout.vue'
import VendorRegisterView from '../views/VendorRegisterView.vue'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/login' },

    // --- ROTAS PÚBLICAS ---
    { path: '/login', name: 'login', component: LoginView },
    { path: '/register', name: 'register', component: RegisterView },
    { path: '/forgot-password', name: 'forgot-password', component: ForgotPasswordView },
    { path: '/vendor/login', name: 'vendor-login', component: VendorLoginView },
    // --- NOVA ROTA PÚBLICA ---
    { path: '/vendor/register', name: 'vendor-register', component: VendorRegisterView },

    // --- ROTAS PRIVADAS DO CLIENTE ---
    {
      path: '/app',
      component: MainLayout,
      beforeEnter: (to, from, next) => store.isAuthenticated ? next() : next('/login'),
      children: [
        { path: 'home', name: 'home', component: HomeView },
        { path: 'product/:id', name: 'product-detail', component: ProductDetailView, props: true },
        { path: 'cart', name: 'cart', component: CartView },
        { path: 'checkout', name: 'checkout', component: CheckoutView },
        { path: 'purchases', name: 'purchases', component: () => import('@/views/PurchasesView.vue') }
      ]
    },

    // --- ROTAS PRIVADAS DO VENDEDOR ---

    {
      path: '/vendor',
      component: VendorLayout,
      beforeEnter: (to, from, next) => store.isVendorAuthenticated ? next() : next('/vendor/login'),
      children: [
        { path: 'dashboard', name: 'vendor-dashboard', component: VendorDashboardView },
        { path: 'questions', name: 'vendor-questions', component: () => import('../views/VendorQuestionsView.vue') },
        { path: 'sales', name: 'vendor-sales', component: () => import('../views/VendorSalesView.vue') },
        { path: 'coupons', name: 'vendor-coupons', component: () => import('../views/VendorCouponsView.vue') }
      ]
    }
  ]
})

export default router