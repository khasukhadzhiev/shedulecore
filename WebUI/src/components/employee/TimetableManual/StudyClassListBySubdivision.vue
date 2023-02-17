<template v-if="$Role.EmployeeHasRole('employee')">
  <div class="stutyclass-list-by-subdivision-content">
    <h5 class="mt-2 text-left">Расписание</h5>
    <div v-if="sortedStudyClassList.length > 0" class="mt-3 text-left">
      <label>Выберите группу</label>

      <div
        class="row"
        v-for="studyClassList in sortedStudyClassList"
        v-bind:key="studyClassList[0].id"
      >
        <div class="col">
          <b-button
            v-for="item in studyClassList"
            v-bind:key="item.id"
            v-bind:value="item"
            class="m-2 btn-width"
            type="button"
            variant="outline-primary"
            @click="toStudyClassTimetable(item)"
          >
            <span class="classShift-caption" v-if="version.showClassShift">{{item.classShift.name}} смена</span>
            {{item.name}}
          </b-button>
           <hr class="hr-style">
        </div>
      </div>

    </div>
    <div v-if="sortedStudyClassList.length == 0 && !isLoading" class="text-center mt-5">
      <h5>В данном подразделении нет групп.</h5>
    </div>
    <div v-if="isLoading" class="text-center">
      <b-spinner variant="primary" label="Text Centered"></b-spinner>
      <strong>Загрузка...</strong>
    </div>
    <div class="mt-3" v-if="sortedStudyClassList.length > 0">
      <div class="text-left" v-if="!searchMistake">
        <div v-if="studyClassListWithMistakes.length > 0">
          <h6>Накладки в след. группах:</h6>
          <span
            v-for="(item, index) in studyClassListWithMistakes"
            :key="item + index"
          >{{item}}{{(index == studyClassListWithMistakes.length-1)?'':', '}}</span>
        </div>
        <div v-else>Накладок нет</div>
      </div>
      <div class="text-left" v-if="searchMistake">
        <h6>Поиск накладок...</h6>
      </div>
    </div>
  </div>
</template>


<script>
import { GetStudyClassListBySubdivision } from "../../../service/timetableDataServices/studyClassService";
import { GetStudyClassNamesWithMistakes } from "../../../service/timetable/timetableService";
import { GetActiveVersion } from "../../../service/versionService";

export default {
  name: "StudyClassListBySubdivision",
  data: function () {
    return {
      subdivision: {},
      version: {},
      sortedStudyClassList: [],
      studyClassListWithMistakes: [],
      isLoading: false,
      searchMistake: false,
    };
  },
  computed: {},
  methods: {
    getStudyClassListBySubdivision(subdivision) {
      this.isLoading = true;
      GetStudyClassListBySubdivision(subdivision)
        .then((response) => {
          this.sortedStudyClassList = response.data;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getStudyClassNamesWithMistakes() {
      this.searchMistake = true;
      GetStudyClassNamesWithMistakes()
        .then((response) => {
          this.studyClassListWithMistakes = response.data;
        })
        .finally(() => {
          this.searchMistake = false;
        });
    },
    toStudyClassTimetable(studyClass) {
      this.$router.push({
        name: "studyClassTimetable",
        params: { studyClass: studyClass },
      });
    },
    getActiveVersion() {
      GetActiveVersion()
        .then((response) => {
          this.version = response.data;
        })
        .catch(() => {
          this.$ntf.Error("Неудалось получить настройки приложения.", null);
        });
    },    
  },
  created() {
    if (this.$route.params.subdivision) {
      this.subdivision = this.$route.params.subdivision;
      this.getStudyClassListBySubdivision(this.subdivision.id);
      this.getStudyClassNamesWithMistakes();
      this.getActiveVersion();
    } else {
      this.$ntf.Error(
        "Не было передано подразделение для отображения списка.",
        null
      );
      this.$router.push({ name: "timetableManual" });
    }
  },
  props: {},
};
</script>

<style scoped>

.hr-style{
  padding: 0;
  margin: 0.3rem;
}

.btn-width {
  min-width: 7rem;
}

.btn-width:hover {
  min-width: 7rem;
}

.btn-width:hover .classShift-caption {
  display: block;
}

.classShift-caption{
  display: none;
  font-size: 8pt;
  margin-top: -22px;
  z-index: 50;
  position: static;
  background-color:#003670;
  padding: 3px;
  border-radius: 3px;
}

.studyClass-list {
  list-style: none;
  padding: 0;
  margin: 0;
}
.studyClass-list li {
  padding: 0.3rem;
}
.studyClass-list li button {
  min-width: 8rem;
}
.stutyclass-list-by-subdivision-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
