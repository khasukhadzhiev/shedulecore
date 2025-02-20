<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="mt-3 text-left" v-if="showAddBlock">
      <div class="row">
        <div class="col-2">
          <label>Вид занятия</label>
          <b-form-select
            v-model="mainLesson.lessonTypeId"
            :options="lessonTypeList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
        <div class="col">
          <label>Дисциплина</label>
          <multiselect
            v-model="mainLesson.subject"
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
            v-model="mainLesson.teacher"
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
      <div class="row">
        <div class="col">
          <form class="form-inline mt-2" v-if="version.useSubWeek">
            <b-form-radio
              class="mr-3"
              v-model.number="mainLesson.isSubWeekLesson"
              name="isSubWeekLesson-radios"
              :value="false"
            >По каждой неделе</b-form-radio>
            <b-form-radio
              class="mr-3"
              v-model.number="mainLesson.isSubWeekLesson"
              name="isSubWeekLesson-radios"
              :value="true"
            >По одной неделе</b-form-radio>
          </form>
          <form class="form-inline mt-2" v-if="version.useSubClass">
            <b-form-checkbox v-model="mainLesson.isSubClassLesson">Добавить как занятие подгруппы</b-form-checkbox>
          </form>
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
          @click="addMainLesson"
          v-if="showAddBlock"
        >Добавить занятие</b-button>
      </div>
    </div>
    <div class="row mt-3 mb-4 text-left">
      <div class="col-6">
        <label>Фильтрация</label>
        <b-form-input v-model="filter" @input="filter = filter.toUpperCase()" placeholder="ФИО преподавателя / Дисциплина"></b-form-input>
      </div>
    </div>    
    <h6 class="text-left">Список занятий группы: {{studyClass.name}} </h6>
    <b-table
      hover
      outlined
      head-variant="light"
      :items="mainLessonList"
      :fields="computedFields"
      :busy="isLoading"
      class="text-left"
    >
      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>
      <template v-slot:cell(index)="row">{{row.index + 1}}</template>
      <template v-slot:cell(lessonType)="row">
        <multiselect
          v-model="row.item.lessonType"
          :options="lessonTypeList"
          :allowEmpty="false"
          track-by="id"
          label="name"
          :show-labels="false"
          placeholder="Выбрать вид занятия"
          :multiple="false"
          @select="editLessonData(row.item)"
        >
          <template slot="noResult">Вид занятий не найден!</template>
        </multiselect>
      </template>
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
      <template
        v-slot:cell(isSubWeekLesson)="row" v-if="version.useSubWeek"
      ><span class="btn" @click="changeSubWeekOrSubClassType(row.item, true, null)">
        {{row.item.isSubWeekLesson ? "По одной неделе" : "По каждой неделе"}}
        <b-icon icon="arrow-left-right" v-if="row.item.rowIndex == null" ></b-icon>
      </span></template>
      <template
        v-slot:cell(isSubClassLesson)="row"  v-if="version.useSubClass">
      <span class="btn" @click="changeSubWeekOrSubClassType(row.item, null, true)">
        {{row.item.isSubClassLesson ? "Подгруппа" : "Группа"}}
        <b-icon icon="arrow-left-right" v-if="row.item.rowIndex == null"></b-icon>
      </span>
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

      <template v-slot:cell(clone)="row" style="width:50px">
          <span class="btn" @click="cloneMainLesson(row.item)">
            <b-icon icon="plus" scale="1"></b-icon>
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
  GetMainLessonList,
  GetMainLessonFilterList,
  AddMainLesson,
  RemoveLesson,
  EditLessonData,
  LessonSet,
  DiscriptLessonDayAndNumber,
  CloneMainLesson
} from "../../../service/lessonService";

export default {
  name: "MainLesson",
  data: function() {
    return {
      isLoading: false,
      mainLesson: {
        lessonTypeId: "",
        studyClassId: this.studyClass.id,
        subject: "",
        teacher: {},
        isParallel: false,
        isSubClassLesson: false,
        isSubWeekLesson: false
      },
      filter:"",
      mainLessonList: [],
      showAddBlock: false,
      fields: [
        { key: "index", label: "№" },
        { key: "lessonType", label: "Вид занятия", sortable: true },
        { key: "subject", label: "Дисциплина", sortable: true },
        { key: "teacher", label: "Преподаватель", sortable: true },
        { key: "isSubClassLesson", label: "Полнота" },
        { key: "isSubWeekLesson", label: "Неделя", sortable: true },
        { key: "removeFromTimetable", label: "Положение" },
        { key: "clone", label: ""},
        { key: "remove", label: "" }
      ]
    };
  },
  computed: {
    computedFields(){
      let reuslt = this.fields;
      if(!this.version.useSubClass){
        reuslt = reuslt.filter(field => field.key != "isSubClassLesson");
      }

      if(!this.version.useSubWeek){
        reuslt = reuslt.filter(field => field.key != "isSubWeekLesson");
      }

      return reuslt;
    }    
  },
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
    cloneMainLesson(lesson){
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите клонировать занятие?", {
          title: "Клонирование занятия",
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
            CloneMainLesson(lesson, this.version.id)
              .then(() => {
                this.$ntf.Success("Положение занятия сброшено!");
                this.getMainLessonList(this.studyClass.id, this.version.id);
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось клонировать занятие.", error);
              });
          }
        });
    },
    getMainLessonList(studyClassid, timetableVersionid) {
      this.isLoading = true;
      GetMainLessonList(studyClassid, timetableVersionid)
        .then(response => {
          this.mainLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.filter = "";
          this.isLoading = false;
        });
    },
    getMainLessonFilterList(studyClassid, timetableVersionid, filter) {
      this.isLoading = true;
      GetMainLessonFilterList(studyClassid, timetableVersionid, filter)
        .then(response => {
          this.mainLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },    
    addMainLesson() {
      this.isLoading = true;
      this.mainLesson.studyClassId = this.studyClass.id;
      AddMainLesson(this.mainLesson, this.version.id)
        .then(() => {
          this.$ntf.Success("Занятие сохранено!");
          this.getMainLessonList(this.studyClass.id, this.version.id);
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
                this.getMainLessonList(this.studyClass.id, this.version.id);
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
    changeSubWeekOrSubClassType(lesson, isSubWeek, isSubClass){
      if(lesson.rowIndex != null){
        return;
      }

      if(isSubWeek){
        lesson.isSubWeekLesson = !lesson.isSubWeekLesson;
        this.editLessonData(lesson);
      }
      else if(isSubClass){
        lesson.isSubClassLesson = !lesson.isSubClassLesson;
        this.editLessonData(lesson);
      }
    },
    editLessonData(lesson){
      EditLessonData(lesson, this.version.id)
        .then(() => {
          this.$ntf.Success("Занятие сохранено!");
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении занятия.", error);
        });      
    },
    discriptLessonDayAndNumber(lessonDay, lessonNumber){
      return DiscriptLessonDayAndNumber(lessonDay, lessonNumber);
    }
  },
  created() {
    if ("id" in this.studyClass) {
      this.getMainLessonList(this.studyClass.id, this.version.id);
      this.mainLesson.lessonTypeId = this.lessonTypeList[0].id;
      this.mainLesson.subject = this.subjectList[0];
      this.mainLesson.teacher = this.teacherList[0];
    }
  },
  props: {
    studyClass: Object,
    lessonTypeList: Array,
    subjectList: Array,
    teacherList: Array,
    version: Object
  },
  watch: {
    studyClass: function() {
      if ("id" in this.studyClass && "id" in this.version) {
        this.getMainLessonList(this.studyClass.id, this.version.id);
        this.mainLesson.studyClassId = this.studyClass.id;
      }
    },
    version: function() {
      if ("id" in this.studyClass && "id" in this.version) {
        this.getMainLessonList(this.studyClass.id, this.version.id);
      }
    },
    filter: function() {
      if ("id" in this.studyClass && "id" in this.version && this.filter) {
        this.getMainLessonFilterList(this.studyClass.id, this.version.id, this.filter);
      }
      else{
        this.getMainLessonList(this.studyClass.id, this.version.id);
      }
    }    
  }
};
</script>

<style scoped>
</style>
