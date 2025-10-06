import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

import './assets/main.css' // Se você instalou o Tailwind, essa linha pode ser diferente

const app = createApp(App)

app.use(router)

app.mount('#app')