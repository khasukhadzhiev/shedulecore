import Vue from 'vue';

const vm = new Vue();
const duration = 10000;
//type: success,error,warn
export const ntf = {
    Success: (txt) => {
        vm.$notify({
            type: "success",
            title: "Успех!",
            text: `${txt}`,
            duration: duration 
          });
    },
    Error: (txt, error) => {
        vm.$notify({
            type: "error",
            title: "Ошибка!",
            text: error == null ? `${txt}` : `${txt}: ${error}`,
            duration: duration 
          });
    },    
    Warn: (txt) => {
        vm.$notify({
            type: "warn",
            title: "Внимание!",
            text: `${txt}`,
            duration: duration 
          });
    },      
  }