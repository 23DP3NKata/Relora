import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { i18n } from './translate'
import './style.css'
import App from './App.vue'
import router from './app/router/router'

createApp(App)
.use(createPinia())
.use(i18n)
.use(router)
.mount('#app')
