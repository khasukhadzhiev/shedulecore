import { timetableAPI } from '../common/axios-common';
import { store } from '../store/store';

export const Role = {
  EmployeeHasRole: (roleName) => {
    return store.state.employeeRoles.includes(roleName.trim()) ? true : false;
  }
}

export function GetRoleList() {
  const url = 'Role/GetRoleList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}