<template>
  <div class="manage-content" v-if="$Role.EmployeeHasRole('admin')">
    <h5 class="mt-2 text-left">Управление нагрузкой</h5>

    <ul class="nav nav-tabs mt-4">
      <li class="nav-item">
        <a class="nav-link active" data-toggle="tab" href="#autoTimetable">Авто расписание</a>
      </li>
    </ul>

    <div class="tab-content">
      <div class="tab-pane fade show active" id="autoTimetable">
         <AutoTimetable />
      </div>
    </div>
  </div>
</template>

<script>
import AutoTimetable from "../admin/DataManage/AutoTimetable";

export default {
  name: "DataManage",
  data: function() {
    return {};
  },
  computed: {},
  components: {
    AutoTimetable
  },
  methods: {},
  created() {},
  props: {}
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
