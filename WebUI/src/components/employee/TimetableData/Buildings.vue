<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="row mt-3 text-left" v-if="showAddBlock">
      <div class="col-6">
        <label>Введите наименование корпуса</label>
        <b-form-input
          v-model="building.name"
          @input="building.name = building.name.toUpperCase()"
        ></b-form-input>
      </div>
    </div>
    <hr v-if="showAddBlock"/>
    <div class="row mb-3 mt-3 text-right">
      <div class="col">
        <a
          href="#"
          @click="showAddBlock = !showAddBlock"
          class="pr-2"
        >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
        <b-button
          variant="outline-primary"
          @click="addBuilding"
          v-if="showAddBlock"
        >Добавить корпус</b-button>
      </div>
    </div>

    <b-table
      hover
      outlined
      head-variant="light"
      :items="buildingList"
      :fields="fields"
      :busy="isLoading"
      class="text-left"
    >
      <template v-slot:cell(index)="data">{{data.index + 1}}</template>
      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>

      <template v-slot:cell(remove)="data" style="width:50px">
        <span class="btn" @click="removeBuilding(data.item.id)">
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
            <b-form-input
              v-model="row.item.name"
              @input="row.item.name = row.item.name.toUpperCase()"
            ></b-form-input>
          </div>
          <div class="col">
            <b-button variant="outline-secondary" class="mr-3" @click="getBuildingList()">Отмена</b-button>
            <b-button variant="outline-primary" @click="editBuilding(row)">Сохранить</b-button>
          </div>          
        </div>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetBuildingList,
  AddBuilding,
  EditBuilding,
  RemoveBuilding
} from "../../../service/timetableDataServices/buildingService";

export default {
  name: "Buildings",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "name", label: "Наименование корпуса", sortable: true },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      building: {
        name: ""
      },
      isLoading: false,
      buildingList: [],
      showAddBlock: false
    };
  },
  computed: {},
  methods: {
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
    addBuilding() {
      let isEmpty = this.building.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Необходимо ввести наименование.");
        return;
      }

      this.isLoading = true;
      AddBuilding(this.building)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Корпус сохранен!");
            this.getBuildingList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении корпуса.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeBuilding(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить корпус?", {
          title: "Удаление корпуса",
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
            this.isLoading = true;
            RemoveBuilding(id)
              .then(() => {
                this.$ntf.Success("Корпус удален!");
                this.getBuildingList();
              })
              .catch(error => {
                this.$ntf.Error("Ошибка при удалении корпуса.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editBuilding(row) {

      let isEmpty = row.item.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Необходимо ввести наименование.");
        return;
      }      
      this.isLoading = true;
      EditBuilding(row.item)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Корпус сохранен!");
            this.getBuildingList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении корпуса.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  },
  created() {
    this.getBuildingList();
  },
  props: {}
};
</script>
<style lang="css" scoped>
.building-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
