import axios from 'axios';
import { API_URL } from '../constants/APIConstants';
import { Logout } from '../service/accountService';
import { store } from '../store/store';
import { router } from '../common/router-common';

export const timetableAPI = axios.create({
  baseURL: API_URL
});

timetableAPI.interceptors.response.use(undefined, (error) => {
  if (error.response && (error.response.status === 401 || error.response.status === 403)) {
    Logout();
    router.push({ name: "login"});
  }
  return Promise.reject(error.response.data);
});

timetableAPI.interceptors.request.use(function (config) {
  const token = `Bearer ${store.getters.EMPLOYEE_DATA.token}`;
  config.headers.Authorization =  token;
  return config;
});