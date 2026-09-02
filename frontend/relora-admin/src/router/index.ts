import { createRouter, createWebHistory } from 'vue-router'
import PendingLotsPage from '@/pages/PendingLotsPage.vue'
import DashboardPage from '@/pages/DashboardPage.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/admin/pending',
    },
    {
      path: '/admin/pending',
      component: PendingLotsPage,
    },
    {
      path: '/admin/dashboard',
      component: DashboardPage,
    },
  ],
})

export default router
