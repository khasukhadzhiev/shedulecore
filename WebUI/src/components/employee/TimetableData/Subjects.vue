<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="row mt-3 text-left" v-if="showAddBlock">
      <div class="col-6">
        <label>Наименование дисциплины</label>
        <b-form-input v-model="subject.name" @input="subject.name = subject.name.toUpperCase()"></b-form-input>
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
          @click="addSubject"
          v-if="showAddBlock"
        >Добавить дисциплину</b-button>
      </div>
    </div>

    <b-table
      hover
      outlined
      head-variant="light"
      :items="subjectList"
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
        <span class="btn" @click="removeSubject(data.item.id)">
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
              v-model.trim="row.item.name"
              @input="row.item.name = row.item.name.toUpperCase()"
            ></b-form-input>
          </div>
        </div>
        <div class="row mt-2 text-right">
          <div class="col">
            <b-button variant="outline-secondary" class="mr-3" @click="getSubjectList()">Отмена</b-button>
            <b-button variant="outline-primary" @click="editSubject(row)">Сохранить</b-button>
          </div>
        </div>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetSubjectList,
  AddSubject,
  EditSubject,
  RemoveSubject
} from "../../../service/timetableDataServices/subjectService";

export default {
  name: "Subjects",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "name", label: "Наименование дисциплины" },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      subject: {
        name: ""
      },
      isLoading: false,
      subjectList: [],
      showAddBlock: false
    };
  },
  computed: {},
  methods: {
    getSubjectList() {
      this.isLoading = true;
      GetSubjectList()
        .then(response => {
          this.subjectList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    changeShowAddBlock() {
      this.showAddBlock = !this.showAddBlock;
    },
    addSubject() {
      let isEmpty = this.subject.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Наименование не может быть пустым.");
        return;
      }
      this.isLoading = true;
      AddSubject(this.subject)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Дисциплина сохранена!");
            this.getSubjectList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении дисциплины.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeSubject(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить дисциплину?", {
          title: "Удаление дисциплины",
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
            RemoveSubject(id)
              .then(() => {
                this.$ntf.Success("Дисциплина удалена!");
                this.getSubjectList();
              })
              .catch(error => {
                this.$ntf.Error("Ошибка при удалении дисциплины.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editSubject(row) {
      let isEmpty = row.item.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Наименование не может быть пустым.");
        return;
      }
      this.isLoading = true;
      EditSubject(row.item)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Дисциплина сохранена!");
            this.getSubjectList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении дисциплины.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  },
  created() {
    this.getSubjectList();
  },
  props: {}
};
</script>
