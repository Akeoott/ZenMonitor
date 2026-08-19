<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const currentRoute = useRoute()

const isExpanded = ref(true)

interface NavItem {
  path: string
  label: string
  icon: string
  class?: string
}

const navItems = ref<NavItem[]>([
  { path: '/', label: 'Processes', icon: '📊' },
  { path: '/performance', label: 'Performance', icon: '📈' },
  { path: '/settings', label: 'Settings', icon: '⚙️', class: 'mt-auto' }
])

const navigateTo = (path: string) => {
  router.push(path)
}
</script>

<template>
  <aside
    class="w-fit flex flex-col p-2 pt-1 h-full ">
    <nav class="flex flex-col flex-1 gap-1">
      <button @click="isExpanded = !isExpanded" class="nav-item text-gray-400">
        <span class="nav-icon">🟰</span>
      </button>

      <button
        v-for="item in navItems" :key="item.path" @click="navigateTo(item.path)"
        :class="[
              'nav-item',
              item.class,
              { active: currentRoute.path === item.path }
            ]">
        <span class="nav-icon">{{ item.icon }}</span>
        <span v-if="isExpanded" class="nav-label">{{ item.label }}</span>
      </button>
    </nav>
  </aside>
</template>

<style scoped>
@import "tailwindcss";
@import "../css/theme.css";

.nav-item {
  @apply flex items-center gap-3 px-2 py-2 rounded-lg text-sm font-medium cursor-pointer
  text-gray-400 hover:text-white hover:bg-white/5 w-full justify-start select-none
  border-none bg-transparent;
}

.nav-item.active {
  @apply bg-white/10 text-white hover:bg-white/15;
}

.nav-icon {
  @apply text-lg shrink-0 flex items-center justify-center w-6 h-6;
}

.nav-label {
  @apply whitespace-nowrap overflow-hidden text-ellipsis text-left;
}
</style>