let endRowOfDayArr = [];
let lessonLabelCellArr = [];
let dayLabelCellArr = [];
let lessonCountAtDay = "";
let timetableRowArr = [];
let version = {};
let dayLabels = [
    "Понедельник",
    "Вторник",
    "Среда",
    "Четверг",
    "Пятница",
    "Суббота",
    "Воскресенье"
];

//Формирует все данные для построения таблицы расписания.
export async function CalculateTable(versionProps) {
    endRowOfDayArr = [];
    lessonLabelCellArr = [];
    dayLabelCellArr = [];
    lessonCountAtDay = "";
    timetableRowArr = [];

    version = versionProps;
    let weekDayCount = version.useSunday ? 7 : 6;
    let useSubWeek = version.useSubWeek ? 2 : 1;
    lessonCountAtDay = version.maxLesson * useSubWeek;
    let rowCountPerDay = version.maxLesson * useSubWeek * weekDayCount;
    timetableRowArr = new Array(rowCountPerDay).fill("0");

    for (let i = 0; i < rowCountPerDay; i++) {
        await calculateEndRowOfDay(i);
        await calculateLessonLabelCell(i);
        await calculateDayLabelCell(i);
    }

    let result = {
        timetableRowArr: timetableRowArr,
        endRowOfDayArr: endRowOfDayArr,
        lessonLabelCellArr: lessonLabelCellArr,
        dayLabelCellArr: dayLabelCellArr,
    }

    return result;
}

//Вычисляет является ли строка последней для дня по индексу.
async function calculateEndRowOfDay(rowIndex) {
    let result =
        (rowIndex % lessonCountAtDay) + 1 == lessonCountAtDay
            ? true
            : false;

    endRowOfDayArr.push(result);
}

//Вычисляет Заголовок, Rowspan и Border для ячейки с порядковым номером пары.
async function calculateLessonLabelCell(rowIndex) {
    let result = {
        label: "",
        combineRow: 0,
        addBorder: false
    };

    if (version.useSubWeek) {
        if ((rowIndex + 1) % 2 != 0) {
            let row = (rowIndex % lessonCountAtDay) + 1;
            let label = row % 2 != 0 ? `${row}-${row + 1}` : "";//`${(row + 1) / 2}` : "";

            result.addBorder =
                ((rowIndex + 1) % lessonCountAtDay) + 1 ==
                    lessonCountAtDay
                    ? true
                    : false;
            result.label = label;
            result.combineRow = 2;
        } else {
            result.label = "";
            result.combineRow = 0;
        }
    } else {
        let row = (rowIndex % lessonCountAtDay) + 1;
        row = row +(row -1);
        result.label = `${row}-${row + 1}`;
        result.addBorder =
            (rowIndex % lessonCountAtDay) + 1 == lessonCountAtDay
                ? true
                : false;
        result.combineRow = 1;
    }

    lessonLabelCellArr.push(result);
}

//Вычисляет Заголовок и Rowspan для ячейки с названием дня недели
async function calculateDayLabelCell(rowIndex) {
    let result = {
        rowspan: 0,
        label: ""
    };

    rowIndex += 1;
    let useSubWeek = version.useSubWeek ? 2 : 1;
    let rowCountPerDay = version.maxLesson * useSubWeek;
    let dayIndex = Math.trunc(rowIndex / rowCountPerDay);

    if (rowIndex == 1 || rowIndex % rowCountPerDay === 1) {
        result.rowspan = rowCountPerDay;
        result.label = dayLabels[dayIndex];
    }

    dayLabelCellArr.push(result);
}