<template>
  <div class="user-div">
    <div class="text-left child-border ml-1">
      <div>{{ now | fullDate }}</div>
      <div>{{ dayOfWeek }}</div>
      <div v-if="version.useSubWeek">{{ this.week }} неделя</div>
    </div>

    <div class="row mt-4 mb-4">
      <div class="col-5">
        <multiselect
          v-model="queryList"
          :options="optionList"
          :show-labels="false"
          placeholder="ФИО преподавателя / Группа"
          :multiple="true"
        >
          <template slot="noResult"
            >Группа или Преподаватель не найдены!</template
          >
          <template slot="noOptions">Список пуст!</template>
        </multiselect>
      </div>
      <div class="col text-left">
        <b-button
          type="button"
          variant="outline-danger"
          v-if="queryTimetable.length > 0 || queryList.length > 0"
          @click="clearTimetable()"
          >Очистить</b-button
        >
      </div>
    </div>
    <div
      class="row"
      v-if="version.showReportingIds && version.showReportingIds.length > 0"
    >
      <div class="col text-left">
        <b-form-checkbox
          id="show-reporting-check"
          v-model="showReporting"
          name="show-reporting-check"
        >
          Показать расписание отчетностей
        </b-form-checkbox>
      </div>
    </div>
    <template
      v-if="queryTimetable.length > 0 && !showReporting"
      id="timetable-panel"
    >
      <hr class="style-two" />
      <div class="row mb-2">
        <div class="col text-left">
          <b-button
            class="reset-btn mr-2"
            size="sm"
            variant="outline-info"
            @click="saveTimetableToPdf()"
            >Сохранить в PDF</b-button
          >
          <b-button
            class="reset-btn"
            size="sm"
            variant="outline-info"
            @click="saveTimetableToExcel()"
            >Сохранить в Excel</b-button
          >
        </div>
      </div>
      <table class="timetable" id="timetable-table">
        <tr id="first-tr">
          <th>День</th>
          <th>Пара</th>
          <th
            :id="timetable.name + timetable.id"
            style="white-space: nowrap"
            :colspan="version.useSubClass ? 2 : 1"
            v-for="(timetable, headerIndex) in queryTimetable"
            v-bind:key="headerIndex"
          >
            {{ timetable.name }}
            <b-tooltip
              :target="timetable.name + timetable.id"
              triggers="hover"
              v-if="timetable.isStudyClass"
            >
              <ul style="list-style: none; padding: 1px; text-align: left">
                <li>
                  <b>Подразделение </b> :
                  {{
                    timetable.subdivision.name.charAt(0).toUpperCase() +
                    timetable.subdivision.name.slice(1).toLowerCase()
                  }}
                </li>
                <li>
                  <b>Форма обучения </b> :
                  {{
                    timetable.educationForm.name.charAt(0).toUpperCase() +
                    timetable.educationForm.name.slice(1).toLowerCase()
                  }}
                </li>
                <li>
                  <b>Смена обучения</b> :
                  {{
                    timetable.classShift.name.charAt(0).toUpperCase() +
                    timetable.classShift.name.slice(1).toLowerCase()
                  }}
                </li>
                <li><b>Кол. обучающихся</b> : {{ timetable.studentsCount }}</li>
              </ul>
            </b-tooltip>
          </th>
        </tr>
        <tr v-for="(el, rowIndex) in timetableRowArr" v-bind:key="rowIndex">
          <td
            :rowspan="dayLabelCellArr[rowIndex].rowspan"
            v-if="dayLabelCellArr[rowIndex].rowspan > 0"
            class="timetable-border"
          >
            <span class="rotated">{{ dayLabelCellArr[rowIndex].label }}</span>
          </td>
          <td
            v-if="lessonLabelCellArr[rowIndex].combineRow != 0"
            :rowspan="lessonLabelCellArr[rowIndex].combineRow"
            :class="{
              'timetable-border-right': true,
              'timetable-border-bottom': lessonLabelCellArr[rowIndex].addBorder,
            }"
          >
            {{ lessonLabelCellArr[rowIndex].label }}
          </td>
          <template v-for="(timetable, timetableIndex) in queryTimetable">
            <td
              v-if="firstTdVisible(rowIndex, timetableIndex)"
              v-bind:key="timetableIndex"
              :class="{
                'td-style': true,
                'timetable-border-right':
                  !version.useSubClass ||
                  tdProps(rowIndex, 0, timetableIndex).colspan > 1,
                'timetable-border-bottom':
                  endRowOfDayArr[rowIndex] ||
                  (endRowOfDayArr[rowIndex + 1] &&
                    tdProps(rowIndex, 0, timetableIndex).rowspan > 1),
              }"
              :rowIndex="rowIndex"
              :colIndex="0"
              :rowspan="tdProps(rowIndex, 0, timetableIndex).rowspan"
              :colspan="tdProps(rowIndex, 0, timetableIndex).colspan"
            >
              <template
                v-for="lesson in getLessonsByIndex(rowIndex, 0, timetableIndex)"
              >
                <div
                  class="lesson"
                  v-bind:key="lesson.id"
                  :lessonId="lesson.id"
                  :class="{
                    flow: lesson.flowId > 0,
                    parallel: lesson.isParallel,
                  }"
                >
                  {{ lesson.lessonType.name.substr(0, 3) }}.
                  {{ lesson.subject.name }}
                  <template v-if="timetable.isStudyClass">
                    <br />
                    {{ lesson.teacher.fullName }}
                  </template>
                  <template v-else>
                    <br />
                    {{
                      lesson.flow ? lesson.flow.name : lesson.studyClass.name
                    }}
                  </template>
                  <br />
                  {{
                    lesson.classroom
                      ? "Ауд. " +
                        lesson.classroom.buildingDto.name +
                        " " +
                        lesson.classroom.name
                      : "Ауд. не указана"
                  }}
                </div>
              </template>
            </td>
            <td
              v-if="
                version.useSubClass && secondTdVisible(rowIndex, timetableIndex)
              "
              v-bind:key="timetableIndex + 'secondTd'"
              :class="{
                'td-style timetable-border-right': true,
                'timetable-border-bottom':
                  endRowOfDayArr[rowIndex] ||
                  (endRowOfDayArr[rowIndex + 1] &&
                    tdProps(rowIndex, 0, timetableIndex).rowspan > 1),
              }"
              :rowIndex="rowIndex"
              :colIndex="1"
              :rowspan="tdProps(rowIndex, 1, timetableIndex).rowspan"
              :colspan="tdProps(rowIndex, 1, timetableIndex).colspan"
            >
              <template
                v-for="lesson in getLessonsByIndex(rowIndex, 1, timetableIndex)"
              >
                <div
                  class="lesson"
                  v-bind:key="lesson.id"
                  :lessonId="lesson.id"
                  :class="{
                    flow: lesson.flowId > 0,
                    parallel: lesson.isParallel,
                  }"
                >
                  {{ lesson.lessonType.name.substr(0, 3) }}.
                  {{ lesson.subject.name }}
                  <template v-if="timetable.isStudyClass">
                    <br />
                    {{ lesson.teacher.firstName }} {{ lesson.teacher.name }}
                    {{ lesson.teacher.middleName }}
                  </template>
                  <template v-else>
                    <br />
                    {{ lesson.studyClass.name }}
                  </template>
                  <br />
                  {{
                    lesson.classroom
                      ? "Ауд. " +
                        lesson.classroom.buildingDto.name +
                        " " +
                        lesson.classroom.name
                      : "Ауд. не указана"
                  }}
                </div>
              </template>
            </td>
          </template>
        </tr>
      </table>
    </template>
    <template
      id="reporting-panel"
      v-if="queryReportingTimetable.length > 0 && showReporting"
    >
      <hr class="style-two" />
      <div class="row mb-2">
        <div class="col text-left">
          <b-button
            class="reset-btn mr-2"
            size="sm"
            variant="outline-info"
            @click="saveReportingToPdf()"
            >Сохранить в PDF</b-button
          >
          <b-button
            class="reset-btn"
            size="sm"
            variant="outline-info"
            @click="saveTimetableReportingToExcel()"
            >Сохранить в Excel</b-button
          >
        </div>
      </div>
      <table class="reporting-timetable" id="reporting-timetable">
        <tr>
          <th>№</th>
          <th>Группа</th>
          <th>Дисциплина</th>
          <th>Преподаватель</th>
          <th>Отчетность</th>
          <th>Дата</th>
          <th>Аудитория</th>
        </tr>
        <tr v-for="(row, index) in queryReportingTimetable" v-bind:key="row.id">
          <td>{{ index + 1 }}</td>
          <td>{{ row.studyClassName }}</td>
          <td>{{ row.subjectName }}</td>
          <td>{{ row.teacherName }}</td>
          <td>{{ row.reportingTypeName }}</td>
          <td>{{ row.date | shortDate}}</td>
          <td>{{ row.classroomName }}</td>
        </tr>
      </table>
    </template>
    <template class="text-center" v-if="isLoading">
      <b-spinner variant="primary" label="Text Centered"></b-spinner>
      <strong>Загрузка...</strong>
    </template>
    <template class="text-center" v-if="timetableIsEmpty">
      <strong>Указанный Преподаватель или Группа не найдены!</strong>
    </template>
  </div>
</template>

<script>
import moment from "moment";
import { GetActiveVersion } from "../service/versionService";
import {
  GetTimetable,
  GetQueryOptionList,
  GetReportingTimetable,
} from "../service/userService";
import { CalculateTable } from "../service/timetable/tableCreateService";
import { lessonVolumeEnum } from "../enums/lessonVolumeEnum";
import {
  SaveTimetableToPdf,
  SaveTimetableReportingToPdf,
  SaveTimetableToXlsx,
  SaveTimetableReportingToXlsx
} from "../service/exportService";

export default {
  name: "User",
  data: function () {
    return {
      isLoading: false,
      now: "",
      version: {},

      //Table creating objects
      timetableRowArr: [],
      lessonLabelCellArr: [],
      endRowOfDayArr: [],
      dayLabelCellArr: [],

      timetableIsEmpty: false,

      queryTimetable: [],
      queryReportingTimetable: [],
      queryList: [],
      optionList: [],
      showReporting: false,
      fields: [
        { key: "index", label: "№" },
        { key: "studyClassName", label: "Группа", sortable: true },
        { key: "subjectName", label: "Дисциплина", sortable: true },
        { key: "teacherName", label: "Преподаватель", sortable: true },
        { key: "reportingTypeName", label: "Отчетность", sortable: true },
        { key: "date", label: "Дата" },
        { key: "classroomName", label: "Аудитория" },
      ],
    };
  },
  computed: {
    week() {
      return moment(this.now).isoWeek() % 2 == 0 ? 1 : 2;
    },
    dayOfWeek() {
      return moment(this.now).format("dddd");
    },
  },

  methods: {
    getTimetableByStudyClass(e) {
      let item = e.currentTarget.lastChild.textContent.trim();
      this.queryList.push(item);
      this.getTimetable();
    },

    createTable() {
      CalculateTable(this.version)
        .then((data) => {
          this.timetableRowArr = data.timetableRowArr;
          this.endRowOfDayArr = data.endRowOfDayArr;
          this.lessonLabelCellArr = data.lessonLabelCellArr;
          this.dayLabelCellArr = data.dayLabelCellArr;
        })
        .catch(() => {
          this.$ntf.Error(
            "Неудалось сформировать таблицу расписания. Обратитесь к администратору.",
            null
          );
        });
    },
    getActiveVersion() {
      GetActiveVersion()
        .then((response) => {
          this.version = response.data;
          this.createTable();
        })
        .catch(() => {
          this.$ntf.Error("Неудалось получить настройки приложения.", null);
        });
    },
    getTimetable() {
      if (this.queryList.length === 0) {
        this.$ntf.Warn("Необходимо ввесте ФИО преподавателя или Группу.");
        return;
      }
      this.isLoading = true;
      GetTimetable(this.queryList.join())
        .then((response) => {
          this.queryTimetable = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить расписание.", error);
        })
        .finally(() => {
          this.isLoading = false;
          this.timetableIsEmpty = this.queryTimetable.length < 1 ? true : false;
        });
    },

    getReportingTimetable() {
      if (this.queryList.length === 0) {
        this.$ntf.Warn("Необходимо ввесте ФИО преподавателя или Группу.");
        return;
      }
      this.isLoading = true;
      GetReportingTimetable(this.queryList.join())
        .then((response) => {
          this.queryReportingTimetable = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить расписание.", error);
        })
        .finally(() => {
          this.isLoading = false;
          this.timetableIsEmpty =
            this.queryReportingTimetable.length < 1 ? true : false;
        });
    },

    getQueryOptionList() {
      this.isLoading = true;
      GetQueryOptionList()
        .then((response) => {
          this.optionList = response.data;
        })
        .catch((error) => {
          this.$ntf.Error(
            "Неудалось получить список вариантов запроса.",
            error
          );
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    clearTimetable() {
      this.queryTimetable = [];
      this.queryList = [];
    },

    saveTimetableToPdf() {
      let element = document.getElementById("timetable-table");
      let outerHTML = element.outerHTML;

      let pdfSaveModelDto = {
        html: outerHTML,
        name: "Расписание " + this.queryList.join().Length > 150 ? this.queryList.join().Substring(0, 150).toUpperCase() : this.queryList.join().toUpperCase(),
      };

      this.isLoading = true;
      SaveTimetableToPdf(pdfSaveModelDto)
        .then((response) => {
          let blob = new Blob([response.data], { type: "application/pdf" });
          let link = document.createElement("a");
          link.href = window.URL.createObjectURL(blob);
          link.download = pdfSaveModelDto.name.toUpperCase();
          link.click();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить pdf.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    saveReportingToPdf() {
      let element = document.getElementById("reporting-timetable");
      let outerHTML = element.outerHTML;
      let pdfSaveModelDto = {
        html: outerHTML,
        name: "Расписание отчетностей " + this.queryList.join().Length > 50 ? this.queryList.join().Substring(0, 50).toUpperCase() : this.queryList.join().toUpperCase(),
      };

      this.isLoading = true;
      SaveTimetableReportingToPdf(pdfSaveModelDto)
        .then((response) => {
          let blob = new Blob([response.data], { type: "application/pdf" });
          let link = document.createElement("a");
          link.href = window.URL.createObjectURL(blob);
          link.download = pdfSaveModelDto.name.toUpperCase();
          link.click();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить pdf.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    saveTimetableToExcel() {
      let element = document.getElementById("timetable-table");
      let outerHTML = element.outerHTML;

      let xlsxSaveModelDto = {
        html: outerHTML,
        name: "Расписание " + this.queryList.join().Length > 50 ? this.queryList.join().Substring(0, 50).toUpperCase() : this.queryList.join().toUpperCase(),
      };

      this.isLoading = true;
      SaveTimetableToXlsx(xlsxSaveModelDto)
        .then((response) => {
          let blob = new Blob([response.data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
          let link = document.createElement("a");
          link.href = window.URL.createObjectURL(blob);
          link.download = xlsxSaveModelDto.name.toUpperCase();
          link.click();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить Excel файл.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    saveTimetableReportingToExcel() {
      let element = document.getElementById("reporting-timetable");
      let outerHTML = element.outerHTML;

      let xlsxSaveModelDto = {
        html: outerHTML,
        name: "Расписание отчетностей " + this.queryList.join().Length > 150 ? this.queryList.join().Substring(0, 150).toUpperCase() : this.queryList.join().toUpperCase(),
      };

      this.isLoading = true;
      SaveTimetableReportingToXlsx(xlsxSaveModelDto)
        .then((response) => {
          let blob = new Blob([response.data], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
          let link = document.createElement("a");
          link.href = window.URL.createObjectURL(blob);
          link.download = xlsxSaveModelDto.name.toUpperCase();
          link.click();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить Excel файл.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    //Вычислить свойства ячейки
    tdProps(rowIndex, colIndex, timetableIndex) {
      let lesson = this.queryTimetable[timetableIndex].lessonList.find(
        (l) => l.rowIndex == rowIndex && l.colIndex == colIndex
      );

      let result = {
        rowspan: 1,
        colspan: 1,
      };

      if (lesson) {
        if (this.getlessonVolume(lesson) == lessonVolumeEnum.Full) {
          result.rowspan = this.version.useSubWeek ? 2 : 1;
          result.colspan = this.version.useSubClass ? 2 : 1;
        } else if (this.getlessonVolume(lesson) == lessonVolumeEnum.SubWeek) {
          result.rowspan = 1;
          result.colspan = this.version.useSubClass ? 2 : 1;
        } else if (this.getlessonVolume(lesson) == lessonVolumeEnum.SubClass) {
          result.rowspan = this.version.useSubWeek ? 2 : 1;
          result.colspan = 1;
        } else {
          result.rowspan = 1;
          result.colspan = 1;
        }
      }
      return result;
    },

    //Видимость первой ячейки
    firstTdVisible(rowIndex, timetableIndex) {
      let visible = true;

      if (this.tdProps(rowIndex - 1, 0, timetableIndex).rowspan > 1) {
        visible = false;
      }

      return visible;
    },

    //Видимость второй ячейки
    secondTdVisible(rowIndex, timetableIndex) {
      let visible = true;

      if (
        !(this.tdProps(rowIndex - 1, 1, timetableIndex).rowspan > 1) &&
        !(
          this.tdProps(rowIndex - 1, 0, timetableIndex).rowspan > 1 &&
          this.tdProps(rowIndex - 1, 0, timetableIndex).colspan > 1
        ) &&
        this.tdProps(rowIndex, 0, timetableIndex).colspan < 2
      ) {
        visible = true;
      } else {
        visible = false;
      }

      return visible;
    },
    //Получить занятия по индексам
    getLessonsByIndex(rowIndex, colIndex, timetableIndex) {
      let lessonList = this.queryTimetable[timetableIndex].lessonList.filter(
        (l) => l.rowIndex == rowIndex && l.colIndex == colIndex
      );

      var flow = lessonList.find((l) => l.flow != null);

      if (flow) {
        return new Array(1).fill(flow);
      }

      return lessonList;
    },
    //Получить объем занятия
    getlessonVolume(lesson) {
      if (!lesson.isSubClassLesson && !lesson.isSubWeekLesson) {
        return lessonVolumeEnum.Full;
      } else if (!lesson.isSubClassLesson && lesson.isSubWeekLesson) {
        return lessonVolumeEnum.SubWeek;
      } else if (lesson.isSubClassLesson && !lesson.isSubWeekLesson) {
        return lessonVolumeEnum.SubClass;
      } else {
        return lessonVolumeEnum.SubWeekSubClass;
      }
    },
  },

  created() {
    this.getActiveVersion();
    this.getQueryOptionList();
    this.now = new Date();
  },
  props: {},
  watch: {
    queryList: function () {
      if (!this.showReporting) {
        if (this.queryList.length > 0) {
          this.getTimetable();
        } else {
          this.queryTimetable = [];
        }
      } else {
        if (this.queryList.length > 0) {
          this.getReportingTimetable();
        } else {
          this.queryReportingTimetable = [];
        }
      }
    },
    showReporting: function () {
      if (!this.showReporting) {
        if (this.queryList.length > 0) {
          this.getTimetable();
        } else {
          this.queryTimetable = [];
        }
      } else {
        if (this.queryList.length > 0) {
          this.getReportingTimetable();
        } else {
          this.queryReportingTimetable = [];
        }
      }
    },
  },
};
</script>

<style scoped>
.hr-style {
  padding: 0;
  margin: 0.3rem;
}

.btn-width {
  min-width: 7rem;
}

.btn-width:hover .classShift-caption {
  display: block;
}

.classShift-caption {
  display: none;
  font-size: 8pt;
  margin-top: -22px;
  z-index: 50;
  position: static;
  background-color: #0d5a66;
  padding: 3px;
  border-radius: 3px;
}

.user-div {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}

.up-first-letter::first-letter {
  text-transform: uppercase;
}

.search-input {
  width: 24rem;
}

.child-border div {
  font-size: 0.8rem;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #a2acba;
  display: inline-block;
  margin-right: 1rem;
}
.flow {
  background-size: 18px;
  background-repeat: no-repeat;
  background-position-x: 99%;
  background-position-y: 97%;
  background-image: url("../wwwroot/lesson-bg-img/flow.png");
}

.parallel {
  background-size: 18px;
  background-repeat: no-repeat;
  background-position-x: 99%;
  background-position-y: 97%;
  background-image: url("../wwwroot/lesson-bg-img/parallel.png");
}
.timetable th {
  border: 2px solid #b0b4b8;
  padding: 3px;
}
.timetable th:nth-child(1) {
  border: 2px solid #b0b4b8;
  padding: 3px;
  max-width: 50px;
}
.timetable th:nth-child(3) {
  width: 220px;
}
.timetable tr,
td {
  border: 1px solid #ced4da;
  padding: 3px;
}

.timetable tr {
  height: 2rem;
}

.td-style {
  max-width: 10rem;
  width: 10rem;
  min-width: 10rem;
  height: 0.5rem;
  font-size: 12px;
}

.rotated {
  display: block;
  position: relative;
  max-width: 50px;
  -ms-transform: rotate(270deg); /* IE 9 */
  -webkit-transform: rotate(270deg); /* Chrome, Safari, Opera */
  transform: rotate(270deg);
}

.timetable-border {
  border: 2px solid #b0b4b8;
}
.timetable-border-right {
  border-right: 2px solid #b0b4b8;
}
.timetable-border-bottom {
  border-bottom: 2px solid #b0b4b8;
}

.reporting-timetable > tr > th {
  text-align: center;
  border-spacing: 0px;
  border-collapse: collapse;
  border: 1px solid rgb(105, 105, 105);  
  padding: 10px;
  font-size: 14pt;
  white-space: nowrap;
}

.reporting-timetable > tr > td {
  border: 1px solid rgb(105, 105, 105);
  padding: 10px;
}

.reporting-timetable > tr > td:nth-child(2) {
  text-align: center;
  white-space: nowrap;
}

.reporting-timetable > td:nth-child(6) {
  text-align: center;
  white-space: nowrap;
}

.reporting-timetable > td:nth-child(7) {
  text-align: center;
  white-space: nowrap;
}

.reporting-timetable > tr > td:nth-child(3) {
  text-align: left;
}
.reporting-timetable > tr > td:nth-child(4) {
  text-align: left;
}
</style>
