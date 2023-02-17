import { timetableAPI } from '../../common/axios-common';

export function GetSubjectList() {
  const url = 'Subject/GetSubjectList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddSubject(SubjectData) {
  const url = 'Subject/AddSubject';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, SubjectData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditSubject(SubjectData) {
  const url = 'Subject/EditSubject';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, SubjectData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveSubject(id) {
  const url = 'Subject/RemoveSubject';
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