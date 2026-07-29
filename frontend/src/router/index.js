import { createRouter, createWebHistory } from 'vue-router'
import StudentLayout from '@/layouts/StudentLayout.vue'
import OperatorLayout from '@/layouts/OperatorLayout.vue'
import StudentServicesView from '@/views/StudentServicesView.vue'
import StudentTicketView from '@/views/StudentTicketView.vue'
import OperatorTicketView from '@/views/OperatorTicketView.vue'
import OperatorHistoryView from '@/views/OperatorHistoryView.vue'
import OperatorServicesView from '@/views/OperatorServicesView.vue'
import OperatorSessionStartView from '@/views/OperatorSessionStartView.vue'
import PanelView from '@/views/PanelView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/student/services'
    },
    {
      path: '/student',
      component: StudentLayout,
      children: [
        {
          path: 'services',
          component: StudentServicesView
        },
        {
          path: 'ticket',
          component: StudentTicketView
        }
      ]
    },
    {
      path: '/operator',
      component: OperatorLayout,
      children: [
        {
          path: '',
          redirect: '/operator/queue',
        },
        {
          path: 'start',
          component: OperatorSessionStartView,
        },
        {
          path: 'queue',
          component: OperatorTicketView,
        },
        {
          path: 'history',
          component: OperatorHistoryView,
        },
        {
          path: 'services',
          component: OperatorServicesView,
        },
      ],
    },
    {
      path: '/panel',
      component: PanelView,
    }
  ],
})

export default router
