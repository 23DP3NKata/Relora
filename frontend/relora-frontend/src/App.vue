<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { Toaster } from "@/components/ui/sonner"

const authStore = useAuthStore()

onMounted(async () => {
  await authStore.checkAuth()
})
</script>

<template>
  <RouterView />
  <Toaster />
</template>