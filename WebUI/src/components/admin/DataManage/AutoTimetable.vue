<template>
  <div v-if="$Role.EmployeeHasRole('admin')">
    <div class="text-left mt-3">
      <label>Выберите версию расписания для генерации</label>
      <div class="row">
        <div class="col-4 text-left">
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
        <div class="col-3 text-left" v-if="version.showClassShift">
          <multiselect
            v-model="classShift"
            :options="classShiftList"
            :allowEmpty="false"
            track-by="id"
            label="name"
            :show-labels="false"
            placeholder="Выбрать версию смену обучения"
            :multiple="false"
          >
            <template slot="noResult">Смена не найдена!</template>
          </multiselect>
        </div>
      </div>
    </div>
    <hr />
    <template v-if="!timetableGenerateProgress.start">
      <div class="row mt-3" v-for="item in educationFormList" :key="item.id">
        <div class="col-2 ml-2 text-left">{{item.name}}</div>
        <div class="col">
          <form class="form-inline">
            <b-form-checkbox class="pr-4" :value="Number(0)" v-model="item.weekDays">Понедельник</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(1)" v-model="item.weekDays">Вторник</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(2)" v-model="item.weekDays">Среда</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(3)" v-model="item.weekDays">Четверг</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(4)" v-model="item.weekDays">Пятница</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(5)" v-model="item.weekDays">Суббота</b-form-checkbox>
            <b-form-checkbox class="pr-4" :value="Number(6)" v-model="item.weekDays" v-if="version.useSunday">Воскресенье</b-form-checkbox>
          </form>
        </div>
      </div>
      <hr />
    </template>
    <template v-if="!timetableGenerateProgress.start">
      <div class="row mt-3">
        <div class="col">
          <form class="form-inline">
            <b-form-checkbox class="pr-4" v-model="geneticAlgorithmDataDto.allowEmptyLesson">Разрешить окна в расписании</b-form-checkbox>
          </form>
        </div>
      </div>
      <hr />
    </template>    
    <div class="row mt-3">
      <div class="col text-left">
        <b-button
          variant="outline-primary"
          class="mr-2"
          @click="generateTimetable"
          v-if="!timetableGenerateProgress.start"
        >Начать генерацию расписания</b-button>
        <b-button
          variant="outline-primary"
          class="mr-2"
          @click="getTimetableGenerateProgress"
          v-if="timetableGenerateProgress.start"
        >Обновить</b-button>            
        <b-button
          variant="outline-primary"
          class="mr-2"
          @click="saveTimetableWithMistakes"
          v-if="timetableGenerateProgress.start"
        >Сохранить с накладками</b-button>        
        <b-button
          variant="outline-danger"
          @click="stopTimetableGenerate"
          v-if="timetableGenerateProgress.start"
        >Отменить генерацию</b-button>
      </div>
    </div>
    <hr />
    <div class="text-left">
      <div class="row" v-if="timetableGenerateProgress.start">
        <div class="col">
          <span class="font-weight-bold mr-3">{{timetableGenerateProgress.message}}</span>
        </div>
      </div>
      <div class="row mt-2">
        <div class="col">{{timetableGenerateProgress.spentTime}}</div>
      </div>
    </div>
  </div>
</template>

<script>
import { GetVersionList } from "../../../service/versionService";
import {
  GenerateTimetable,
  GetTimetableGenerateProgress,
  StopTimetableGenerate,
  SaveTimetableWithMistakes
} from "../../../service/timetable/timetableService";
import {
  GetEducationFormList,
  GetClassShiftList,
} from "../../../service/timetableDataServices/studyClassService";

export default {
  name: "AutoTimetable",
  data: function () {
    return {
      version: {},
      versionList: [],
      classShift: [],
      classShiftList: [],
      educationFormList: [],
      geneticAlgorithmDataDto: {
        versionId: 0,
        classShiftId: 0,
        educationFormData: [],
        allowEmptyLesson: false
      },
      timetableGenerateProgress: {
        message: "",
      },
    };
  },
  computed: {},
  methods: {
    getVersionList() {
      this.isLoading = true;
      GetVersionList()
        .then((response) => {
          this.versionList = response.data;
          if (response.data.length > 0) {
            this.version = response.data.find((v) => v.isActive);
          }
          this.getEducationFormList();
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getClassShiftList() {
      this.isLoading = true;
      GetClassShiftList()
        .then((response) => {
          this.classShiftList = response.data;
          if (response.data.length > 0) {
            this.classShift = response.data[0];
          }
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    saveTimetableWithMistakes(){
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите сохранить расписание с накладками? Если подождать, будет найдено оптимальное расписание", {
          title: "Генерация расписания",
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
            SaveTimetableWithMistakes()
              .then(() => {
                this.$ntf.Success("Расписание сохранено.");
                this.getTimetableGenerateProgress();
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось сохранить расписание.", error);
              });                      
          }
        });
    },
    generateTimetable() {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите сгенерировать расписание? Имеющееся расписание будет сброшено!", {
          title: "Генерация расписания",
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
            this.geneticAlgorithmDataDto.versionId = this.version.id;
            this.geneticAlgorithmDataDto.classShiftId = this.classShift.id;
            this.geneticAlgorithmDataDto.educationFormData = this.educationFormList;
            GenerateTimetable(this.geneticAlgorithmDataDto)
              .then(() => {
                this.getTimetableGenerateProgress();
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось сгенерировать расписание.", error);
              });
            this.timetableGenerateProgress.start = true;
          }
        });
    },
    getEducationFormList() {
      GetEducationFormList()
        .then((response) => {
          this.educationFormList = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
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
    stopTimetableGenerate() {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите остановить генерацию?", {
          title: "Генерация расписания",
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
            StopTimetableGenerate()
              .then(() => {
                this.$ntf.Success("Генерация расписания остановлена.");
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось остановить генерацию расписания.", error);
              });              
          }
        });
    },
  },
  created() {
    this.getVersionList();
    this.getClassShiftList();
    this.getTimetableGenerateProgress();
  },
  props: {},
};
</script>

<style scoped>
.manage-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
