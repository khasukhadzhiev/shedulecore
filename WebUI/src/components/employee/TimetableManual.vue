<template v-if="$Role.EmployeeHasRole('employee')">
  <div class="timetable-manual-content">
    <h5 class="mt-2 text-left">
      Расписание
    </h5>
    <div v-if="subdivisionList.length > 0" class="text-left mt-3">
      <label>Выберите подразделение</label>
      <div class="text-left" v-if="!isLoading">
        <ul class="subdivision-list">
          <li v-for="item in subdivisionList" v-bind:key="item.id">
            <b-button
              variant="outline-primary"
              class="m-1 p-2 btn btn-outline-primary"
              v-bind:value="item.name"
              @click="toStudyClassListBySubdivision(item)"
            >{{item.name}}</b-button>
          </li>
        </ul>
      </div>
    </div>
    <div  v-if="subdivisionList.length == 0 && !isLoading" class="text-center mt-5">
      <h5>Вы не привязаны ни к одному подразделению для формирования расписания. Обратитесь к администратору.</h5>
    </div>
    <div v-if="isLoading" class="text-center">
      <b-spinner variant="primary" label="Text Centered"></b-spinner>
      <strong>Загрузка...</strong>
    </div>
  </div>
</template>

<script>
import { GetCurrentEmployeeSubdivisionList } from "../../service/subdivisionService";

export default {
  name: "TimetableManual",
  data: function() {
    return {
      subdivisionList: [],
      isLoading: false
    };
  },
  computed: {},
  methods: {
    getCurrentEmployeeSubdivisionList() {
      this.isLoading = true;
      GetCurrentEmployeeSubdivisionList()
        .then(response => {
          this.subdivisionList = response.data;
          if (!(this.subdivisionList.length > 1)) {
            this.toStudyClassListBySubdivision(this.subdivisionList[0]);
          }
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    toStudyClassListBySubdivision(subdivision) {
      if(subdivision){
        this.$router.push({
          name: "studyClassListBySubdivision",
          params: { subdivision: subdivision }
        });
      }
    }
  },
  created() {
    this.getCurrentEmployeeSubdivisionList();
  },
  props: {}
};
</script>
<style scoped>
.subdivision-list {
  list-style: none;
  padding: 0;
  margin: 0;
}
.subdivision-list li {
  padding: 0.3rem;
}
.subdivision-list li button {
  min-width: 15rem;
}
.timetable-manual-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>