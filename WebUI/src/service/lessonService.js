import { timetableAPI } from '../common/axios-common';
let dayLabels = [
  "Понедельник",
  "Вторник",
  "Среда",
  "Четверг",
  "Пятница",
  "Суббота",
  "Воскресенье"
];
let lessonNumberLabels = [
  "Первая",
  "Вторая",
  "Третья",
  "Четвертая",
  "Пятая",
  "Шестая",
  "Седьмая",
  "Восьмая",
  "Девятая",
  "Десятая",
  "Одинадцатая",
  "Двенадцатая",
  "Тринадцатая",
  "Четырнадцатая",
  "Пятнадцатая",
];


export function GetMainLessonList(studyClassId, versionId) {
  const url = 'Lesson/GetMainLessonList';
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

export function GetFlowLessonList(studyClassId, versionId) {
  const url = 'Lesson/GetFlowLessonList';
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

export function GetParallelLessonList(studyClassId, versionId) {
  const url = 'Lesson/GetParallelLessonList';
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

export function GetLessonTypeList() {
  const url = 'Lesson/GetLessonTypeList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function AddMainLesson(lesson, versionId) {
  const url = 'Lesson/AddLesson';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, lesson, {
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

export function AddFlowLesson(lesson, versionId) {
  const url = 'Lesson/AddFlow';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, lesson, {
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

export function AddParallelLesson(lesson, versionId) {
  const url = 'Lesson/AddParallel';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, lesson, {
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

export function RemoveLesson(lessonId) {
  const url = 'Lesson/RemoveLesson';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
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

export function GetLessonList(studyClassId, versionId) {
  const url = 'Lesson/GetLessonList';
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

export function LessonSet(lesson) {
  const url = 'Lesson/LessonSet';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, lesson)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function ResetAllLessons(studyClassId) {
  const url = 'Lesson/ResetAllLessons';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        studyClassId: studyClassId
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function EditLessonData(lesson, versionId) {
  const url = 'Lesson/EditLessonData';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, lesson, {
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

export function GetMainLessonFilterList(studyClassId, versionId, filter) {
  const url = 'Lesson/GetMainLessonFilterList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        studyClassId: studyClassId,
        versionId: versionId,
        filter: filter
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetFlowLessonFilterList(studyClassId, versionId, filter) {
  const url = 'Lesson/GetFlowLessonFilterList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        studyClassId: studyClassId,
        versionId: versionId,
        filter: filter
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetParallelLessonFilterList(studyClassId, versionId, filter) {
  const url = 'Lesson/GetParallelLessonFilterList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
        studyClassId: studyClassId,
        versionId: versionId,
        filter: filter
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function DiscriptLessonDayAndNumber(lessonDay, lessonNumber){
  if(lessonDay < 7 && lessonNumber <15){
    return dayLabels[lessonDay]+ ", " + lessonNumberLabels[lessonNumber] + " пара"
  }
}
