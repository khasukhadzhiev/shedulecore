<template>
  <div class="reports-content" v-if="$Role.EmployeeHasRole('employee')">
    <h5 class="mt-2 text-left">Отчеты</h5>
    <ul class="nav nav-tabs mt-4">
      <li class="nav-item">
        <a class="nav-link active" data-toggle="tab" href="#classroomBusy">Занятость аудиторий</a>
      </li>
    </ul>

    <div class="tab-content">
      <div class="tab-pane fade show active" id="classroomBusy">
        <ClassroomBusy />
      </div>
    </div>
  </div>
</template>

<script>
import ClassroomBusy from "../employee/Reports/ClassroomBusy";

export default {
  name: "Reports",
  data: function () {
    return {};
  },
  computed: {},
  components: {
    ClassroomBusy
  },  
  methods: {},
  created() {},
  props: {},
};
</script>

<style scoped>
.reports-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
