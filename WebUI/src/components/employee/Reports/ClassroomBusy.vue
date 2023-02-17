<template>
  <div class="mt-3 ml-2" v-if="$Role.EmployeeHasRole('employee')">
    <div class="row">
      <div class="col-6 text-left">
        <label class="ml-2">Аудитория</label>
        <multiselect
          v-model="selectedClassroomList"
          :options="classroomList"
          track-by="id"
          :custom-label="customLabel"
          :show-labels="false"
          placeholder="Выбрать аудиторию"
          :multiple="true"
          @input="getTimetableByClassrooms"
        >
          <template slot="noResult">Аудитория не найдена!</template>
          <template slot="noOptions">Список аудиторий пуст!</template>
        </multiselect>
      </div>
      <div class="col-6 text-left">
        <label class="ml-2">Версия</label>
        <multiselect
          v-model="version"
          :options="versionList"
          track-by="id"
          label="name"
          :show-labels="false"
          placeholder="Выбрать версию"
          :multiple="false"
          :preselect-first="true"
          :allow-empty="false"
          @input="getTimetableByClassrooms"
        >
          <template slot="noResult">Версия не найдена!</template>
        </multiselect>
      </div>      
    </div>
    <div class="mt-4">
      <template v-if="queryTimetable.length > 0 && !isLoading">
        <table class="timetable" id="timetable-table">
          <tr id="first-tr">
            <th>День</th>
            <th>Пара</th>
            <th
              style="white-space:nowrap;"
              :colspan="version.useSubClass ? 2 : 1"
              v-for="(timetable, headerIndex) in queryTimetable"
              v-bind:key="headerIndex"
            >{{timetable.name}}</th>
          </tr>
          <tr v-for="(el, rowIndex) in timetableRowArr" v-bind:key="rowIndex">
            <td
              :rowspan="dayLabelCellArr[rowIndex].rowspan"
              v-if="dayLabelCellArr[rowIndex].rowspan > 0"
              class="timetable-border"
            >
              <span class="rotated">{{dayLabelCellArr[rowIndex].label}}</span>
            </td>
            <td
              v-if="lessonLabelCellArr[rowIndex].combineRow != 0"
              :rowspan="lessonLabelCellArr[rowIndex].combineRow"
              :class="{'timetable-border-right':true,'timetable-border-bottom' : lessonLabelCellArr[rowIndex].addBorder}"
            >{{lessonLabelCellArr[rowIndex].label}}</td>
            <template v-for="(timetable, timetableIndex) in queryTimetable">
              <td
                v-if="firstTdVisible(rowIndex, timetableIndex)"
                v-bind:key="timetableIndex"
                :class="{ 
              'td-style':true,
              'timetable-border-right': !version.useSubClass || tdProps(rowIndex, 0, timetableIndex).colspan > 1,
              'timetable-border-bottom':endRowOfDayArr[rowIndex]}"
                :rowIndex="rowIndex"
                :colIndex="0"
                :rowspan="tdProps(rowIndex,0, timetableIndex).rowspan"
                :colspan="tdProps(rowIndex,0, timetableIndex).colspan"
              >
                <template v-for="lesson in getLessonsByIndex(rowIndex, 0, timetableIndex)">
                  <div
                    class="lesson"
                    v-bind:key="lesson.id"
                    :lessonId="lesson.id"
                    :class="{
                  'flow':lesson.flowId > 0,
                  'parallel':lesson.isParallel,
                }"
                  >
                    {{lesson.lessonType.name.substr(0,3)}}. {{lesson.subject.name}}
                    <template>
                      <br />
                      {{lesson.teacher.firstName}} {{lesson.teacher.name}} {{lesson.teacher.middleName}}
                      <br />
                      {{lesson.flow ? lesson.flow.name : lesson.studyClass.name}}
                    </template>
                  </div>
                </template>
              </td>
              <td
                v-if="version.useSubClass 
              && secondTdVisible(rowIndex, timetableIndex)"
                v-bind:key="timetableIndex + 'secondTd'"
                :class="{
              'td-style timetable-border-right': true,
              'timetable-border-bottom' : endRowOfDayArr[rowIndex]}"
                :rowIndex="rowIndex"
                :colIndex="1"
                :rowspan="tdProps(rowIndex,1, timetableIndex).rowspan"
                :colspan="tdProps(rowIndex,1, timetableIndex).colspan"
              >
                <template v-for="lesson in getLessonsByIndex(rowIndex, 1, timetableIndex)">
                  <div
                    class="lesson"
                    v-bind:key="lesson.id"
                    :lessonId="lesson.id"
                    :class="{
                  'flow':lesson.flowId > 0,
                  'parallel':lesson.isParallel,
                }"
                  >
                    {{lesson.lessonType.name.substr(0,3)}}. {{lesson.subject.name}}
                    <template>
                      {{lesson.teacher.firstName}} {{lesson.teacher.name}} {{lesson.teacher.middleName}}
                      <br />
                      {{lesson.studyClass.name}}
                    </template>
                  </div>
                </template>
              </td>
            </template>
          </tr>
        </table>
      </template>
    </div>
  </div>
</template>

<script>
import { GetClassroomList } from "../../../service/timetableDataServices/classroomService";
import { GetVersionList } from "../../../service/versionService";
import { GetTimetableByClassrooms } from "../../../service/timetableDataServices/classroomService";
import { CalculateTable } from "../../../service/timetable/tableCreateService";
import { lessonVolumeEnum } from "../../../enums/lessonVolumeEnum";

export default {
  name: "ClassroomBusy",
  data: function () {
    return {
      classroomList: [],
      selectedClassroomList: [],

      isLoading: false,
      now: "",
      versionList:[],
      version: {},

      //Table creating objects
      timetableRowArr: [],
      lessonLabelCellArr: [],
      endRowOfDayArr: [],
      dayLabelCellArr: [],

      queryTimetable: []
    };
  },
  computed: {},
  components: {},
  methods: {
    customLabel({ buildingDto, name }) {
      if (buildingDto && name) {
        return `${buildingDto.name} ${name}`;
      }
    },    
    getClassroomList() {
      this.isLoading = true;
      GetClassroomList()
        .then((response) => {
          this.classroomList = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getVersionList() {
      GetVersionList()
        .then((response) => {
          this.versionList = response.data;
          this.version = this.versionList.find(l => l.isActive);
          this.createTable();
        })
        .catch(() => {
          this.$ntf.Error("Неудалось получить настройки приложения.", null);
        });
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
    getTimetableByClassrooms() {
      if (this.selectedClassroomList.length === 0) {
        this.clearTimetable();
        return;
      }
      this.isLoading = true;
      GetTimetableByClassrooms(this.selectedClassroomList, this.version.id)
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

    clearTimetable() {
      this.queryTimetable = [];
      this.selectedClassroomList = [];
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
    this.getClassroomList();
    this.getVersionList();
  },
  props: {},
};
</script>

<style scoped>
.btn-width{
  min-width: 5rem;
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
  background-image: url("../../../wwwroot/lesson-bg-img/flow.png");
}

.parallel {
  background-size: 18px;
  background-repeat: no-repeat;
  background-position-x: 99%;
  background-position-y: 97%;
  background-image: url("../../../wwwroot/lesson-bg-img/parallel.png");
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
</style>
