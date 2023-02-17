<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div v-if="buildingList.length < 1" class="text-center mt-5">
      <h5>Чтобы добавлять аудитории, необходимо добавить хотя бы один учебный корпус.</h5>
    </div>
    <div v-else>
      <div class="row mt-3 text-left" v-if="showAddBlock">
        <div class="col">
          <label>Корпус</label>
          <b-form-select
            v-model="classroom.buildingId"
            :options="buildingList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
        <div class="col">
          <label>Наименование аудитории</label>
          <b-form-input
            v-model="classroom.name"
            @input="classroom.name = classroom.name.toUpperCase()"
          ></b-form-input>
        </div>
        <div class="col">
          <label>Количество посадочных мест</label>
          <b-form-input v-model.number="classroom.seatsCount" type="number" min="1"></b-form-input>
        </div>
        <div class="col">
          <label>Тип аудитории</label>
          <b-form-select
            v-model="classroom.classroomTypeId"
            :options="classroomTypeList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
      </div>
      <hr v-if="showAddBlock" />
      <div class="row mb-3 mt-3 text-right">
        <div class="col">
          <a
            href="#"
            @click="changeShowAddBlock"
            class="pr-2"
          >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
          <b-button
            variant="outline-primary"
            @click="addClassroom"
            v-if="showAddBlock"
          >Добавить аудиторию</b-button>
        </div>
      </div>

      <b-table
        hover
        outlined
        head-variant="light"
        :items="classroomList"
        :fields="fields"
        :busy="isLoading"
        class="text-left"
      >
        <template v-slot:cell(index)="data">{{data.index + 1}}</template>
        <template v-slot:cell(building)="data">{{data.item.buildingDto.name}}</template>
        <template v-slot:table-busy>
          <div class="text-center my-2">
            <b-spinner variant="primary" label="Text Centered"></b-spinner>
          </div>
        </template>

        <template v-slot:cell(type)="row">{{getClassroomTypeName(row.item.classroomTypeId)}}</template>

        <template v-slot:cell(remove)="data" style="width:50px">
          <span class="btn" @click="removeClassroom(data.item.id)">
            <b-icon-trash variant="danger"></b-icon-trash>
          </span>
        </template>

        <template v-slot:cell(show_details)="row" style="width:50px">
          <span class="btn" @click="row.toggleDetails">
            <b-icon icon="pencil"></b-icon>
          </span>
        </template>

        <template v-slot:row-details="row">
          <div class="row text-left">
            <div class="col">
              <b-form-select
                v-model="row.item.buildingId"
                :options="buildingList"
                value-field="id"
                text-field="name"
                disabled-field="notEnabled"
              ></b-form-select>
            </div>            
            <div class="col">
              <b-form-input
                v-model="row.item.name"
                @input="row.item.name = row.item.name.toUpperCase()"
              ></b-form-input>
            </div>
            <div class="col">
              <b-form-input v-model.number="row.item.seatsCount" type="number" min="1"></b-form-input>
            </div>
            <div class="col">
              <b-form-select
                v-model="row.item.classroomTypeId"
                :options="classroomTypeList"
                class="mb-3"
                value-field="id"
                text-field="name"
                disabled-field="notEnabled"
              ></b-form-select>
            </div>
          </div>
          <div class="row mt-2 text-right">
            <div class="col">
              <b-button variant="outline-secondary" class="mr-3" @click="getClassroomList()">Отмена</b-button>
              <b-button variant="outline-primary" @click="editClassroom(row)">Сохранить</b-button>
            </div>
          </div>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script>
import {
  GetClassroomList,
  GetClassroomTypeList,
  AddClassroom,
  EditClassroom,
  RemoveClassroom,
} from "../../../service/timetableDataServices/classroomService";

export default {
  name: "Classrooms",
  data: function () {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "building", label: "Корпус" },
        { key: "name", label: "Наименование аудитории" },
        { key: "seatsCount", label: "Количество посадочных мест" },
        { key: "type", label: "Вид аудитории" },
        { key: "show_details", label: "" },
        { key: "remove", label: "" },
      ],
      classroom: {
        name: "",
        seatsCount: 1,
        classroomTypeId: null,
        buildingId: null,
      },
      isLoading: false,
      classroomList: [],
      classroomTypeList: [],
      showAddBlock: false,
    };
  },
  computed: {},
  methods: {
    getClassroomTypeName(id) {
      let classroomType = this.classroomTypeList.find((x) => x.id === id);
      if (classroomType) {
        return classroomType.name;
      }
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
    getClassroomTypeList() {
      GetClassroomTypeList()
        .then((response) => {
          this.classroomTypeList = response.data;
          this.classroom.classroomTypeId = response.data[0].id;
        })
        .catch((error) => {
          this.$ntf.Error("Неудалось получить данные.", error);
        });
    },
    changeShowAddBlock() {
      this.showAddBlock = !this.showAddBlock;
    },
    addClassroom() {
      let isEmptyName = this.classroom.name.replace(/\s/g, "");
      let isEmptySeatsCount = this.classroom.seatsCount < 1;

      if (isEmptyName === "" || isEmptySeatsCount) {
        this.$ntf.Warn(
          "Наименование и количетство посадочных мест должно быть заполнено."
        );
        return;
      }

      this.isLoading = true;
      AddClassroom(this.classroom)
        .then((response) => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Аудитория сохранена!");
            this.getClassroomList();
          }
        })
        .catch((error) => {
          this.$ntf.Error("Ошибка при сохранении аудитории.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeClassroom(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить аудиторию?", {
          title: "Удаление аудитории",
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
            this.isLoading = true;
            RemoveClassroom(id)
              .then(() => {
                this.$ntf.Success("Аудитория удалена!");
                this.getClassroomList();
              })
              .catch((error) => {
                this.$ntf.Error("Ошибка при удалении аудитории.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editClassroom(row) {
      let isEmptyName = row.item.name.replace(/\s/g, "");
      let isEmptySeatsCount = row.item.seatsCount < 1;

      if (isEmptyName === "" || isEmptySeatsCount) {
        this.$ntf.Warn(
          "Наименование и количетство посадочных мест должно быть заполнено."
        );
        return;
      }

      this.isLoading = true;
      EditClassroom(row.item)
        .then((response) => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Аудитория сохранена!");
            this.getClassroomList();
          }
        })
        .catch((error) => {
          this.$ntf.Error("Ошибка при сохранении аудитории.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
  },
  created() {
    this.getClassroomTypeList();
    this.getClassroomList();
    if (this.buildingList.length > 0) {
      this.classroom.buildingId = this.buildingList[0].id;
    }
  },
  props: {
    buildingList: Array,
  },
  watch: {
    buildingList: function() {
      if (this.buildingList.length > 0) {
        this.classroom.buildingId = this.buildingList[0].id;
      }
    },
  }
};
</script>