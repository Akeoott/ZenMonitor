<script setup lang="ts">
import { nextTick, onMounted, onUnmounted } from 'vue';
import Lenis from 'lenis';
import MainLayout from '@/layouts/MainLayout.vue';

let lenis: Lenis | null = null;

onMounted(async () => {
  await nextTick();

  lenis = new Lenis({
    duration: 1,
    easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t)),
    smoothWheel: true,
  });

  function raf(time: number) {
    lenis?.raf(time);
    requestAnimationFrame(raf);
  }

  requestAnimationFrame(raf);
});

onUnmounted(() => {
  lenis?.destroy();
});
</script>

<template>
  <MainLayout>
    <router-view />
  </MainLayout>
</template>