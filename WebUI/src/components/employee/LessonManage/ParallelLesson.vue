<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="mt-3 text-left" v-if="showAddBlock">
      <div class="row">
        <div class="col">
          <label>Вид занятия</label>
          <b-form-select
            v-model="parallelLesson.lessonTypeId"
            :options="lessonTypeList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
        <div class="col">
          <label>Дисциплина</label>
          <multiselect
            v-model="parallelLesson.subject"
            :options="subjectList"
            :allowEmpty="false"
            track-by="id"
            label="name"
            :show-labels="false"
            placeholder="Выбрать дисциплину"
            :multiple="false"
          >
            <template slot="noResult">Дисциплина не найдена!</template>
          </multiselect>
        </div>
        <div class="col">
          <label>Преподаватель</label>
          <multiselect
            v-model="parallelLesson.teacher"
            :options="teacherList"
            :allowEmpty="false"
            track-by="id"
            label="fullName"
            :show-labels="false"
            placeholder="Выбрать преподавателя"
            :multiple="false"
          >
            <template slot="noResult">Преподаватель не найден!</template>
          </multiselect>
        </div>
      </div>
    </div>
    <hr v-if="showAddBlock" />
    <div class="row mt-3 text-right">
      <div class="col">
        <a
          href="#"
          @click="showAddBlock = !showAddBlock"
          class="pr-2"
        >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
        <b-button
          variant="outline-primary"
          @click="addParallelLesson"
          v-if="showAddBlock"
        >Добавить параллель</b-button>
      </div>
    </div>
    <div class="row mt-3 mb-4 text-left">
      <div class="col-6">
        <label>Фильтрация</label>
        <b-form-input v-model="filter" @input="filter = filter.toUpperCase()" placeholder="ФИО преподавателя / Дисциплина"></b-form-input>
      </div>
    </div>        
    <h6 class="text-left">Список параллелей {{studyClass.name}} группы</h6>
    <b-table
      hover
      outlined
      head-variant="light"
      :items="parallelLessonList"
      :fields="fields"
      :busy="isLoading"
      class="text-left"
    >
      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>
      <template v-slot:cell(index)="row">{{row.index + 1}}</template>
      <template v-slot:cell(lessonType)="row">{{row.item.lessonType.name}}</template>

      <template v-slot:cell(subject)="row">
        <multiselect
          v-model="row.item.subject"
          :options="subjectList"
          :allowEmpty="false"
          track-by="id"
          label="name"
          :show-labels="false"
          placeholder="Выбрать дисциплину"
          :multiple="false"
          @select="editLessonData(row.item)"
        >
          <template slot="noResult">Дисциплина не найдена!</template>
        </multiselect>
      </template>
      <template v-slot:cell(teacher)="row">
        <multiselect
          v-model="row.item.teacher"
          :options="teacherList"
          :allowEmpty="false"
          track-by="id"
          label="fullName"
          :show-labels="false"
          placeholder="Выбрать преподавателя"
          :multiple="false"
          @select="editLessonData(row.item)"
        >
          <template slot="noResult">Преподаватель не найден!</template>
        </multiselect>
      </template>
      <template v-slot:cell(removeFromTimetable)="row">
          <span v-if="row.item.rowIndex != null" class="btn" @click="removeFromTimetable(row.item)">
            {{ discriptLessonDayAndNumber(row.item.lessonDay, row.item.lessonNumber) }}
            <b-icon icon="reply"></b-icon>
          </span>
          <span v-else>
            Не назначено
          </span>
      </template>
      <template v-slot:cell(remove)="data" style="width:50px">
        <span class="btn" @click="removeLesson(data.item.id)">
          <b-icon-trash variant="danger"></b-icon-trash>
        </span>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetParallelLessonList,
  GetParallelLessonFilterList,
  AddParallelLesson,
  RemoveLesson,
  EditLessonData,
  LessonSet,
  DiscriptLessonDayAndNumber
} from "../../../service/lessonService";

export default {
  name: "ParallelLesson",
  data: function() {
    return {
      isLoading: false,
      parallelLesson: {
        lessonTypeId: "",
        studyClassId: this.studyClass.id,
        subject: "",
        teacher: "",
        isParallel: true,
        isSubWeekLesson: false
      },
      parallelLessonList: [],
      filter:"",
      showAddBlock: false,
      fields: [
        { key: "index", label: "№" },
        { key: "lessonType", label: "Вид занятия", sortable: true },
        { key: "subject", label: "Дисциплина", sortable: true },
        { key: "teacher", label: "Преподаватель", sortable: true },
        { key: "removeFromTimetable", label: "Положение" },
        { key: "remove", label: "" }
      ]
    };
  },
  computed: {},
  methods: {
    removeFromTimetable(lesson){
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить занятие из сетки расписания?", {
          title: "Удаление из сетки расписания",
          size: "sm",
          buttonSize: "sm",
          okVariant: "danger",
          okTitle: "ДА",
          cancelTitle: "НЕТ",
          footerClass: "p-2",
          hideHeaderClose: false,
          centered: true
        })
        .then(value => {
          if (value) {
            lesson.RowIndex = null;
            LessonSet(lesson)
              .then(() => {
                this.$ntf.Success("Положение занятия сброшено!");
                this.getMainLessonList(this.studyClass.id, this.version.id);
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось сбросить положение занятия.", error);
              });
          }
        });
    },
    getParallelLessonList(studyClassId, timetableVersionId) {
      this.isLoading = true;
      GetParallelLessonList(studyClassId, timetableVersionId)
        .then(response => {
          this.parallelLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.filter = "";
          this.isLoading = false;
        });
    },
    getParallelLessonFilterList(studyClassId, timetableVersionId, filter) {
      this.isLoading = true;
      GetParallelLessonFilterList(studyClassId, timetableVersionId, filter)
        .then(response => {
          this.parallelLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },    
    addParallelLesson() {
      this.isLoading = true;
      AddParallelLesson(this.parallelLesson, this.version.id)
        .then(() => {
          this.$ntf.Success("Занятие сохранено!");
          this.getParallelLessonList(this.studyClass.id, this.version.id);
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении занятия.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeLesson(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить занятие?", {
          title: "Удаление занятия",
          size: "sm",
          buttonSize: "sm",
          okVariant: "danger",
          okTitle: "ДА",
          cancelTitle: "НЕТ",
          footerClass: "p-2",
          hideHeaderClose: false,
          centered: true
        })
        .then(value => {
          if (value) {
            this.isLoading = true;
            RemoveLesson(id)
              .then(() => {
                this.getParallelLessonList(this.studyClass.id, this.version.id);
                this.$ntf.Success("Занятие удалено!");
              })
              .catch(error => {
                this.$ntf.Error("Неудалось удалить занятие.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editLessonData(lesson) {
      this.isLoading = true;
      EditLessonData(lesson, this.version.id)
        .then(() => {
          this.$ntf.Success("Занятие сохранено!");
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении занятия.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    discriptLessonDayAndNumber(lessonDay, lessonNumber){
      return DiscriptLessonDayAndNumber(lessonDay, lessonNumber);
    }
  },
  created() {
    if ("id" in this.studyClass) {
      this.getParallelLessonList(this.studyClass.id, this.version.id);
      this.parallelLesson.lessonTypeId = this.lessonTypeList[0].id;
      this.parallelLesson.subject = this.subjectList[0];
      this.parallelLesson.teacher = this.teacherList[0];
    }
  },
  props: {
    version: Object,
    studyClass: Object,
    lessonTypeList: Array,
    subjectList: Array,
    teacherList: Array
  },
  watch: {
    studyClass: function() {
      if ("id" in this.studyClass) {
        this.getParallelLessonList(this.studyClass.id, this.version.id);
        this.parallelLesson.studyClassId = this.studyClass.id;
      }
    },
    version: function() {
      if ("id" in this.version) {
        this.getParallelLessonList(this.studyClass.id, this.version.id);
      }
    },
    filter: function() {
      if ("id" in this.studyClass && "id" in this.version && this.filter) {
        this.getParallelLessonFilterList(this.studyClass.id, this.version.id, this.filter);
      }
      else{
        this.getParallelLessonList(this.studyClass.id, this.version.id);
      }
    }        
  }
};
</script>

<style scoped>
</style>
