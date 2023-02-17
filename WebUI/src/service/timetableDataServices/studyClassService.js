import { timetableAPI } from '../../common/axios-common';

export function GetStudyClassList() {
  const url = 'StudyClass/GetStudyClassList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetStudyClassForUser() {
  const url = 'StudyClass/GetStudyClassForUser';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddStudyClass(StudyClassData) {
  const url = 'StudyClass/AddStudyClass';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, StudyClassData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditStudyClass(StudyClassData) {
  const url = 'StudyClass/EditStudyClass';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, StudyClassData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveStudyClass(id) {
  const url = 'StudyClass/RemoveStudyClass';
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

export function GetClassShiftList() {
  const url = 'StudyClass/GetClassShiftList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetEducationFormList() {
  const url = 'StudyClass/GetEducationFormList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetStudyClassListBySubdivision(subdivisionId) {
  const url = 'StudyClass/GetStudyClassListBySubdivision';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        subdivisionId: subdivisionId
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}
