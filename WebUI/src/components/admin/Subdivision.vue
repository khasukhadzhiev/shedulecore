<template>
  <div class="subdivision-content" v-if="$Role.EmployeeHasRole('admin')">
    <h5 class="mt-2 text-left">
      Подразделения
    </h5>    
    <div class="row mt-3 text-left" v-if="showAddBlock">
      <div class="col-6">
        <label>Введите наименование подразделения</label>
        <b-form-input
          v-model="subdivision.name"
          @input="subdivision.name = subdivision.name.toUpperCase()"
        ></b-form-input>
      </div>
    </div>
    <hr>
    <div class="row mb-3 mt-3 text-right">
      <div class="col">
        <a
          href="#"
          @click="showAddBlock = !showAddBlock"
          class="pr-2"
        >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
        <b-button
          variant="outline-primary"
          @click="addSubdivision"
          v-if="showAddBlock"
        >Добавить подразделение</b-button>
      </div>
    </div>

    <b-table
      hover
      outlined
      head-variant="light"
      :items="subdivisionList"
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
        <span class="btn" @click="removeSubdivision(data.item.id)">
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
            <b-button variant="outline-secondary" class="mr-3" @click="getSubdivisionList()">Отмена</b-button>
            <b-button variant="outline-primary" @click="editSubdivision(row)">Сохранить</b-button>
          </div>          
        </div>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetSubdivisionList,
  AddSubdivision,
  EditSubdivision,
  RemoveSubdivision
} from "../../service/subdivisionService";

export default {
  name: "Subdivision",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "name", label: "Наименование подразделения", sortable: true },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      subdivision: {
        name: ""
      },
      isLoading: false,
      subdivisionList: [],
      showAddBlock: false
    };
  },
  computed: {},
  methods: {
    getSubdivisionList() {
      this.isLoading = true;
      GetSubdivisionList()
        .then(response => {
          this.subdivisionList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    addSubdivision() {
      let isEmpty = this.subdivision.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Необходимо ввести наименование.");
        return;
      }

      this.isLoading = true;
      AddSubdivision(this.subdivision)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Подразделение сохранено!");
            this.getSubdivisionList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении подразделения.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeSubdivision(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить подразделение?", {
          title: "Удаление подразделения",
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
            RemoveSubdivision(id)
              .then(() => {
                this.$ntf.Success("Подразделение удалено!");
                this.getSubdivisionList();
              })
              .catch(error => {
                this.$ntf.Error("Ошибка при удалении подразделения.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editSubdivision(row) {

      let isEmpty = row.item.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Необходимо ввести наименование.");
        return;
      }      
      this.isLoading = true;
      EditSubdivision(row.item)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Подразделение сохранено!");
            this.getSubdivisionList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении подразделения.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  },
  created() {
    this.getSubdivisionList();
  },
  props: {}
};
</script>
<style lang="css" scoped>
.subdivision-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
