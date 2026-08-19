import { createRouter, createWebHistory } from 'vue-router';
import Processes from '@/views/ViewProcesses.vue';
import Performance from '@/views/ViewPerformance.vue';
import Settings from '@/views/ViewSettings.vue';

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