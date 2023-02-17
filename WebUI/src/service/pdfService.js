import { timetableAPI } from '../common/axios-common';

export function SaveTimetableToPdf(pdfSaveModelDto) {
    const url = 'Pdf/SaveTimetableToPdf';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, pdfSaveModelDto,
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

  export function SaveTimetableReportingToPdf(pdfSaveModelDto) {
    const url = 'Pdf/SaveTimetableReportingToPdf';
    return new Promise((resolve, reject) => {
      timetableAPI.post(url, pdfSaveModelDto,
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
  