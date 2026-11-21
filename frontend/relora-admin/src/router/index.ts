import { createRouter, createWebHistory } from 'vue-router'
import PendingLotsPage from '@/pages/PendingLotsPage.vue'
import PendingLotDetailsPage from '@/pages/PendingLotDetailsPage.vue'
import DashboardPage from '@/pages/DashboardPage.vue'
import DisputesPage from '@/pages/DisputesPage.vue'
import OverdueShipmentsPage from '@/pages/OverdueShipmentsPage.vue'
import SupportRequestsPage from '@/pages/SupportRequestsPage.vue'
import LoginPage from '@/pages/LoginPage.vue'
import { currentUser, authService } from '@/services/authService'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/admin/pending',
    },
    {
      path: '/login',
      component: LoginPage,
      meta: { public: true },
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

// bez admina citas lapas nerāda, sūta uz login
router.beforeEach(async (to) => {
  if (to.meta.public) {
    return true
  }

  const user = currentUser.value ?? (await authService.loadCurrentUser())

  if (!user || !user.isAdmin) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }

  return true
})

export default router
