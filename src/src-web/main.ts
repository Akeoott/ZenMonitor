import App from '@/App.vue';
import { getCurrentWindow } from '@tauri-apps/api/window';
import { createApp } from 'vue';

import '@/styles/styles.css';
import router from './router';

import { api } from "@/services/Api";
import { isRunningInTauri } from '@/utils/Utils';

const app = createApp(App);

if (isRunningInTauri)
  await getCurrentWindow().show();

await api.connect()

app.use(router);
app.mount('#app');

if ('scrollRestoration' in history)
  history.scrollRestoration = 'manual';
