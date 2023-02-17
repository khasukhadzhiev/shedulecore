import { timetableAPI } from '../common/axios-common';

export function GetVersionList() {
  const url = 'Version/GetVersionList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddVersion(timetableVersion) {
  const url = 'Version/AddVersion';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, timetableVersion)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditVersion(timetableVersion) {
  const url = 'Version/EditVersion';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, timetableVersion)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveVersion(id) {
  const url = 'Version/RemoveVersion';
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

export function GetActiveVersion() {
  const url = 'Version/GetActiveVersion';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetVersion(versionId) {
  const url = 'Version/GetVersion';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        versionId: versionId
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

