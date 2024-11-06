<template>
  <div id="app">
    <nav class="navbar navbar-expand-sm navbar-light navbar-style">
      <button
        class="navbar-toggler collapsed"
        type="button"
        data-toggle="collapse"
        data-target="#navbarToggler"
        aria-expanded="false"
        aria-controls="navbar"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <router-link to="/user" class="navbar-brand">
        <img src="./wwwroot/logochesu.png" width="130" />
      </router-link>

      <button
        class="btn btn-icon btn-transparent-dark order-1 order-lg-0 mr-lg-2 btn-feather"
        id="sidebarToggle"
        href="#"
        v-if="isAutentificated"
        @click="hideSidebar = !hideSidebar"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="feather"
        >
          <line x1="3" y1="12" x2="21" y2="12" />
          <line x1="3" y1="6" x2="21" y2="6" />
          <line x1="3" y1="18" x2="21" y2="18" />
        </svg>
      </button>

      <div class="navbar-collapse collapse" id="navbarToggler">
        <ul class="nav navbar-nav mr-auto mt-2">
          <li class="nav-item active">
            <router-link :to="guestRouteEnum.user" class="nav-link">Главная</router-link>
          </li>
          <li class="nav-item">
            <router-link :to="guestRouteEnum.contacts" class="nav-link">Контакты</router-link>
          </li>
        </ul>
        <ul class="navbar-nav mt-2">
          <li class="nav-item" v-if="!isAutentificated">
            <router-link :to="guestRouteEnum.login" class="nav-link">Вход</router-link>
          </li>
          <li class="nav-item" v-if="isAutentificated" @click="logoutemployee()">
            <router-link :to="guestRouteEnum.login" class="nav-link">Выйти</router-link>
          </li>
        </ul>
      </div>
    </nav>

    <div>
      <div
        :class="{
        'col sidebar col':true,
        'hide-sidebar': hideSidebar
        }"
        v-if="isAutentificated"
      >
        <template v-if="$Role.EmployeeHasRole('admin')">
          <div class="heading">МЕНЮ АДМИНИСТРАТОРА</div>
          <div class="menu-admin">
            <ul>
              <li>
                <router-link :to="adminRouteEnum.subdivision">Подразделения</router-link>
              </li>
              <li>
                <router-link :to="adminRouteEnum.employeeList">Сотрудники</router-link>
              </li>
              <li>
                <router-link :to="adminRouteEnum.import">Импорт</router-link>
              </li>
              <li>
                <router-link :to="adminRouteEnum.dataManage">Управление нагрузкой</router-link>
              </li>
              <li>
                <router-link :to="adminRouteEnum.version">Версии расписания</router-link>
              </li>
            </ul>
          </div>
        </template>

        <template v-if="$Role.EmployeeHasRole('employee')">
          <div class="heading">МЕНЮ СОТРУДНИКА</div>
          <div class="menu-employee">
            <ul>
              <li>
                <router-link :to="employeeRouteEnum.timetableManual">Расписание занятий</router-link>
              </li>
              <li>
                <router-link :to="employeeRouteEnum.studyClassReporting">Расписание отчетностей</router-link>
              </li>              
              <li>
                <router-link :to="employeeRouteEnum.lessonManage">Управление занятиями</router-link>
              </li>
              <li>
                <router-link :to="employeeRouteEnum.timetableData">Данные расписания</router-link>
              </li>
              <li>
                <router-link :to="employeeRouteEnum.reports">Отчеты</router-link>
              </li>
            </ul>
          </div>
          <div class="footer">
            <div class="heading">СОТРУДНИК</div>
            <div class="logged-name">{{shortname}}</div>
          </div>
        </template>
      </div>

      <div
        :class="{
        'content-view-style':true,
        'content-add-margin': !hideSidebar && isAutentificated,
        'content-remove-margin': hideSidebar
        }"
      >
        <router-view></router-view>
      </div>
    </div>

    <notifications />
  </div>
</template>

<script>
import { Logout } from "./service/accountService";
import {
  GuestRouteEnum,
  EmployeeRouteEnum,
  AdminRouteEnum
} from "./enums/routeEnum";

export default {
  name: "app",
  components: {},
  data: function() {
    return {
      guestRouteEnum: GuestRouteEnum,
      employeeRouteEnum: EmployeeRouteEnum,
      adminRouteEnum: AdminRouteEnum,
      hideSidebar: false
    };
  },
  computed: {
    isAutentificated() {
      return this.$store.getters.EMPLOYEE_DATA.isAutentificated;
    },
    fullname() {
      return this.$store.getters.EMPLOYEE_DATA.fullName;
    },
    shortname() {
      return this.$store.getters.EMPLOYEE_DATA.fullName
        .trim()
        .split(" ")
        .reduce(
          (res, current, i) =>
            i > 0 ? (res += current[0] + ".") : current + " ",
          ""
        );
    }
  },
  methods: {
    logoutemployee() {
      Logout();
    }
  },
  mounted() {
    this.$store.dispatch("CHECK_TOKEN_VALIDATE");
  }
};
</script>

<style scoped>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.content-view-style {
  padding-top: 4rem;
  height: 100vh;
}

.content-padding {
  padding: 1rem;
}

.navbar-style {
  z-index: 1000;
  background-color: white;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  position: fixed;
  left: 0;
  top: 0;
  width: 100%;
}
.navbar-brand {
  width: 12rem;
}

.nav-link {
  margin-top: -0.4rem;
  font-size: 0.8rem;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #a2acba;
}

.btn-feather {
  padding: 0;
  -webkit-box-pack: center;
  justify-content: center;
  overflow: hidden;
  border-radius: 100%;
  flex-shrink: 0;
  height: calc((1rem * 1.5) + (0.5rem * 2) + (2px)) !important;
  width: calc((1rem * 1.5) + (0.5rem * 2) + (2px)) !important;
}
.btn-feather:hover {
  background-color: rgba(31, 45, 65, 0.1);
}
.feather {
  color: #fff;
  background-color: transparent;
  border-color: transparent;
  color: rgba(31, 45, 65, 0.5) !important;
  height: 1rem;
  width: 1rem;
}

.menu-admin ul,
.menu-employee ul {
  list-style-type: none;
  margin: 0;
  padding-left: 1rem;
}

.menu-admin ul li,
.menu-employee ul li {
  display: flex;
  -webkit-box-align: center;
  align-items: center;
  line-height: normal;
  padding-top: 0.5rem;
  padding-bottom: 0.5rem;
  position: relative;
}

.menu-admin ul li a,
.menu-employee ul li a {
  color: #474f5c;
  text-decoration: none;
}

.menu-admin ul li a:hover,
.menu-employee ul li a:hover {
  color: #0061f2;
}

.hide-sidebar {
  margin-left: -15rem;
  transition: 0.3s;
}

.content-add-margin {
  margin-left: 15rem;
  transition: 0.3s;
}
.content-remove-margin {
  margin-left: 0rem;
  transition: 0.3s;
}

.sidebar {
  width: 15rem;
  position: fixed;
  background-color: white;
  top: 0;
  padding: 5rem 0 0 0;
  height: 100%;
  z-index: 20;
  transition: 0.3s;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
}

.sidebar .heading {
  text-align: left;
  padding: 1.75rem 1rem 0.75rem;
  font-size: 0.7rem;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #a2acba;
}

.footer {
  height: 6rem;
  width: 100%;
  position: absolute;
  bottom: 0;
  background-color: rgba(31, 45, 65, 0.05);
}

.logged-name {
  text-align: center;
}
</style>
