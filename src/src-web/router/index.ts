import Performance from '@/views/PerformanceView.vue';
import Processes from '@/views/ProcessesView.vue';
import Settings from '@/views/SettingsView.vue';
import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  { path: '/', component: Processes },
  { path: '/performance', component: Performance },
  { path: '/settings', component: Settings },
  { path: "/:pathMatch(.*)*", component: Processes },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
