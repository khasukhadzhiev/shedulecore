import { timetableAPI } from '../common/axios-common';

export function ImportTimetableData(importingFile, versionId) {
  const url = 'Import/ImportTimetableData';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, importingFile, {
      params:{
        versionId: versionId
      },
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetImportProgress() {
  const url = 'Import/GetImportProgress';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveImportProgress() {
  const url = 'Import/RemoveImportProgress';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}