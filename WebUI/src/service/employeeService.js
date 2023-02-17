import { timetableAPI } from '../common/axios-common';

export function GetEmployeeList() {
  const url = 'Employee/getEmployeeList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditEmployee(employeeData) {
  const url = 'Employee/EditEmployee';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, employeeData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddEmployee(employeeData) {
  const url = 'Employee/AddEmployee';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, employeeData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}


export function RemoveEmployee(id) {
  const url = 'Employee/deleteEmployee';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        id: id
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}