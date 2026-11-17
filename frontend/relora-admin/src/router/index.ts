import { createRouter, createWebHistory } from 'vue-router'
import PendingLotsPage from '@/pages/PendingLotsPage.vue'
import PendingLotDetailsPage from '@/pages/PendingLotDetailsPage.vue'
import DashboardPage from '@/pages/DashboardPage.vue'
import DisputesPage from '@/pages/DisputesPage.vue'
import OverdueShipmentsPage from '@/pages/OverdueShipmentsPage.vue'
import SupportRequestsPage from '@/pages/SupportRequestsPage.vue'

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
      path: '/admin/pending/:id',
      component: PendingLotDetailsPage,
    },
    {
      path: '/admin/dashboard',
      component: DashboardPage,
    },
    {
      path: '/admin/disputes',
      component: DisputesPage,
    },
    {
      path: '/admin/overdue-shipments',
      component: OverdueShipmentsPage,
    },
    {
      path: '/admin/support',
      component: SupportRequestsPage,
    },
  ],
})

export default router
