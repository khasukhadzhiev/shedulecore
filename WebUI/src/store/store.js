import Vue from 'vue';
import Vuex from 'vuex';
import { CheckTokenValid } from '../service/accountService';

Vue.use(Vuex);

export const store = new Vuex.Store({
    state: {
      fullName: localStorage.getItem('employee-fullName') || "",
      token: localStorage.getItem('employee-token') || '',
      employeeRoles: localStorage.getItem('employee-employeeRoles') || [],
      isAutentificated: localStorage.getItem('employee-isAutentificated') ||  false,
      expiration: localStorage.getItem('employee-expiration') || ""
    },

    getters:{
      EMPLOYEE_DATA: state => {
        return {
          fullName: state.fullName,
          token: state.token,
          employeeRoles: state.employeeRoles,
          isAutentificated: state.isAutentificated,
          expiration: state.expiration
        }
      }
    },

    mutations: {
      SET_EMPLOYEE_DATA: (state, employeeData) =>{
        state.fullName = employeeData.fullName;
        state.token = employeeData.token;
        state.employeeRoles = employeeData.employeeRoles;
        state.isAutentificated = employeeData.isAutentificated;
        state.expiration = employeeData.expiration;
      },
      CHECK_TOKEN_VALIDATE:() =>{
        CheckTokenValid();
      }
    },
    
    actions:{
      SET_EMPLOYEE_DATA: (context, employeeData) =>{
        context.commit('SET_EMPLOYEE_DATA', employeeData);
      },
      CHECK_TOKEN_VALIDATE:(context) => {
        context.commit('CHECK_TOKEN_VALIDATE');
      }
    }
  })
  