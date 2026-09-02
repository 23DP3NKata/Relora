import { createRouter, createWebHistory, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import {
  getPreferredLocale,
  localizedFullPath,
  routeLocale,
  setLocale,
  type AppLocale,
} from '@/translate'
import { installSeo } from '@/app/seo'

import LoginPage from '@/pages/auth/LoginPage.vue'
import RegisterPage from '@/pages/auth/RegisterPage.vue'
import ForgotPasswordPage from '@/pages/auth/ForgotPasswordPage.vue'
import ResetPasswordPage from '@/pages/auth/ResetPasswordPage.vue'
import HomePage from '@/pages/home/HomePage.vue'
import AboutPage from '@/pages/info/AboutPage.vue'
import TermsPage from '@/pages/info/TermsPage.vue'
import PrivacyPolicyPage from '@/pages/info/PrivacyPolicyPage.vue'
import HowToSellPage from '@/pages/info/sell/HowToSellPage.vue'
import SellerProtectionPage from '@/pages/info/sell/SellerProtectionPage.vue'
import BrandPartnershipsPage from '@/pages/info/sell/BrandPartnershipsPage.vue'
import CookiePolicyPage from '@/pages/info/legal/CookiePolicyPage.vue'
import AuctionRulesPage from '@/pages/info/legal/AuctionRulesPage.vue'
import JournalPage from '@/pages/info/JournalPage.vue'
import SustainabilityPage from '@/pages/info/SustainabilityPage.vue'
import SearchPage from '@/pages/search/SearchPage.vue'
import MainLayout from '@/layouts/MainLayout.vue'
import LotPage from '@/pages/lots/LotPage.vue'
import UserProfilePage from '@/pages/user/UserProfilePage.vue'
import NotFoundPage from '@/pages/errors/NotFoundPage.vue'
import CatalogPage from '@/pages/catalog/CatalogPage.vue'
import CreateLotPage from '@/pages/lots/CreateLotPage.vue'
import MyListingsPage from '@/pages/lots/MyListingsPage.vue'
import EditLotPage from '@/pages/lots/EditLotPage.vue'
import OrdersPage from '@/pages/orders/OrdersPage.vue'
import OrderDetailsPage from '@/pages/orders/OrderDetailsPage.vue'
import DesignersPage from '@/pages/designers/DesignersPage.vue'
import FrequentlyAskedQuestions from '@/pages/info/FrequentlyAskedQuestions.vue'
import SupportPage from '@/pages/info/SupportPage.vue'
import BuyerProtectionPage from '@/pages/info/buy/BuyerProtectionPage.vue'
import ReturnsPolicyPage from '@/pages/info/buy/ReturnsPolicyPage.vue'

function localeHome(locale: AppLocale): string {
  return `/${locale}`
}

const routes = [
  {
    path: '/',
    redirect: () => localeHome(getPreferredLocale()),
  },
  {
    path: '/:locale(en|de|fr)',
    component: RouterView,
    children: [
      {
        path: 'login',
        component: LoginPage,
        meta: { seoKey: 'login' },
      },
      {
        path: 'register',
        component: RegisterPage,
        meta: { seoKey: 'register' },
      },
      {
        path: 'reset-password',
        component: ResetPasswordPage,
        meta: { seoKey: 'resetPassword' },
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordPage,
        meta: { seoKey: 'forgotPassword' },
      },
      {
        path: '',
        component: MainLayout,
        children: [
          {
            path: '',
            component: HomePage,
            meta: { seoKey: 'home' },
          },
          {
            path: 'home',
            redirect: (to: { params: Record<string, unknown> }) => localeHome(
              routeLocale(to.params.locale) ?? getPreferredLocale(),
            ),
          },
          {
            path: 'profile/:username',
            component: UserProfilePage,
            meta: { seoKey: 'profile' },
          },
          {
            path: 'lot/:lotId',
            component: LotPage,
            meta: { seoKey: 'lot' },
          },
          {
            path: 'lots/:id',
            component: LotPage,
            meta: { seoKey: 'lot' },
          },
          {
            path: 'lots/:id/edit',
            component: EditLotPage,
            meta: {
              requiresAuth: true,
              seoKey: 'editLot',
            },
          },
          {
            path: 'about',
            component: AboutPage,
            meta: { seoKey: 'about' },
          },
          {
            path: 'faq',
            component: FrequentlyAskedQuestions,
            meta: { seoKey: 'faq' },
          },
          {
            path: 'contact',
            component: SupportPage,
            meta: { seoKey: 'support' },
          },
          {
            path: 'terms',
            component: TermsPage,
            meta: { seoKey: 'terms' },
          },
          {
            path: 'privacy-policy',
            component: PrivacyPolicyPage,
            meta: { seoKey: 'privacy' },
          },
          {
            path: 'buyer-protection',
            component: BuyerProtectionPage,
            meta: { seoKey: 'buyerProtection' },
          },
          {
            path: 'returns-policy',
            component: ReturnsPolicyPage,
            meta: { seoKey: 'returnsPolicy' },
          },
          {
            path: 'cookie-policy',
            component: CookiePolicyPage,
            meta: { seoKey: 'cookiePolicy' },
          },
          {
            path: 'how-to-sell',
            component: HowToSellPage,
            meta: { seoKey: 'howToSell' },
          },
          {
            path: 'auction-rules',
            component: AuctionRulesPage,
            meta: { seoKey: 'auctionRules' },
          },
          {
            path: 'seller-protection',
            component: SellerProtectionPage,
            meta: { seoKey: 'sellerProtection' },
          },
          {
            path: 'journal',
            component: JournalPage,
            meta: { seoKey: 'journal' },
          },
          {
            path: 'journal/:slug',
            component: JournalPage,
            meta: { seoKey: 'journal' },
          },
          {
            path: 'sustainability',
            component: SustainabilityPage,
            meta: { seoKey: 'sustainability' },
          },
          {
            path: 'brand-partnerships',
            component: BrandPartnershipsPage,
            meta: { seoKey: 'brandPartnerships' },
          },
          {
            path: 'sell',
            component: CreateLotPage,
            meta: {
              requiresAuth: true,
              seoKey: 'sell',
            },
          },
          {
            path: 'listings',
            component: MyListingsPage,
            meta: {
              requiresAuth: true,
              seoKey: 'listings',
            },
          },
          {
            path: 'orders',
            component: OrdersPage,
            meta: {
              requiresAuth: true,
              seoKey: 'orders',
            },
          },
          {
            path: 'orders/:id',
            component: OrderDetailsPage,
            meta: {
              requiresAuth: true,
              seoKey: 'orderDetails',
            },
          },
          // {
          //   path: 'catalog',
          //   name: 'catalog',
          //   component: CatalogPage,
          //   meta: {
          //     requiresAuth: false,
          //     seoKey: 'catalog',
          //   },
          // },
          // {
          //   path: 'auctions',
          //   name: 'auctions',
          //   component: CatalogPage,
          //   meta: {
          //     requiresAuth: false,
          //     seoKey: 'catalog',
          //   },
          // },
          {
            path: 'designers',
            name: 'designers',
            component: DesignersPage,
            meta: {
              requiresAuth: false,
              seoKey: 'designers',
            },
          },
          {
            path: 'designers/:brandSlug',
            name: 'designer-catalog',
            component: CatalogPage,
            meta: {
              requiresAuth: false,
              seoKey: 'catalog',
            },
          },
          {
            path: 'search',
            name: 'search',
            component: SearchPage,
            meta: {
              requiresAuth: false,
              seoKey: 'search',
            },
          },
          {
            path: ':category(bags|shoes|jewellery|vintage|accessories)',
            name: 'catalog-global-category',
            component: CatalogPage,
            meta: { seoKey: 'catalog' },
          },
          {
            path: ':gender(men|women)',
            name: 'catalog-gender',
            component: CatalogPage,
            meta: {
              requiresAuth: false,
              seoKey: 'catalog',
            },
          },
          {
            path: ':gender(men|women)/:category',
            name: 'catalog-category',
            component: CatalogPage,
            meta: {
              requiresAuth: false,
              seoKey: 'catalog',
            },
          },
          {
            path: ':gender(men|women)/:category/:subcategory',
            name: 'catalog-subcategory',
            component: CatalogPage,
            meta: {
              requiresAuth: false,
              seoKey: 'catalog',
            },
          },
          {
            path: ':pathMatch(.*)*',
            component: NotFoundPage,
            meta: { seoKey: 'notFound' },
          },
        ],
      },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    component: NotFoundPage,
    meta: { seoKey: 'notFound' },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach(async (to) => {
  const locale = routeLocale(to.params.locale)

  if (!locale) {
    return {
      path: localizedFullPath(
        to.fullPath,
        getPreferredLocale(),
      ),
      replace: true,
    }
  }

  setLocale(locale)

  const authStore = useAuthStore()

  if (!to.meta.requiresAuth) {
    return true
  }

  if (authStore.isAuthenticated) {
    return true
  }

  const isLoggedIn = await authStore.checkAuth()

  if (isLoggedIn) {
    return true
  }

  return {
    path: `/${locale}/login`,
    query: {
      redirect: to.fullPath,
    },
  }
})

installSeo(router)

export default router
