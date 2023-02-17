<template>
  <div class="lesson-manage-content" v-if="$Role.EmployeeHasRole('employee')">
    <h5 class="mt-2 text-left">Управление занятиями</h5>
    <template v-if="!hideTabs">
      <div v-if="timetableGenerateProgress.start">
        <h5>В данный момент запущена автогенерация расписания. Необходимо дождаться остановки процесса генерации для внесения изменений.</h5>
      </div>      
      <template v-if="!timetableGenerateProgress.start">
      <div class="mt-3 text-left">Выберите версию и группу</div>
      <div class="row mt-3 text-left">
        <div class="col-4">
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
        <div class="col-2">
          <div class="mr-3">Группа</div>
          <multiselect
            v-model="studyClass"
            :options="studyClassList"
            :allowEmpty="false"
            track-by="id"
            label="name"
            :show-labels="false"
            placeholder="Выбрать группу"
            :multiple="false"
          >
            <template slot="noResult">Группа не найдена!</template>
          </multiselect>
        </div>
      </div>
      <hr />        
        <div>
          <ul class="nav nav-tabs">
            <li class="nav-item">
              <a class="nav-link active" data-toggle="tab" href="#addLesson">
                Управление занятиями
                <strong>{{studyClass.name}}</strong>
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" data-toggle="tab" href="#addFlow">
                Управление потоками
                <strong>{{studyClass.name}}</strong>
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" data-toggle="tab" href="#addParallel">
                Управление параллелями
                <strong>{{studyClass.name}}</strong>
              </a>
            </li>
          </ul>

          <div class="tab-content">
            <div class="tab-pane fade show active" id="addLesson">
              <MainLesson
                :version="version"
                :studyClass="studyClass"
                :lessonTypeList="lessonTypeList"
                :subjectList="subjectList"
                :teacherList="teacherList"
              />
            </div>
            <div class="tab-pane fade show" id="addFlow">
              <FlowLesson
                :version="version"
                :studyClass="studyClass"
                :lessonTypeList="lessonTypeList"
                :subjectList="subjectList"
                :teacherList="teacherList"
                :studyClassList="studyClassList"
              />
            </div>
            <div class="tab-pane fade show" id="addParallel">
              <ParallelLesson
                :version="version"
                :studyClass="studyClass"
                :lessonTypeList="lessonTypeList"
                :subjectList="subjectList"
                :teacherList="teacherList"
              />
            </div>
          </div>
        </div>
      </template>  
    </template>
    <div v-if="hideTabs && !isLoading" class="text-center mt-5">
      <h5>Чтобы добавлять занятия, у Вас должно быть добавлено минимум по одной дисциплине, преподавателю и группе!</h5>
    </div>
    <div class="text-center" v-if="isLoading">
      <b-spinner variant="primary" label="Text Centered"></b-spinner>
      <strong>Загрузка...</strong>
    </div>    
  </div>
</template>

<script>
import FlowLesson from "./LessonManage/FlowLesson";
import MainLesson from "./LessonManage/MainLesson";
import ParallelLesson from "./LessonManage/ParallelLesson";
import { GetStudyClassList } from "../../service/timetableDataServices/studyClassService";
import { GetLessonTypeList } from "../../service/lessonService";
import { GetSubjectList } from "../../service/timetableDataServices/subjectService";
import { GetTeacherList } from "../../service/timetableDataServices/teacherService";
import { GetVersionList } from "../../service/versionService";
import { GetTimetableGenerateProgress } from "../../service/timetable/timetableService";
export default {
  name: "LessonManage",
  components: {
    FlowLesson,
    MainLesson,
    ParallelLesson
  },
  data: function() {
    return {
      studyClassList: [],
      versionList: [],
      version: {},
      lessonTypeList: [],
      subjectList: [],
      teacherList: [],
      studyClass: {},
      isLoading: false,
      timetableGenerateProgress: {
        message: "",
      },            
    };
  },
  computed: {
    hideTabs: function() {
      if (
        this.studyClassList.length < 1 ||
        this.lessonTypeList.length < 1 ||
        this.subjectList.length < 1 ||
        this.teacherList.length < 1
      ) {
        return true;
      } else {
        return false;
      }
    }
  },
  methods: {
    getStudyClassList() {
      this.isLoading = true;
      GetStudyClassList()
        .then(response => {
          this.studyClassList = response.data;
          if (response.data.length > 0) {
            this.studyClass = response.data[0];
          }
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
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
    getLessonTypeList() {
      GetLessonTypeList().then(response => {
        this.lessonTypeList = response.data;
      });
    },
    getSubjectList() {
      GetSubjectList().then(response => {
        this.subjectList = response.data;
      });
    },
    getTeacherList() {
      GetTeacherList().then(response => {
        this.teacherList = response.data;
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
    this.getStudyClassList();
    this.getLessonTypeList();
    this.getSubjectList();
    this.getTeacherList();
  },
  props: {}
};
</script>

<style scoped>
.lesson-manage-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
