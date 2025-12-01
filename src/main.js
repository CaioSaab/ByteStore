import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { store } from './store/index.js'

import './assets/main.css'

async function bootstrap() {
  await store.initialize();
  const app = createApp(App);
  app.use(router);
  app.mount('#app');
}

bootstrap();