import { timetableAPI } from '../../common/axios-common';

export function GetTeacherList() {
  const url = 'Teacher/GetTeacherList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddTeacher(teacherData) {
  const url = 'Teacher/AddTeacher';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, teacherData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditTeacher(teacherData) {
  const url = 'Teacher/EditTeacher';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, teacherData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveTeacher(id) {
  const url = 'Teacher/RemoveTeacher';
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