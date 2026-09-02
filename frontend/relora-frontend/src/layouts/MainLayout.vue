<script setup lang="ts">
import Header from '@/components/layout/Header.vue'
import Footer from '@/components/layout/Footer.vue'
import Container from '@/components/ui/Container.vue'
</script>

<template>
  <div class="min-h-screen flex flex-col selection:bg-black selection:text-white dark:selection:bg-white dark:selection:text-black">
    <Header />

    <main class="flex-1 py-6">
      <Container>
        <RouterView />
      </Container>
    </main>

    <Footer />
  </div>
</template>