import { timetableAPI } from '../../common/axios-common';

export function GetClassroomList() {
  const url = 'Classroom/GetClassroomList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetClassroomTypeList() {
  const url = 'Classroom/GetClassroomTypeList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}


export function AddClassroom(ClassroomData) {
  const url = 'Classroom/AddClassroom';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, ClassroomData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditClassroom(ClassroomData) {
  const url = 'Classroom/EditClassroom';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, ClassroomData)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function RemoveClassroom(id) {
  const url = 'Classroom/RemoveClassroom';
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


export function SetClassroomByLesson(lessonId, classroomId) {
  const url = 'Classroom/SetClassroomByLesson';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, null, {
      params: {
        lessonId: lessonId,
        classroomId: classroomId
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetWarningsByClassroom(lessonId) {
  const url = 'Classroom/GetWarningsByClassroom';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, null, {
      params: {
        lessonId: lessonId
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetTimetableByClassrooms(selectedClassroomListDto, versionId) {
  const url = 'Classroom/GetTimetableByClassrooms';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, selectedClassroomListDto, {
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