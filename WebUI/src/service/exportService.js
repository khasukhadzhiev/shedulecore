import { timetableAPI } from '../common/axios-common';

export function SaveTimetableToPdf(saveFileModelDto) {
    const url = 'Export/SaveTimetableToPdf';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, saveFileModelDto,
        {
            responseType: 'arraybuffer',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/pdf'
            }
        })
        .then(response => {
          return resolve(response);
        }).catch(error => {
          return reject(error);
        });
    });
  }

  export function SaveTimetableReportingToPdf(saveFileModelDto) {
    const url = 'Export/SaveTimetableReportingToPdf';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, saveFileModelDto,
        {
            responseType: 'arraybuffer',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/pdf'
            }
        })
        .then(response => {
          return resolve(response);
        }).catch(error => {
          return reject(error);
        });
    });
  }
  
  export function SaveTimetableToXlsx(saveFileModelDto) {
    const url = 'Export/SaveTimetableToXlsx';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, saveFileModelDto,
        {
            responseType: 'arraybuffer',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/pdf'
            }
        })
        .then(response => {
          return resolve(response);
        }).catch(error => {
          return reject(error);
        });
    });
  }

  export function SaveTimetableReportingToXlsx(saveFileModelDto) {
    const url = 'Export/SaveTimetableReportingToXlsx';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, saveFileModelDto,
        {
            responseType: 'arraybuffer',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/pdf'
            }
        })
        .then(response => {
          return resolve(response);
        }).catch(error => {
          return reject(error);
        });
    });
  }