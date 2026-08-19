import {getCurrentWindow } from '@tauri-apps/api/window';
import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import './css/styles.css';

const app = createApp(App)

app.use(router)

app.mount('#app')

if ('scrollRestoration' in history) {
  history.scrollRestoration = 'manual';
}

// Show window after frontend has loaded
await getCurrentWindow().show();
