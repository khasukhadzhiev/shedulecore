<template>
  <div class="lesson-manage-content" v-if="$Role.EmployeeHasRole('employee')">
    <h5 class="mt-2 text-left">Расписание экзаменов</h5>
    <div v-if="timetableGenerateProgress.start" class="mt-2">
      <h5>В данный момент запущена автогенерация расписания. Необходимо дождаться остановки процесса генерации для внесения изменений.</h5>
    </div>    
    <template v-if="!timetableGenerateProgress.start">
      <template v-if="!hideTabs">
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
              v-model="studyClassReport.studyClass"
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
          <div class="mt-3 text-left" v-if="showAddBlock">
            <div class="row">
              <div class="col">
                <label>Дисциплина</label>
                <multiselect
                  v-model="studyClassReport.subject"
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
                  v-model="studyClassReport.teacher"
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
              <div class="col">
                <label>Вид отчетности</label>
                <multiselect
                  v-model="studyClassReport.reportingType"
                  :options="reportingTypeList"
                  :allowEmpty="false"
                  track-by="id"
                  label="name"
                  :show-labels="false"
                  placeholder="Выбрать вид отчетности"
                  :multiple="false"
                >
                  <template slot="noResult">Отчетность не найдена!</template>
                </multiselect>
              </div>
              <div class="col">
                <label>Аудитория</label>
                <multiselect
                  v-model="studyClassReport.classroom"
                  :options="classroomList"
                  track-by="id"
                  :custom-label="customLabel"
                  :show-labels="false"
                  placeholder="Выбрать аудиторию"
                  :multiple="false"
                >
                  <template slot="noResult">Аудитория не найдена!</template>
                </multiselect>
              </div>
              <div class="col">
                <label>Дата проведения</label>
                <datetime
                  v-model="studyClassReport.date"
                  auto
                  placeholder="Выбрать дату"
                  :phrases="{ ok: 'Выбрать', cancel: 'Отменить' }"
                  input-style="min-height: 40px; border-radius: 5px; border: 1px solid #e8e8e8; padding-left: 10px;"
                >
                </datetime>
              </div>
            </div>
          </div>
          <hr v-if="showAddBlock" />
          <div class="row mt-3 mb-3 text-right">
            <div class="col">
              <a href="#" @click="showAddBlock = !showAddBlock" class="pr-2">{{
                showAddBlock
                  ? "Скрыть панель добавления"
                  : "Показать панель добавления"
              }}</a>
              <b-button
                variant="outline-primary"
                @click="addStudyClassReporting"
                v-if="showAddBlock"
                >Добавить отчетность</b-button
              >
            </div>
          </div>
        </div>
      </template>
      <template>
        <b-table
          hover
          outlined
          head-variant="light"
          :items="studyClassReportingList"
          :fields="fields"
          :busy="isLoading"
          class="text-left"
        >
          <template v-slot:cell(index)="data">{{ data.index + 1 }}</template>
          <template v-slot:cell(remove)="data" style="width: 50px">
            <span class="btn" @click="removeStudyClassReporting(data.item.id)">
              <b-icon-trash variant="danger"></b-icon-trash>
            </span>
          </template>

          <template v-slot:cell(show_details)="row" style="width: 50px">
            <span class="btn" @click="row.toggleDetails">
              <b-icon icon="pencil"></b-icon>
            </span>
          </template>

          <template #cell(date)="data">
            {{data.item.date | shortDate}}
          </template>        

          <template #cell(classroom)="data">
            {{data.item.classroom && data.item.classroom.buildingDto.name}} {{data.item.classroom && data.item.classroom.name}}
          </template>                

          <template v-slot:row-details="row">
            <div class="row">
              <div class="col">
                <label>Дисциплина</label>
                <multiselect
                  v-model="row.item.subject"
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
                  v-model="row.item.teacher"
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
              <div class="col">
                <label>Вид отчетности</label>
                <multiselect
                  v-model="row.item.reportingType"
                  :options="reportingTypeList"
                  :allowEmpty="false"
                  track-by="id"
                  label="name"
                  :show-labels="false"
                  placeholder="Выбрать вид отчетности"
                  :multiple="false"
                >
                  <template slot="noResult">Отчетность не найдена!</template>
                </multiselect>
              </div>
              <div class="col">
                <label>Аудитория</label>
                <multiselect
                  v-model="row.item.classroom"
                  :options="classroomList"
                  track-by="id"
                  :custom-label="customLabel"
                  :show-labels="false"
                  placeholder="Выбрать аудиторию"
                  :multiple="false"
                >
                  <template slot="noResult">Аудитория не найдена!</template>
                </multiselect>
              </div>
              <div class="col">
                <label>Дата проведения</label>
                <datetime
                  v-model="row.item.date"
                  auto
                  placeholder="Выбрать дату"
                  :phrases="{ ok: 'Выбрать', cancel: 'Отменить' }"
                  input-style="min-height: 40px; border-radius: 5px; border: 1px solid #e8e8e8; padding-left: 10px;"
                >
                </datetime>
              </div>
            </div>

            <hr />
            <div class="row mt-2 text-right">
              <div class="col">
                <b-button
                  variant="outline-secondary"
                  class="mr-3"
                  @click="getStudyClassReportingList()"
                  >Отмена</b-button
                >
                <b-button
                  variant="outline-primary"
                  @click="editStudyClassReporting(row.item)"
                  >Сохранить</b-button
                >
              </div>
            </div>
          </template>

          <template v-slot:table-busy>
            <div class="text-center my-2">
              <b-spinner variant="primary" label="Text Centered"></b-spinner>
            </div>
          </template>
        </b-table>
      </template>
    </template>    
    <div v-if="hideTabs && !isLoading" class="text-center mt-5">
      <h5>
        Чтобы добавлять экзамены или зачеты, у Вас должно быть добавлено минимум
        по одной дисциплине, преподавателю и группе!
      </h5>
    </div>
  </div>
</template>

<script>
import { GetStudyClassList } from "../../service/timetableDataServices/studyClassService";
import { GetSubjectList } from "../../service/timetableDataServices/subjectService";
import { GetTeacherList } from "../../service/timetableDataServices/teacherService";
import { GetVersionList } from "../../service/versionService";
import { GetClassroomList } from "../../service/timetableDataServices/classroomService";
import { GetTimetableGenerateProgress } from "../../service/timetable/timetableService";
import {
  GetReportingTypeList,
  GetStudyClassReportingList,
  AddStudyClassReporting,
  EditStudyClassReporting,
  RemoveStudyClassReporting,
} from "../../service/reportingTypeService";

export default {
  name: "StudyClassReporting",
  components: {},
  data: function () {
    return {
      showAddBlock: false,
      studyClassList: [],
      versionList: [],
      version: {},
      subjectList: [],
      teacherList: [],
      classroomList: [],
      reportingTypeList: [],
      studyClassReport: {
        studyClass: {},
        subject: {},
        teacher: {},
        reportingType: {},
        classroom: null,
        versionId: Number,
        date: '',
      },
      studyClassReportingList: [],
      fields: [
        { key: "index", label: "№" },
        { key: "subject.name", label: "Дисциплина", sortable: true },
        { key: "teacher.fullName", label: "Преподаватель", sortable: true },
        { key: "reportingType.name", label: "Отчетность", sortable: true },
        { key: "date", label: "Дата" },
        { key: "classroom", label: "Аудитория" },
        { key: "show_details", label: "" },
        { key: "remove", label: "" },
      ],
      isLoading: false,
      timetableGenerateProgress: {
        message: "",
      },            
    };
  },
  computed: {
    hideTabs: function () {
      if (
        this.studyClassList.length < 1 ||
        this.subjectList.length < 1 ||
        this.teacherList.length < 1
      ) {
        return true;
      } else {
        return false;
      }
    },
  },
  methods: {
    customLabel({ buildingDto, name }) {
      if (buildingDto && name) {
        return `${buildingDto.name} ${name}`;
      }
    },
    getStudyClassReportingList() {
      this.isLoading = true;
      GetStudyClassReportingList(
        this.studyClassReport.studyClass.id,
        this.version.id
      )
        .then((response) => {
          this.studyClassReportingList = response.data;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getStudyClassList() {
      this.isLoading = true;
      GetStudyClassList()
        .then((response) => {
          this.studyClassList = response.data;
          if (response.data.length > 0) {
            this.studyClassReport.studyClass = response.data[0];
          }
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
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
          }
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getSubjectList() {
      GetSubjectList().then((response) => {
        this.subjectList = response.data;
        if (response.data.length > 0) {
          this.studyClassReport.subject = response.data[0];
        }
      });
    },
    getTeacherList() {
      GetTeacherList().then((response) => {
        this.teacherList = response.data;
        if (response.data.length > 0) {
          this.studyClassReport.teacher = response.data[0];
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
    getReportingTypeList() {
      GetReportingTypeList().then((response) => {
        this.reportingTypeList = response.data;
        if (response.data.length > 0) {
          this.studyClassReport.reportingType = response.data[0];
        }
      });
    },
    addStudyClassReporting() {
      this.isLoading = true;

      let studyCLassReporting = {
        id:this.studyClassReport.id,
        studyClassId: this.studyClassReport.studyClass.id,
        subjectId: this.studyClassReport.subject.id,
        teacherId: this.studyClassReport.teacher.id,
        reportingTypeId: this.studyClassReport.reportingType.id,
        classroomId: this.studyClassReport.classroom && this.studyClassReport.classroom.id,
        date: this.studyClassReport.date,
        versionId: this.version.id,
      };

      AddStudyClassReporting(studyCLassReporting)
        .then(() => {
          this.$ntf.Success("Отчетность добавлена!");
          this.getStudyClassReportingList();
        })
        .catch((error) => {
          this.$ntf.Error("Ошибка при добавлении отчетности.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    editStudyClassReporting(studyClassReport) {
      this.isLoading = true;

      let studyCLassReporting = {
        id: studyClassReport.id,
        studyClassId: studyClassReport.studyClass.id,
        subjectId: studyClassReport.subject.id,
        teacherId: studyClassReport.teacher.id,
        reportingTypeId: studyClassReport.reportingType.id,
        classroomId: studyClassReport.classroom && studyClassReport.classroom.id,
        date: studyClassReport.date,
        versionId: this.version.id,
      };

      EditStudyClassReporting(studyCLassReporting)
        .then(() => {
          this.$ntf.Success("Отчетность обновлена!");
          this.getStudyClassReportingList();
        })
        .catch((error) => {
          this.$ntf.Error("Ошибка при обновлении отчетности.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeStudyClassReporting(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить отчетность?", {
          title: "Удаление отчетности",
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
            RemoveStudyClassReporting(id)
              .then(() => {
                this.$ntf.Success("Отчетность удалена!");
                this.getStudyClassReportingList();
              })
              .catch((error) => {
                this.$ntf.Error("Неудалось удалить отчетность.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
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
    this.getSubjectList();
    this.getTeacherList();
    this.getClassroomList();
    this.getReportingTypeList();
  },
  props: {},
  watch: {
    "studyClassReport.studyClass": function () {
      if ("id" in this.studyClassReport.studyClass) {
        this.getStudyClassReportingList();
      }
    },
    version: function () {
      if ("id" in this.version) {
        this.getStudyClassReportingList();
      }
    },
  },
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
