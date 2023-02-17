import Vue from 'vue';
import App from './App.vue';
import { BootstrapVue, BootstrapVueIcons } from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import 'bootstrap/dist/js/bootstrap.min.js'
import 'vue-multiselect/dist/vue-multiselect.min.css';
import './filters/DateFilters';
import { store } from './store/store';
import { router } from './common/router-common';
import { Role } from './service/roleService';
import { ntf } from './service/notifyService';
import Notifications from 'vue-notification';
import Multiselect from 'vue-multiselect';
import { Datetime } from 'vue-datetime';
import 'vue-datetime/dist/vue-datetime.css'
 
Vue.component('datetime', Datetime);
Vue.component('multiselect', Multiselect)
Vue.use(Notifications); 
Vue.use(BootstrapVue);
Vue.use(BootstrapVueIcons);
Vue.config.productionTip = false;
Vue.prototype.$Role = Role;
Vue.prototype.$ntf = ntf;

new Vue({
  render: h => h(App),
  router,
  store
}).$mount('#app');
