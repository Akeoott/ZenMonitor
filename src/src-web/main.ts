import {getCurrentWindow } from '@tauri-apps/api/window';
import { createApp } from 'vue';

import App from '@/App.vue';
import router from './router';
import '@/css/styles.css';

import { isRunningInTauri } from "./utils/Utils.ts";
import { api } from './services/Api'

const app = createApp(App)

if (isRunningInTauri)
  await getCurrentWindow().show();

await api.connect()

app.use(router)
app.mount('#app')

if ('scrollRestoration' in history)
  history.scrollRestoration = 'manual';

requestAnimationFrame(() => {
  const loader = document.getElementById("page-loader");
  if (loader) {
    loader.classList.add("hidden");
    setTimeout(() => loader.remove(), 500);
  }
});