<template v-if="$Role.EmployeeHasRole('employee')">
  <div class="timetabledata-content">
    <h5 class="mt-2 text-left">Данные расписания</h5>
    <div v-if="timetableGenerateProgress.start" class="mt-2">
      <h5>В данный момент запущена автогенерация расписания. Необходимо дождаться остановки процесса генерации для внесения изменений.</h5>
    </div>    
    <template v-if="!timetableGenerateProgress.start">
      <div v-if="subdivisionList.length > 0">
        <div class="row mt-3 text-left">
          <div class="col-4">
            <label>Введите наименование подразделения</label>
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
        <ul class="nav nav-tabs mt-4">
          <li class="nav-item">
            <a class="nav-link active" data-toggle="tab" href="#studyClasses">Группы</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" data-toggle="tab" href="#subjects">Дисциплины</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" data-toggle="tab" href="#teachers">Преподаватели</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" data-toggle="tab" href="#buildings">Учебные корпуса</a>
          </li>        
          <li class="nav-item" @click="getBuildingList">
            <a class="nav-link" data-toggle="tab" href="#classrooms">Аудитории</a>
          </li>
        </ul>

        <div class="tab-content">
          <div class="tab-pane fade show active" id="studyClasses">
            <StudyClasses :version="version" />
          </div>
          <div class="tab-pane fade show" id="subjects">
            <Subjects />
          </div>
          <div class="tab-pane fade show" id="teachers">
            <Teachers :version="version" />
          </div>
          <div class="tab-pane fade show" id="buildings">
            <Buildings :version="version" />
          </div>        
          <div class="tab-pane fade show" id="classrooms">
            <Classrooms :buildingList="buildingList" />
          </div>
        </div>
      </div>
    </template>
    <div v-if="subdivisionList.length == 0 && !isLoading" class="text-center mt-5">
      <h5>У вас нет роли или вы не связаны ни с одним подразделением для добавления данных! Обратитесь к администратору.</h5>
    </div>
    <div class="text-center" v-if="isLoading">
      <b-spinner variant="primary" label="Text Centered"></b-spinner>
      <strong>Загрузка...</strong>
    </div>
  </div>
</template>

<script>
import Teachers from "./TimetableData/Teachers";
import Subjects from "./TimetableData/Subjects";
import StudyClasses from "./TimetableData/StudyClasses";
import Buildings from "./TimetableData/Buildings";
import Classrooms from "./TimetableData/Classrooms";
import { GetCurrentEmployeeSubdivisionList } from "../../service/subdivisionService";
import { GetVersionList } from "../../service/versionService";
import { GetBuildingList } from "../../service/timetableDataServices/buildingService";
import { GetTimetableGenerateProgress } from "../../service/timetable/timetableService";

export default {
  name: "TimetableData",
  data: function() {
    return {
      subdivisionList: [],
      versionList: [],
      version: {},
      buildingList:[],
      isLoading:false,
      timetableGenerateProgress: {
        message: "",
      },         
    };
  },
  components: {
    Teachers,
    Subjects,
    StudyClasses,
    Buildings,
    Classrooms
  },
  computed: {},
  methods: {
    getCurrentEmployeeSubdivisionList() {
      this.isLoading = true;
      GetCurrentEmployeeSubdivisionList()
        .then(response => {
          this.subdivisionList = response.data;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getVersionList() {
      this.isLoading = true;
      GetVersionList()
        .then(response => {
          this.versionList = response.data;
          if (response.data.length > 0) {
            this.version = response.data.find(v => v.isActive);
          }
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getBuildingList() {
      this.isLoading = true;
      GetBuildingList()
        .then(response => {
          this.buildingList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные", error);
        })
        .finally(() => {
          this.isLoading = false;
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
  },
  created() {
    this.getVersionList();
    this.getTimetableGenerateProgress();
    this.getCurrentEmployeeSubdivisionList();
  }
};
</script>

<style scoped>
.timetabledata-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
