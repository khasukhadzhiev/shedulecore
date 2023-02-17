import { timetableAPI } from '../common/axios-common';
import { store } from '../store/store';
import moment from "moment";

export function Login(accountData) {
  const url = 'account/login';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, accountData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function Logout() {
  let employeedata = {
    token: '',
    employeeRoles: [],
    fullName: "",
    isAutentificated: false,
    expiration: ""
  };
  store.dispatch('SET_EMPLOYEE_DATA', employeedata);
  localStorage.clear();  
}

export function CheckTokenValid(){
  if (localStorage.getItem("employee-expiration") !== null) {
    let expiration = moment(localStorage.getItem("employee-expiration")).format();
    let tokenValidityDate = new Date(expiration);
    let now = new Date();
    if (tokenValidityDate < now) {
      Logout();
    }
  } else {
    Logout();
  }
}