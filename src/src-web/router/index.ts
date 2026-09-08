import { createRouter, createWebHistory } from 'vue-router';

import MainLayout from '@/layouts/MainLayout.vue';
import Performance from '@/views/PerformanceView.vue';
import Processes from '@/views/ProcessesView.vue';
import Settings from '@/views/SettingsView.vue';

const routes = [
  { path: '/', component: Processes, meta: { layout: MainLayout } },
  { path: '/performance', component: Performance, meta: { layout: MainLayout } },
  { path: '/settings', component: Settings, meta: { layout: MainLayout } },
  { path: "/:pathMatch(.*)*", component: Processes, meta: { layout: MainLayout } },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router
