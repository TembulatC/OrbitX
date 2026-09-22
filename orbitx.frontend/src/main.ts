import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router' // Импортируем роутер
import 'leaflet/dist/leaflet.css'

const app = createApp(App)

app.use(router) // Говорим Vue использовать роутер
app.mount('#app')
