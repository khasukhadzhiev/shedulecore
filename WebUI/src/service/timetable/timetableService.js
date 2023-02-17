import { timetableAPI } from '../../common/axios-common';

export function GetMistakesByStudyClass(studyClassId, versionId) {
  const url = 'Timetable/GetMistakesByStudyClass';
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

export function GetStudyClassNamesWithMistakes() {
  const url = 'Timetable/GetStudyClassNamesWithMistakes';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GenerateTimetable(geneticAlgorithmDataDto) {
  const url = 'Timetable/GenerateTimetable';
  return new Promise((resolve, reject) => {
    timetableAPI.post(url, geneticAlgorithmDataDto)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetTimetableGenerateProgress() {
  const url = 'Timetable/GetTimetableGenerateProgress';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function StopTimetableGenerate() {
  const url = 'Timetable/StopTimetableGenerate';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function SaveTimetableWithMistakes() {
  const url = 'Timetable/SaveTimetableWithMistakes';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}