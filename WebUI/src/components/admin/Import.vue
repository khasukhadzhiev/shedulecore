
<template v-if="$Role.EmployeeHasRole('admin')">
    <div class="import-content">
      <div>
          <ul class="nav nav-tabs">
            <li class="nav-item">
              <a class="nav-link active" data-toggle="tab" href="#dataImport">
                Импорт данных
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" data-toggle="tab" href="#classroomImport">
                Импорт аудиторий
              </a>
            </li>
          </ul>

          <div class="tab-content">
            <div class="tab-pane fade show active" id="dataImport">
              <DataImport/>
            </div>
            <div class="tab-pane fade show" id="classroomImport">
              <ClassroomImport/>
            </div>
          </div>
        </div>



    </div>
</template>

<script>
import DataImport from "./Import/DataImport.vue";
import ClassroomImport from "./Import/ClassroomImport.vue";

export default {
  name: "Import",
  data: function () {
    return {};
  },
  computed: {},
  components: {
    DataImport,
    ClassroomImport
  },  
  methods: {},
  created() {},
  props: {},
};
</script>

<style scoped>
.import-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>