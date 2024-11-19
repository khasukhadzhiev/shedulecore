<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="mt-3 text-left" v-if="showAddBlock">
      <div class="row">
        <div class="col-2">
          <label>Вид занятия</label>
          <b-form-select
            v-model="flowLesson.lessonTypeId"
            :options="lessonTypeList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
        <div class="col">
          <label>Дисциплина</label>
          <multiselect
            v-model="flowLesson.subject"
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
            v-model="flowLesson.teacher"
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
        <div class="col-2">
          <label>Группы в потоке</label>
          <multiselect
            v-model="selectedstudyClass"
            :options="studyClassList"
            track-by="id"
            label="name"
            :show-labels="false"
            placeholder="Выбрать группу"
            :multiple="true"
          >
            <template slot="noResult">Группа не найдена!</template>
          </multiselect>
        </div>
      </div>
      <div class="row" v-if="version.useSubWeek">
        <div class="col">
          <form class="form-inline mt-2">
            <b-form-radio
              class="mr-3"
              v-model.number="flowLesson.isSubWeekLesson"
              name="isSubWeekLesson-radios"
              :value="false"
            >По каждой неделе</b-form-radio>
            <b-form-radio
              v-model.number="flowLesson.isSubWeekLesson"
              name="isSubWeekLesson-radios"
              :value="true"
            >По одной неделе</b-form-radio>
          </form>
        </div>
      </div>
    </div>
    <hr v-if="showAddBlock" />
    <div class="row mb-3 mt-3 text-right">
      <div class="col">
        <a
          href="#"
          @click="showAddBlock = !showAddBlock"
          class="pr-2"
        >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
        <b-button
          variant="outline-primary"
          @click="addFlowLesson"
          v-if="showAddBlock"
          :disabled="selectedstudyClass.length < 1"
        >Добавить поточное занятие</b-button>
      </div>
    </div>
    <div class="row mt-3 mb-4 text-left">
      <div class="col-6">
        <label>Фильтрация</label>
        <b-form-input v-model="filter" @input="filter = filter.toUpperCase()" placeholder="ФИО преподавателя / Дисциплина"></b-form-input>
      </div>
    </div>        
    <h6 class="text-left">Список поточных занятий {{studyClass.name}} группы</h6>
    <b-table
      hover
      outlined
      head-variant="light"
      :items="flowLessonList"
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

      <template v-slot:cell(flowStudyClassNames)="row">{{row.item.flowStudyClassNames}}</template>
      <template
        v-slot:cell(isSubWeekLesson)="row" v-if="version.useSubWeek"
      ><span class="btn" @click="changeSubWeekType(row.item)">
        {{row.item.isSubWeekLesson ? "По одной неделе" : "По каждой неделе"}}
        <b-icon icon="arrow-left-right" v-if="row.item.rowIndex == null" ></b-icon>
      </span></template>

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
  GetFlowLessonList,
  GetFlowLessonFilterList,
  AddFlowLesson,
  RemoveLesson,
  EditLessonData,
  LessonSet,
  DiscriptLessonDayAndNumber
} from "../../../service/lessonService";

export default {
  name: "FlowLesson",
  data: function() {
    return {
      isLoading: false,
      flowLesson: {
        lessonTypeId: "",
        studyClassId: this.studyClass.id,
        subject: "",
        teacher: "",
        isParallel: false,
        flowStudyClassIds: [],
        isSubWeekLesson: false
      },
      filter:"",      
      selectedstudyClass: [],
      flowLessonList: [],
      showAddBlock: false,
      fields: [
        { key: "index", label: "№" },
        { key: "lessonType", label: "Вид занятия", sortable: true  },
        { key: "subject", label: "Дисциплина", sortable: true  },
        { key: "teacher", label: "Преподаватель", sortable: true  },
        { key: "flowStudyClassNames", label: "Группы в потоке", sortable: true  },
        { key: "isSubWeekLesson", label: "Неделя" },
        { key: "removeFromTimetable", label: "Положение" },
        { key: "remove", label: "" }
      ]
    };
  },
  computed: {
    computedFields(){
      let reuslt = this.fields;

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
                this.getFlowLessonList(this.studyClass.id, this.version.id);
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось сбросить положение занятия.", error);
              });
          }
        });
    },
    getFlowLessonList(studyClassId, timetableVersionId) {
      this.isLoading = true;
      GetFlowLessonList(studyClassId, timetableVersionId)
        .then(response => {
          this.flowLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.filter="";
          this.isLoading = false;
        });
    },
    getFlowLessonFilterList(studyClassid, timetableVersionid, filter) {
      this.isLoading = true;
      GetFlowLessonFilterList(studyClassid, timetableVersionid, filter)
        .then(response => {
          this.flowLessonList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },        
    addFlowLesson() {
      let isEmty =this.flowLesson.flowStudyClassIds.length < 1;

      if (isEmty === "") {
        this.$ntf.Warn(
          "Необходимо выбрать группы для создания потока."
        );
        return;
      }

      this.isLoading = true;
      this.flowLesson.flowStudyClassIds = this.selectedstudyClass.map(
        f => f.id
      );
      this.flowLesson.flowStudyClassIds.push(this.studyClass.id);
      AddFlowLesson(this.flowLesson, this.version.id)
        .then(() => {
          this.$ntf.Success("Занятие сохранено!");
          this.getFlowLessonList(this.studyClass.id, this.version.id);
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
                this.getFlowLessonList(this.studyClass.id, this.version.id);
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
    multipleSelectItems(e) {
      e.preventDefault();

      var select = this;
      var scroll = select.scrollTop;

      e.target.selected = !e.target.selected;

      setTimeout(function() {
        select.scrollTop = scroll;
      }, 0);
    },
    changeSubWeekType(lesson){
      if(lesson.rowIndex != null){
        return;
      }
      lesson.isSubWeekLesson = !lesson.isSubWeekLesson;
      this.editLessonData(lesson);
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
    if ('id' in this.studyClass) {
      this.getFlowLessonList(this.studyClass.id, this.version.id);
      this.flowLesson.lessonTypeId = this.lessonTypeList[0].id;
      this.flowLesson.subject = this.subjectList[0];
      this.flowLesson.teacher = this.teacherList[0];
    }
  },
  props: {
    version: Object,
    studyClass: Object,
    lessonTypeList: Array,
    subjectList: Array,
    teacherList: Array,
    studyClassList: Array,
  },
  watch: {
    studyClass: function() {
      if ('id' in this.studyClass) {
        this.getFlowLessonList(this.studyClass.id, this.version.id);
        this.flowLesson.studyClassId = this.studyClass.id;
      }
    },
    version: function() {
      if ('id' in this.version) {
        this.getFlowLessonList(this.studyClass.id, this.version.id);
      }
    },
    filter: function() {
      if ("id" in this.studyClass && "id" in this.version && this.filter) {
        this.getFlowLessonFilterList(this.studyClass.id, this.version.id, this.filter);
      }
      else{
        this.getFlowLessonList(this.studyClass.id, this.version.id);
      }
    }        
  }
};
</script>

<style scoped>
</style>
