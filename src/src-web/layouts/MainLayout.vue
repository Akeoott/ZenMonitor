<script setup lang="ts">
import CompSidebar from "@/components/CompSidebar.vue";
</script>

<template>
  <div class="h-screen w-screen overflow-hidden flex flex-col font-sans select-none">

    <header class="px-3 h-12 w-full flex items-center">
      <h1 class="text-l font-bold">ZenMonitor</h1>
    </header>

    <div class="custom-scroll flex flex-1 overflow-hidden">
      <CompSidebar/>

      <main class="flex-1 h-full bg-bg-900 rounded-tl-2xl overflow-hidden flex flex-col min-h-0">
        <div class="flex-1 min-h-0 overflow-y-auto">
          <RouterView />
        </div>
      </main>
    </div>
  </div>
</template>

<style scoped/>
