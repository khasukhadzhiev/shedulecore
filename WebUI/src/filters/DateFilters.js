import Vue from "vue";
import moment from 'moment';

moment.locale('ru');

Vue.filter('fullDate', (date) => {
    return moment(date).format('LLL'); //14 января 2020 г., 19:59
});

Vue.filter('shortDate', (date) => {
    if (date) {
        return moment(date).format('L');
    }
    else {
        return '';
    }
});