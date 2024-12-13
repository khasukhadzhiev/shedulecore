<template>
  <div
    class="studyclass-timetable-content"
    v-if="$Role.EmployeeHasRole('employee')"
  >
    <h5 class="mt-2 text-left">Расписание {{ studyClass.name }}</h5>
    <div v-if="timetableGenerateProgress.start">
      <h5>В данный момент запущена автогенерация расписания. Необходимо дождаться остановки процесса генерации для внесения изменений.</h5>
    </div>    
    <template v-if="!timetableGenerateProgress.start">
    <div class="row">
      <div class="col-5 text-left">
        <div class="mr-3">Версия расписания</div>
        <multiselect
          v-model="version"
          :options="versionList"
          :allowEmpty="false"
          track-by="id"
          label="name"
          :show-labels="false"
          placeholder="Выбрать версию расписания"
          :multiple="false"
        >
          <template slot="noResult">Версия не найдена!</template>
        </multiselect>
      </div>
    </div>
    <hr />      
    <div v-if="!isLoading">
      <div>
        <div class="row mb-2" v-if="setLessonList.length > 0">
          <div class="col text-left">
            <b-button
              class="reset-btn"
              size="sm"
              variant="outline-warning"
              @click="resetAllLessons()"
              >Сбросить расписание</b-button
            >
          </div>
        </div>

        <table class="timetable">
          <tr>
            <th>День</th>
            <th>Пара</th>
            <th
              :id="studyClass.name + studyClass.id"
              :colspan="version.useSubClass ? 2 : 1"
            >
              {{ studyClass.name }}
              <b-tooltip
                :target="studyClass.name + studyClass.id"
                triggers="hover"
              >
                <ul style="list-style: none; padding: 1px; text-align: left">
                  <li>
                    <b>Подразделение </b> :
                    {{
                      studyClass.subdivision.name.charAt(0).toUpperCase() +
                      studyClass.subdivision.name.slice(1).toLowerCase()
                    }}
                  </li>
                  <li>
                    <b>Форма обучения </b> :
                    {{
                      studyClass.educationForm.name.charAt(0).toUpperCase() +
                      studyClass.educationForm.name.slice(1).toLowerCase()
                    }}
                  </li>
                  <li>
                    <b>Смена обучения</b> :
                    {{
                      studyClass.classShift.name.charAt(0).toUpperCase() +
                      studyClass.classShift.name.slice(1).toLowerCase()
                    }}
                  </li>
                  <li>
                    <b>Кол. обучающихся</b> : {{ studyClass.studentsCount }}
                  </li>
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
                'timetable-border-bottom':
                  lessonLabelCellArr[rowIndex].addBorder,
              }"
            >
              {{ lessonLabelCellArr[rowIndex].label }}
            </td>
            <td
              v-if="firstTdVisible(rowIndex)"
              :class="{
                'dropzone td-style': true,
                'timetable-border-right':
                  !version.useSubClass || tdProps(rowIndex, 0).colspan > 1,
                'timetable-border-bottom':
                  endRowOfDayArr[rowIndex] ||
                  (endRowOfDayArr[rowIndex + 1] &&
                    tdProps(rowIndex, 0).rowspan > 1),
              }"
              :rowIndex="rowIndex"
              :colIndex="0"
              :rowspan="tdProps(rowIndex, 0).rowspan"
              :colspan="tdProps(rowIndex, 0).colspan"
              @dragenter="addBackgroundTooltip"
              @drop="lessonDropToTD"
              @dragover="lessonOverTarget"
              @dragleave="removeBackgroundTooltip"
            >
              <template v-for="lesson in getLessonsByIndex(rowIndex, 0)">
                <div
                  class="lesson"
                  draggable="true"
                  v-bind:key="lesson.id"
                  :lessonId="lesson.id"
                  @dragstart="lessonDragStart"
                  :class="{
                    'lesson-full':
                      !lesson.isSubClassLesson && !lesson.isSubWeekLesson,
                    'lesson-subclass':
                      lesson.isSubClassLesson && !lesson.isSubWeekLesson,
                    'lesson-subWeek': lesson.isSubWeekLesson,
                    'lesson-subclass-subWeek':
                      lesson.isSubClassLesson && lesson.isSubWeekLesson,
                    flow: lesson.flowId > 0,
                    parallel: lesson.isParallel,
                  }"
                >
                  <span
                    v-if="lesson.flowStudyClassNames"
                    class="studyClassNames"
                    >{{ lesson.flowStudyClassNames }}</span
                  >
                  {{ lesson.lessonType.name.substr(0, 3) }}.
                  {{ lesson.subject.name }}
                  <br />
                  {{ lesson.teacher.fullName }}
                  <template>
                    <br />
                    <b-form-select
                      class="classroom-select"
                      v-model="lesson.classroomId"
                      value-field="id"
                      text-field="name"
                      @change="
                        setClassroomByLesson(lesson.id, lesson.classroomId)
                      "
                      size="sm"
                    >
                      <option
                        v-for="(option, idx) in classroomList"
                        :key="idx"
                        :value="option.id"
                        :title="option.title || null"
                      >
                        {{ option.buildingDto.name }} {{ option.name }}
                      </option>
                      <template v-slot:first>
                        <b-form-select-option :value="null" disabled
                          >- - -</b-form-select-option
                        >
                      </template>
                    </b-form-select>
                  </template>
                </div>
              </template>
            </td>
            <td
              v-if="version.useSubClass && secondTdVisible(rowIndex)"
              :class="{
                'dropzone timetable-border-right td-style': true,
                'timetable-border-bottom':
                  endRowOfDayArr[rowIndex] ||
                  (endRowOfDayArr[rowIndex + 1] &&
                    tdProps(rowIndex, 0).rowspan > 1),
              }"
              :rowIndex="rowIndex"
              :colIndex="1"
              :rowspan="tdProps(rowIndex, 1).rowspan"
              :colspan="tdProps(rowIndex, 1).colspan"
              @dragenter="addBackgroundTooltip"
              @drop="lessonDropToTD"
              @dragover="lessonOverTarget"
              @dragleave="removeBackgroundTooltip"
            >
              <template v-for="lesson in getLessonsByIndex(rowIndex, 1)">
                <div
                  class="lesson"
                  draggable="true"
                  v-bind:key="lesson.id"
                  :lessonId="lesson.id"
                  @dragstart="lessonDragStart"
                  :class="{
                    'lesson-full':
                      !lesson.isSubClassLesson && !lesson.isSubWeekLesson,
                    'lesson-subclass':
                      lesson.isSubClassLesson && !lesson.isSubWeekLesson,
                    'lesson-subWeek': lesson.isSubWeekLesson,
                    'lesson-subclass-subWeek':
                      lesson.isSubClassLesson && lesson.isSubWeekLesson,
                    flow: lesson.flowId > 0,
                    parallel: lesson.isParallel,
                  }"
                >
                  <span
                    v-if="lesson.flowStudyClassNames"
                    class="studyClassNames"
                    >{{ lesson.flowStudyClassNames }}</span
                  >
                  {{ lesson.lessonType.name.substr(0, 3) }}.
                  {{ lesson.subject.name }}
                  <br />
                  {{ lesson.teacher.firstName }} {{ lesson.teacher.name }}
                  {{ lesson.teacher.middleName }}
                  <template>
                    <br />
                    <b-form-select
                      class="classroom-select"
                      v-model="lesson.classroomId"
                      value-field="id"
                      text-field="name"
                      @change="
                        setClassroomByLesson(lesson.id, lesson.classroomId)
                      "
                      size="sm"
                    >
                      <option
                        v-for="(option, idx) in classroomList"
                        :key="idx"
                        :value="option.id"
                        :title="option.title || null"
                      >
                        {{ option.buildingDto.name }} {{ option.name }}
                      </option>
                      <template v-slot:first>
                        <b-form-select-option :value="null" disabled
                          >- - -</b-form-select-option
                        >
                      </template>
                    </b-form-select>
                  </template>
                </div>
              </template>
            </td>
          </tr>
        </table>
      </div>
      <div
        id="unsetLessonDiv"
        class="unset-lesson-list dropzone"
        @drop="lessonDropToDiv"
        @dragenter="addBackgroundTooltip"
        @dragleave="removeBackgroundTooltip"
        @dragover="lessonOverTarget"
      >
        Список занятий {{ studyClass.name }}
        <hr style="margin-top: 0.2em" />
        <div
          class="lesson"
          draggable="true"
          v-for="lesson in unsetLessonList"
          v-bind:key="lesson.id"
          :lessonId="lesson.id"
          @dragstart="lessonDragStart"
          :class="{
            'lesson-full': !lesson.isSubClassLesson && !lesson.isSubWeekLesson,
            'lesson-subclass':
              lesson.isSubClassLesson && !lesson.isSubWeekLesson,
            'lesson-subWeek': lesson.isSubWeekLesson,
            'lesson-subclass-subWeek':
              lesson.isSubClassLesson && lesson.isSubWeekLesson,
            flow: lesson.flowId > 0,
            parallel: lesson.isParallel,
          }"
        >
          <span v-if="lesson.flowStudyClassNames" class="studyClassNames">{{
            lesson.flowStudyClassNames
          }}</span>
          {{ lesson.lessonType.name.substr(0, 3) }}. {{ lesson.subject.name }}
          <br />
          {{ lesson.teacher.fullName }}
        </div>
      </div>
      <div class="mistake-list">
        Накладки по расписанию
        <hr style="margin-top: 0.2rem" />
        <ul v-if="mistakeList.length > 0">
          <li
            v-for="(mistake, index) in mistakeList"
            v-bind:key="mistake + index"
          >
            {{ index + 1 }}. {{ mistake }}
          </li>
        </ul>
        <ul v-if="mistakeList.length == 0 && !isLoadingMistakes">
          <li>Накладок нет</li>
        </ul>
        <div v-if="isLoadingMistakes" class="text-center">
          <b-spinner small variant="warning" label="Text Centered"></b-spinner>
        </div>
      </div>
    </div>
    <div class="text-center" v-else>
      <b-spinner variant="primary" label="Text Centered" class="mt-2"></b-spinner>
      <strong>Загрузка...</strong>
    </div>
    </template>
  </div>
</template>
<script>
import { GetMistakesByStudyClass, GetTimetableGenerateProgress } from "../../../service/timetable/timetableService";
import {
  GetLessonList,
  LessonSet,
  ResetAllLessons,
} from "../../../service/lessonService";
import { SetClassroomByLesson, GetWarningsByClassroom } from "../../../service/timetableDataServices/classroomService";
import { GetVersionList } from "../../../service/versionService";
import { CalculateTable } from "../../../service/timetable/tableCreateService";
import { lessonVolumeEnum } from "../../../enums/lessonVolumeEnum";
import { GetClassroomList } from "../../../service/timetableDataServices/classroomService";

export default {
  name: "StudyClassTimetable",
  components: {},
  data: function () {
    return {
      isLoading: false,
      isLoadingMistakes: false,
      studyClass: {},

      //Table creating objects
      timetableRowArr: [],
      lessonLabelCellArr: [],
      endRowOfDayArr: [],
      dayLabelCellArr: [],

      //Lesson dragging objects
      draggingleLesson: {},
      currentRowIndex: 0,
      prevRowINdex: 0,
      allowLessonDrop: false,
      unsetLessonList: [],
      setLessonList: [],

      mistakeList: [],
      classroomList: [],
      scrollY: null,

      versionList: [],
      version: {},
      timetableGenerateProgress: {
        message: "",
      },      
    };
  },
  computed: {},
  methods: {
    getLessonList() {
      this.isLoading = true;
      GetLessonList(this.studyClass.id, this.version.id)
        .then((response) => {
          this.unsetLessonList = response.data.filter(
            (l) => l.rowIndex == null
          );
          this.setLessonList = response.data.filter((l) => l.rowIndex != null);
          this.getMistakesByStudyClass();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить занятия.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getVersionList() {
      this.isLoading = true;
      GetVersionList()
        .then((response) => {
          this.versionList = response.data;
          if (response.data.length > 0) {
            this.version = response.data.find((v) => v.isActive);
            this.getLessonList();
          }
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
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

    resetAllLessons() {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите сбросить расписание?", {
          title: "Сбросить расписание",
          size: "sm",
          buttonSize: "sm",
          okVariant: "danger",
          okTitle: "ДА",
          cancelTitle: "НЕТ",
          footerClass: "p-2",
          hideHeaderClose: false,
          centered: true,
        })
        .then((value) => {
          if (value) {
            this.isLoading = true;
            ResetAllLessons(this.studyClass.id)
              .then(() => {
                this.$ntf.Success("Расписание группы сброшено!");
                this.getLessonList();
                this.getMistakesByStudyClass();
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось сбросить занятия.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
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

    setClassroomByLesson(lessonId, classroomId) {
      SetClassroomByLesson(lessonId, classroomId)
        .then(() => {
          this.$ntf.Success("Аудитория сохранена!");
          this.getMistakesByStudyClass();
          this.getWarningsByClassroom(lessonId);
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        });
    },
    

    getWarningsByClassroom(lessonId) {
      GetWarningsByClassroom(lessonId)
        .then((response) => {
          if(response.data.showMessage){
            response.data.messageList.forEach(message => {
              this.$ntf.Warn(message);
            });
          }
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        });
    },

    getMistakesByStudyClass() {
      this.isLoadingMistakes = true;
      GetMistakesByStudyClass(this.studyClass.id, this.version.id)
        .then((response) => {
          this.mistakeList = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить наклдки расписания.", error);
        })
        .finally(() => {
          this.isLoadingMistakes = false;
        });
    },
    getTimetableGenerateProgress() {
      GetTimetableGenerateProgress()
        .then((response) => {
          this.timetableGenerateProgress = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные по прогрессу.", error);
        });
    },    
    //#region Dragging
    //Убрать все выделения в тексте
    unselectAllText() {
      //Убрать все выделения
      if (window.getSelection) {
        window.getSelection().removeAllRanges();
      } else if (document.selection) {
        document.selection.empty();
      }
    },

    //Получить занятия по индексам
    getLessonsByIndex(rowIndex, colIndex) {
      let lessonList = this.setLessonList.filter(
        (l) => l.rowIndex == rowIndex && l.colIndex == colIndex
      );

      return lessonList;
    },

    //Срабытывает когда занятие находится над принимаемым элементом.
    lessonOverTarget(ev) {
      let dropzone = ev.target.classList.contains("dropzone");
      if (
        this.draggingleLesson &&
        Object.keys(this.draggingleLesson).length !== 0 &&
        dropzone
      ) {
        if (ev.preventDefault) {
          ev.preventDefault(); // Necessary. Allows us to drop.
        }
        ev.dataTransfer.dropEffect = "move"; // See the section on the DataTransfer object.
        return false;
      }
      ev.dataTransfer.dropEffect = "none";
    },

    //Окрасить фон при наведении (подсказка)
    addBackgroundTooltip(ev) {
      this.removeAllBGColor();
      let dropzone = ev.target.classList.contains("dropzone");
      let unsetLessonDiv = ev.target.classList.contains("unset-lesson-list");
      if (unsetLessonDiv) {
        return ev.toElement.classList.add("bg-green");
      } else if (
        this.draggingleLesson &&
        Object.keys(this.draggingleLesson).length !== 0 &&
        dropzone
      ) {
        let rowIndex = parseInt(ev.target.getAttribute("rowIndex"));
        let colIndex = parseInt(ev.target.getAttribute("colIndex"));
        this.currentRowIndex = rowIndex;

        if (
          this.getlessonVolume(this.draggingleLesson) == lessonVolumeEnum.Full
        ) {
          let tdHaveChild = false;

          let secondRowIndex =
            rowIndex > 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

          let queryString = this.version.useSubWeek
            ? `[rowIndex="${rowIndex}"],[rowIndex="${secondRowIndex}"]`
            : `[rowIndex="${rowIndex}"]`;

          let tdList = document.querySelectorAll(queryString);

          if (this.draggingleLesson.isParallel) {
            tdList.forEach((td) => {
              td.children.forEach(
                (child) =>
                  (tdHaveChild = !child.classList.contains("parallel")
                    ? true
                    : tdHaveChild)
              );
            });
          } else {
            tdList.forEach(
              (td) =>
                (tdHaveChild = td.children.length > 0 ? true : tdHaveChild)
            );
          }

          if (tdHaveChild) {
            this.allowLessonDrop = false;
            tdList.forEach((td) => td.classList.add("bg-red"));
          } else {
            this.allowLessonDrop = true;
            tdList.forEach((td) => td.classList.add("bg-green"));
          }
        } else if (
          this.getlessonVolume(this.draggingleLesson) ==
          lessonVolumeEnum.SubWeek
        ) {
          let tdList = document.querySelectorAll(`[rowIndex="${rowIndex}"]`);

          let tdHaveChild = false;

          tdList.forEach(
            (td) => (tdHaveChild = td.children.length > 0 ? true : tdHaveChild)
          );

          tdHaveChild =
            this.version.useSubClass && tdList.length < 2 ? true : tdHaveChild;

          if (tdHaveChild) {
            this.allowLessonDrop = false;
            tdList.forEach((td) => td.classList.add("bg-red"));
          } else {
            this.allowLessonDrop = true;
            tdList.forEach((td) => td.classList.add("bg-green"));
          }
        } else if (
          this.getlessonVolume(this.draggingleLesson) ==
          lessonVolumeEnum.SubClass
        ) {
          let secondCol = colIndex > 0 ? 0 : 1;

          let secondRowIndex =
            rowIndex > 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

          let queryString = this.version.useSubWeek
            ? `[rowIndex="${rowIndex}"][colIndex="${colIndex}"],[rowIndex="${secondRowIndex}"][colIndex="${colIndex}"]`
            : `[rowIndex="${rowIndex}"][colIndex="${colIndex}"]`;

          let tdList = document.querySelectorAll(queryString);

          let tdHaveChild = false;

          if (this.version.useSubWeek) {
            let tdIsColspan = document.querySelectorAll(
              `[rowIndex="${secondRowIndex}"][colIndex="${secondCol}"]`
            );

            tdIsColspan.forEach(
              (td) =>
                (tdHaveChild =
                  td.getAttribute("colspan") == 2 ? true : tdHaveChild)
            );

            if (tdHaveChild) {
              tdIsColspan.forEach((td) => td.classList.add("bg-red"));
            }
          }

          tdList.forEach(
            (td) => (tdHaveChild = td.children.length > 0 ? true : tdHaveChild)
          );

          if (tdHaveChild) {
            this.allowLessonDrop = false;
            tdList.forEach((td) => td.classList.add("bg-red"));
          } else {
            this.allowLessonDrop = true;
            tdList.forEach((td) => td.classList.add("bg-green"));
          }
        } else {
          if (ev.toElement.children.length > 0) {
            this.allowLessonDrop = false;
            return ev.toElement.classList.add("bg-red");
          } else {
            this.allowLessonDrop = true;
            return ev.toElement.classList.add("bg-green");
          }
        }
      }
    },

    //Убрать окраску фона при наведении (подсказка)
    removeBackgroundTooltip(ev) {
      let rowIndex = parseInt(ev.currentTarget.getAttribute("rowindex"));
      var unsetLessonDiv = document.getElementsByClassName(
        "unset-lesson-list"
      )[0];
      unsetLessonDiv.classList.remove("bg-green");

      if (
        this.getlessonVolume(this.draggingleLesson) == lessonVolumeEnum.Full
      ) {
        if (
          this.prevRowINdex > 0 &&
          this.prevRowINdex % 2 != 0 &&
          Math.abs(this.prevRowINdex - this.currentRowIndex) <= 1
        ) {
          this.prevRowINdex =
            this.currentRowIndex == 0 ? 1 : this.currentRowIndex;
          return;
        }

        if (
          this.prevRowINdex > 0 &&
          this.prevRowINdex % 2 == 0 &&
          Math.abs(this.prevRowINdex - this.currentRowIndex) <= 1
        ) {
          this.prevRowINdex =
            this.currentRowIndex == 0 ? 1 : this.currentRowIndex;
          return;
        }

        let secondRowIndex =
          rowIndex > 0 && rowIndex % 2 != 0 ? rowIndex-- : rowIndex++;

        let tdList = document.querySelectorAll(
          `[rowIndex="${rowIndex}"],[rowIndex="${secondRowIndex}"]`
        );

        tdList.forEach((td) => {
          td.classList.remove("bg-green");
          td.classList.remove("bg-red");
        });

        this.prevRowINdex = this.currentRowIndex + 1;
      } else if (
        this.getlessonVolume(this.draggingleLesson) == lessonVolumeEnum.SubWeek
      ) {
        if (this.prevRowINdex == this.currentRowIndex) {
          return;
        }

        let tdList = document.querySelectorAll(`[rowIndex="${rowIndex}"]`);

        tdList.forEach((td) => {
          td.classList.remove("bg-green");
          td.classList.remove("bg-red");
        });

        this.prevRowINdex = this.currentRowIndex;
      } else if (
        this.getlessonVolume(this.draggingleLesson) == lessonVolumeEnum.SubClass
      ) {
        //
      } else {
        ev.toElement.classList.remove("bg-red");
        ev.toElement.classList.remove("bg-green");
      }

      if (isNaN(this.currentRowIndex)) {
        this.currentRowIndex = 0;
      }
    },

    //Определить перетаскиваемое занятие
    lessonDragStart(ev) {
      this.unselectAllText();
      if (typeof ev.target.getAttribute === "function") {
        ev.dataTransfer.effectAllowed = "move";
        let lessonId = ev.target.getAttribute("lessonid");
        ev.dataTransfer.setData("lessonid", lessonId);
        ev.dataTransfer.setDragImage(ev.target, 0, 0);

        this.draggingleLesson = this.unsetLessonList.find(
          (l) => l.id == lessonId
        );

        if (!this.draggingleLesson) {
          this.draggingleLesson = this.setLessonList.find(
            (l) => l.id == lessonId
          );
        }

        return true;
      }
    },

    //Переместить занятие в блок нераспределенных занятий
    lessonDropToDiv(ev) {
      var src = ev.dataTransfer.getData("lessonid");
      let elem = document.querySelector(`[lessonid="${src}"]`);
      if (elem) {
        this.draggingleLesson.rowIndex = null;
        this.draggingleLesson.colIndex = null;
        this.draggingleLesson.classroomId = null;

        let indexByAdd = this.unsetLessonList.indexOf(this.draggingleLesson);
        if (indexByAdd < 0) {
          this.unsetLessonList.push(this.draggingleLesson);
          LessonSet(this.draggingleLesson)
            .then(() => {
              this.$ntf.Success("Положение занятия сброшено!");
              this.getMistakesByStudyClass();
            })
            .catch((error) => {
              this.$ntf.Error("Неудалось сбросить положение занятия.", error);
            });
        }

        let indexByRemove = this.setLessonList.indexOf(this.draggingleLesson);
        if (indexByRemove > -1) {
          this.setLessonList.splice(indexByRemove, 1);
        }

        this.draggingleLesson = {};
        this.removeBackgroundTooltip(ev);
        return false;
      }
      this.draggingleLesson = {};
      this.removeAllBGColor();
    },

    //Переместить занятие в таблицу
    lessonDropToTD(ev) {
      if (!this.allowLessonDrop) {
        this.$ntf.Warn("Вы не можете добавить занятие на указанную позицию.");
        this.removeAllBGColor();
        return;
      }

      var src = ev.dataTransfer.getData("lessonid");
      let elem = document.querySelector(`[lessonid="${src}"]`);
      if (elem) {
        let rowIndex = parseInt(ev.target.getAttribute("rowIndex"));
        let colIndex = parseInt(ev.target.getAttribute("colIndex"));

        if (
          this.getlessonVolume(this.draggingleLesson) == lessonVolumeEnum.Full
        ) {
          if (this.version.useSubWeek) {
            this.draggingleLesson.rowIndex =
              rowIndex > 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex;
          } else {
            this.draggingleLesson.rowIndex = rowIndex;
          }

          this.draggingleLesson.colIndex = 0;
        } else if (
          this.getlessonVolume(this.draggingleLesson) ==
          lessonVolumeEnum.SubWeek
        ) {
          this.draggingleLesson.rowIndex = rowIndex;
          this.draggingleLesson.colIndex = 0;
        } else if (
          this.getlessonVolume(this.draggingleLesson) ==
          lessonVolumeEnum.SubClass
        ) {
          if (this.version.useSubWeek) {
            this.draggingleLesson.rowIndex =
              rowIndex > 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex;
          } else {
            this.draggingleLesson.rowIndex = rowIndex;
          }

          this.draggingleLesson.colIndex = colIndex;
        } else {
          this.draggingleLesson.rowIndex = rowIndex;
          this.draggingleLesson.colIndex = colIndex;
        }

        const indexByAdd = this.setLessonList.indexOf(this.draggingleLesson);
        if (indexByAdd < 0) {
          this.setLessonList.push(this.draggingleLesson);
        }

        LessonSet(this.draggingleLesson)
          .then(() => {
            this.$ntf.Success("Положение занятия сохранено!");
            this.getMistakesByStudyClass();
            this.getWarningsByClassroom(src);
          })
          .catch((error) => {
            this.$ntf.Error("Неудалось сохранить положение занятия.", error);
          });

        const indexByRemove = this.unsetLessonList.indexOf(
          this.draggingleLesson
        );
        if (indexByRemove > -1) {
          this.unsetLessonList.splice(indexByRemove, 1);
        }

        this.draggingleLesson = {};
        this.removeAllBGColor();
        return false;
      }
      this.draggingleLesson = {};
      this.removeAllBGColor();
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

    //Убрать все подсказки
    removeAllBGColor() {
      let tdList = document.getElementsByTagName("TD");
      tdList.forEach((td) => {
        td.classList.remove("bg-green");
        td.classList.remove("bg-red");
      });
    },

    //Вычислить свойства ячейки
    tdProps(rowIndex, colIndex) {
      let lesson = this.setLessonList.find(
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
    firstTdVisible(rowIndex) {
      let visible = true;

      if (this.tdProps(rowIndex - 1, 0).rowspan > 1) {
        visible = false;
      }

      return visible;
    },

    //Видимость второй ячейки
    secondTdVisible(rowIndex) {
      let visible = true;

      if (
        !(this.tdProps(rowIndex - 1, 1).rowspan > 1) &&
        !(
          this.tdProps(rowIndex - 1, 0).rowspan > 1 &&
          this.tdProps(rowIndex - 1, 0).colspan > 1
        ) &&
        this.tdProps(rowIndex, 0).colspan < 2
      ) {
        visible = true;
      } else {
        visible = false;
      }

      return visible;
    },

    scrollDivFunc() {
      let div = document.getElementById("unsetLessonDiv");
      this.scrollY = Math.round(window.scrollY);

      if (this.scrollY > 155) {
        div.classList.add("unset-lesson-div-fixed");
      } else if (div) {
        div.classList.remove("unset-lesson-div-fixed");
      }
    },
    //#endregion
  },
  created() {
    this.getClassroomList();
    if (this.$route.params.studyClass) {
      this.studyClass = this.$route.params.studyClass;
      this.getTimetableGenerateProgress();
      this.getVersionList();
    } else {
      this.$router.push({ name: "timetableManual" });
      this.$ntf.Error("Не была передана группа для отображения расписания.", null);
    }
  },
  mounted() {
    window.addEventListener("scroll", this.scrollDivFunc);
  },

  destroyed() {
    window.removeEventListener("scroll", this.scrollDivFunc, false);
  },
  watch: {
    version: function () {
      this.createTable();
      this.getLessonList();
    },
  },
  props: {},
};
</script>

<style scoped>
.studyclass-timetable-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}

.reset-btn {
  position: relative;
  left: 0;
}
.unset-lesson-list {
  padding: 1rem;
  overflow: auto;
  text-align: right;
  position: absolute;
  width: 36rem;
  min-height: 13rem;
  max-height: 40rem;
  background-color: white;
  border: 1px solid #ced4da;
  border-radius: 0.25rem;
  right: 0;
  top: 8.5rem;
}

.unset-lesson-div-fixed {
  position: fixed;
  top: 4rem;
  right: 1rem;
}

.unset-lesson-list div {
  float: left;
  margin: 5px;
}

.mistake-list {
  padding: 1rem;
  max-width: 40rem;
  max-height: 25rem;
  text-align: right;
  overflow: auto;
  position: fixed;
  right: 1rem;
  bottom: 1rem;
  border: 1px solid #ffb648;
  border-radius: 0.25rem;
}

.mistake-list ul {
  text-align: left;
  font-size: 10pt;
  list-style: none;
  margin: 0;
  padding: 0;
}

.lesson {
  text-align: center;
  border-radius: 5px;
  border: 1px solid #919191;
  font-size: 12px;
  background-size: 15px;
  background-repeat: no-repeat;
  background-position-x: 3px;
  background-position-y: 3px;
  width: 160px;
}
.lesson-full {
  background-color: #66cdaa;
}
.lesson-full:hover {
  background-size: 15px;
  background-repeat: no-repeat;
  background-position-x: 3px;
  background-position-y: 3px;
  background-image: url("../../../wwwroot/lesson-bg-img/full.png");
}
.lesson-subclass {
  background-color: #4682b4;
}
.lesson-subclass:hover {
  background-image: url("../../../wwwroot/lesson-bg-img/subclass.png");
}

.lesson-subWeek {
  background-color: #c0c0c0;
}
.lesson-subWeek:hover {
  background-image: url("../../../wwwroot/lesson-bg-img/subWeek.png");
}

.lesson-subclass-subWeek {
  background-color: #708090;
}
.lesson-subclass-subWeek:hover {
  background-image: url("../../../wwwroot/lesson-bg-img/subclass-subWeek.png");
}

.flow {
  background-size: 18px;
  background-repeat: no-repeat;
  background-position-x: 99%;
  background-position-y: 97%;
  background-image: url("../../../wwwroot/lesson-bg-img/flow.png");
}

.flow .studyClassNames {
  background-color: inherit;
  display: none;
  position: absolute;
}

.flow:hover .studyClassNames {
  background-color: white;
  padding: 1px 3px 1px 3px;
  border-radius: 2px;
  display: block;
  position: absolute;
}

.parallel {
  background-size: 18px;
  background-repeat: no-repeat;
  background-position-x: 99%;
  background-position-y: 97%;
  background-image: url("../../../wwwroot/lesson-bg-img/parallel.png");
}

td .parallel {
  margin-bottom: 3px;
}

td .parallel:nth-last-child(1) {
  margin-bottom: 30px;
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
.timetable tr,
td {
  border: 1px solid #ced4da;
  padding: 3px;
}

.timetable th:nth-child(3) {
  width: 220px;
}

.timetable tr {
  height: 2rem;
}

.td-style {
  max-width: 10rem;
  width: 10rem;
  min-width: 10rem;
}

.timetable td div {
  width: 100%;
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

.bg-green {
  background-color: rgba(0, 153, 0, 0.247);
}
.bg-red {
  background-color: rgba(153, 0, 0, 0.247);
}

.classroom-select {
  width: 140px;
  font-weight: 600;
  font-size: 9pt;
  text-align: left;
  border: 1px solid #919191;
  margin-bottom: -1px;
  background-color: inherit;
}
</style>
