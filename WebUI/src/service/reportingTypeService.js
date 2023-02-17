import { timetableAPI } from '../common/axios-common';

export function GetReportingTypeList() {
  const url = 'StudyClassReporting/GetReportingTypeList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetStudyClassReportingList(studyClassId, versionId) {
  const url = 'StudyClassReporting/GetStudyClassReportingList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        studyClassId: studyClassId,
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

export function AddStudyClassReporting(studyClassReporting) {
  const url = 'StudyClassReporting/AddStudyClassReporting';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, studyClassReporting)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditStudyClassReporting(studyClassReporting) {
  const url = 'StudyClassReporting/EditStudyClassReporting';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, studyClassReporting)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveStudyClassReporting(id) {
  const url = 'StudyClassReporting/RemoveStudyClassReporting';
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