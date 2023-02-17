import { timetableAPI } from '../common/axios-common';

export function GetTimetable(queryString) {
    const url = 'User/GetTimetable';
    return new Promise((resolve, reject) => {
      timetableAPI.get(url, {
        params: {
            queryString: queryString
        }
      })
        .then(response => {
          return resolve(response);
        }).catch(error => {
          return reject(error);
        });
    });
}

export function GetReportingTimetable(queryString) {
  const url = 'User/GetReportingTimetable';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url, {
      params: {
          queryString: queryString
      }
    })
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}

export function GetQueryOptionList() {
  const url = 'User/GetQueryOptionList';
  return new Promise((resolve, reject) => {
    timetableAPI.get(url)
      .then(response => {
        return resolve(response);
      }).catch(error => {
        return reject(error);
      });
  });
}
