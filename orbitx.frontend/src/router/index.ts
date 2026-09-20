import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import SatelliteCatalogModelingView from '../views/satellites/SatelliteCatalogModelingView.vue'
import PrivacyView from '../views/PrivacyView.vue'
import TermsView from '../views/TermsView.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/satellites_modeling',
    name: 'satellites_modeling',
    component: SatelliteCatalogModelingView
  },
  {
    path: '/satellites_modeling/:satellite_id',
    name: 'satellites-modeling',
    component: () => import('../views/satellites/SatelliteModelingView.vue')
  },
  {
    path: '/privacy',
    name: 'privacy',
    component: PrivacyView
  },
  {
    path: '/terms',
    name: 'terms',
    component: TermsView
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
