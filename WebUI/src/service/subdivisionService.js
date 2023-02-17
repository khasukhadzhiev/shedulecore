import { timetableAPI } from '../common/axios-common';

export function GetSubdivisionList() {
  const url = 'Subdivision/GetSubdivisionList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetCurrentEmployeeSubdivisionList() {
  const url = 'Subdivision/GetCurrentEmployeeSubdivisionList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddSubdivision(subdivisionData) {
  const url = 'Subdivision/AddSubdivision';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, subdivisionData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditSubdivision(subdivisionData) {
  const url = 'Subdivision/EditSubdivision';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, subdivisionData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveSubdivision(id) {
  const url = 'Subdivision/RemoveSubdivision';
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