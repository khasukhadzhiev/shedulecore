import Vue from 'vue';
import VueRouter from 'vue-router';

import Login from '../components/Login';
import User from '../components/User';
import NotFound from '../components/NotFound';
import Contacts from '../components/Contacts';
import DataImport from '../components/admin/DataImport';
import Version from '../components/admin/Version';
import DataManage from '../components/admin/DataManage';
import EmployeeList from '../components/admin/EmployeeList';
import LessonManage from '../components/employee/LessonManage';
import StudyClassReporting from '../components/employee/StudyClassReporting';
import Subdivision from '../components/admin/Subdivision';
import Reports from '../components/employee/Reports';
import StudyClassListBySubdivision from '../components/employee/TimetableManual/StudyClassListBySubdivision';
import TimetableManual from '../components/employee/TimetableManual';
import StudyClassTimetable from '../components/employee/TimetableManual/StudyClassTimetable';
import { Role } from '../enums/roleEnum';
import { GuestRouteEnum, AdminRouteEnum, EmployeeRouteEnum } from '../enums/routeEnum';
import TimetableData from '../components/employee/TimetableData';
import {CheckTokenValid} from '../service/accountService';



Vue.use(VueRouter);

export const router = new VueRouter({
  mode: 'history',
  routes: [
    //guest route
    { path: `${GuestRouteEnum.login}`, name: "login", component: Login },
    { path: `${GuestRouteEnum.user}`, name: "user", component: User },
    { path: `${GuestRouteEnum.contacts}`, name: "contacts", component: Contacts },
    { path: `${GuestRouteEnum.userSharp}`, name: "userSharp", component: User },
    { path: `${GuestRouteEnum.userSlash}`, name: "userSlash", component: User },

    //admin route
    { path: `${AdminRouteEnum.dataImport}`, name: "dataImport", component: DataImport, meta: { authorize: [Role.Admin] } },
    { path: `${AdminRouteEnum.dataManage}`, name: "dataManage", component: DataManage, meta: { authorize: [Role.Admin] } },
    { path: `${AdminRouteEnum.employeeList}`, name: "employeeList", component: EmployeeList, meta: { authorize: [Role.Admin] } },
    { path: `${AdminRouteEnum.version}`, name: "version", component: Version, meta: { authorize: [Role.Admin] } },
    { path: `${AdminRouteEnum.subdivision}`, name: "subdivision", component: Subdivision, meta: { authorize: [Role.Admin] } },

    //employee route
    { path: `${EmployeeRouteEnum.studyClassTimetable}`, name: "studyClassTimetable", component: StudyClassTimetable, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.lessonManage}`, name: "lessonManage", component: LessonManage, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.studyClassReporting}`, name: "studyClassReporting", component: StudyClassReporting, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.reports}`, name: "reports", component: Reports, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.timetableManual}`, name: "timetableManual", component: TimetableManual, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.timetableData}`, name: "timetableData", component: TimetableData, meta: { authorize: [Role.Employee] } },
    { path: `${EmployeeRouteEnum.studyClassListBySubdivision}`, name: "studyClassListBySubdivision", component: StudyClassListBySubdivision, meta: { authorize: [Role.Employee] } },

    //not found route
    { path: `${GuestRouteEnum.notFound}`, name: "not-found", component: NotFound }
  ]
});


router.beforeEach((to, from, next) => {
  const vm = router.app;
  const { authorize } = to.meta;
  let guestRouteArr = Object.values(GuestRouteEnum);
  let employeeRoles = vm.$store.getters.EMPLOYEE_DATA.employeeRoles;
  CheckTokenValid();
  if (!guestRouteArr.includes(to.matched[0].path)) {
    if (vm.$store.getters.EMPLOYEE_DATA.isAutentificated) {

      if (authorize) {
        const hasRole = authorize.filter((role) => { return employeeRoles.includes(role) });
        if (authorize.length && hasRole.length === 0) {
          vm.$notify({
            type: 'warn',
            title: 'Уважаемый пользователь.',
            text: 'К сожалению у Вас нет прав досутпа для открытия этого раздела приложения.',
            duration: 7000
          });
          return next({ path: `${GuestRouteEnum.user}` });
        }
      }
      next();
    } else {
      vm.$notify({
        type: 'warn',
        title: 'Уважаемый пользователь.',
        text: 'К сожалению у Вас нет прав доступа для открытия этого раздела приложения или срок действия токена истек. Пожалуйста авторизируйтесь.',
        duration: 7000
      });
      next('login');
    }
  } else {
    next();
  }
});