import { timetableAPI } from '../../common/axios-common';

export function GetBuildingList() {
  const url = 'Building/GetBuildingList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddBuilding(buildingData) {
  const url = 'Building/AddBuilding';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, buildingData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditBuilding(buildingData) {
  const url = 'Building/EditBuilding';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, buildingData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveBuilding(id) {
  const url = 'Building/RemoveBuilding';
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