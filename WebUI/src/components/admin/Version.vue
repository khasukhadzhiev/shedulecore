<template v-if="$Role.EmployeeHasRole('admin')">
  <div class="version-content">
    <div>
      <h5 class="mt-2 text-left">Версии расписания</h5>
      <div class="row mt-3 text-left" v-if="showAddBlock">
        <div class="col">
          <label>Введите наименование версии</label>
          <b-form-input
            v-model.trim="version.name"
            type="text"
            @input="version.name = version.name.toUpperCase()"
          ></b-form-input>
        </div>
        <div class="col pt-3">
          <b-form-checkbox
            v-model="version.isActive"
            class="pr-3 mt-4"
          >Переключиться на новое расписание</b-form-checkbox>
        </div>
      </div>
      <hr />
      <div class="row mb-3 mt-3 text-right">
        <div class="col">
          <a
            href="#"
            @click="showAddBlock = !showAddBlock"
            class="pr-2"
          >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
          <b-button
            variant="outline-primary"
            @click="addVersion"
            v-if="showAddBlock"
          >Добавить версию</b-button>
        </div>
      </div>

      <b-table
        hover
        outlined
        head-variant="light"
        :items="versionList"
        :fields="fields"
        :busy="isLoading"
        class="text-left"
      >
        <template v-slot:cell(index)="data">{{data.index + 1}}</template>
        <template v-slot:cell(isActive)="data">{{data.item.isActive ? 'Активно' : 'Нет'}}</template>
        <template v-slot:table-busy>
          <div class="text-center my-2">
            <b-spinner variant="primary" label="Text Centered"></b-spinner>
          </div>
        </template>

        <template v-slot:cell(remove)="data" style="width:50px">
          <span class="btn" @click="removeVersion(data.item.id)">
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
            <div class="col">
              <b-form-checkbox v-model="row.item.isActive" class="pr-3 mt-2">Активно</b-form-checkbox>
            </div>
          </div>
          <hr />
          <div class="row text-left">
            <div class="col">
              <label>Макс. количество занятий в день</label>
              <b-form-input v-model.number="row.item.maxLesson" type="number" min="2"></b-form-input>
            </div>
            <div class="col">
              <b-form-checkbox
                v-model="row.item.useSunday"
                class="pr-3 mt-4"
              >Использовать воскресенье в расписании</b-form-checkbox>
            </div>
            <div class="col">
              <b-form-checkbox
                v-model="row.item.useSubWeek"
                class="pr-3 mt-4"
              >Использовать деление на недели</b-form-checkbox>
            </div>
            <div class="col">
              <b-form-checkbox
                v-model="row.item.useSubClass"
                class="pr-3 mt-4"
              >Использовать деление на подгруппы</b-form-checkbox>
            </div>
          </div>
          <hr />
          <div class="row text-left">
            <div class="col">
              <form class="form-inline">
                <b-form-checkbox
                  v-model="row.item.showEducationForm"
                  class="pr-3"
                >Показывать форму обучения</b-form-checkbox>
                <b-form-checkbox
                  v-model="row.item.showClassShift"
                  class="pr-3"
                >Показывать смену обучения</b-form-checkbox>
              </form>
            </div>
            <div class="col">
              <div class="mb-3">Выводить расписание:</div>  
                <b-form-checkbox-group
                  id="checkbox-group"
                  v-model="row.item.showReportingIds"
                >
                          <b-form-checkbox class="pr-4" value="1" v-model="row.item.showReportingIds">Экзаменов</b-form-checkbox>
                          <b-form-checkbox class="pr-4" value="2" v-model="row.item.showReportingIds">Зачетов</b-form-checkbox>    
                          <b-form-checkbox class="pr-4" value="3" v-model="row.item.showReportingIds">Зачетов с оценкой</b-form-checkbox>
                </b-form-checkbox-group>                         
            </div>
          </div>    
          <hr />
          <div class="row text-right">
            <div class="col">
              <b-button variant="outline-secondary" class="mr-3" @click="getVersionList()">Отмена</b-button>
              <b-button variant="outline-primary" @click="editVersion(row.item)">Сохранить</b-button>
            </div>
          </div>
        </template>
      </b-table>
    </div>
  </div>
</template>

<script>
import {
  AddVersion,
  EditVersion,
  RemoveVersion,
  GetVersionList
} from "../../service/versionService";

export default {
  name: "Version",
  data: function() {
    return {
      isLoading: false,
      fields: [
        { key: "index", label: "№" },
        { key: "name", label: "Наименование расписания", sortable: true  },
        { key: "isActive", label: "Активная версия", sortable: true  },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      version: {
        name:""
      },
      versionList: [],
      showAddBlock: false
    };
  },
  components: {},
  computed: {},
  methods: {
    getVersionList() {
      this.isLoading = true;
      GetVersionList()
        .then(response => {
          this.versionList = response.data;
        })
        .catch(() => {
          this.$ntf.Error("Неудалось получить версии расписания.", null);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    addVersion() {
      let isEmpty = this.version.name.replace(/\s/g, "");

      if (isEmpty === "") {
        this.$ntf.Warn("Необходимо ввести наименование.");
        return;
      }

      this.isLoading = true;
      AddVersion(this.version)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Версия сохранена!");
            this.getVersionList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Неудалось сохранить.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    editVersion(version) {
      let isEmpty = version.name.replace(/\s/g, "");

      if (isEmpty === "" || version.maxLesson < 2) {
        this.$ntf.Warn("Наименование должно быть заполнены. Максимальное колчество занятий в день должно быть больше 1.");
        return;
      }      
      this.isLoading = true;
      EditVersion(version)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Версия сохранена!");
            this.getVersionList();
          }
        })
        .catch(error => {
          this.$ntf.Error("Неудалось сохранить.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeVersion(id) {
      this.$bvModal
        .msgBoxConfirm(
          "Вы уверены, что хотите удалить версию расписания? Вы не сможете удалить версию, если на ней создано расписание.",
          {
            title: "Удаление версии расписания",
            size: "sm",
            buttonSize: "sm",
            okVariant: "danger",
            okTitle: "ДА",
            cancelTitle: "НЕТ",
            footerClass: "p-2",
            hideHeaderClose: false,
            centered: true
          }
        )
        .then(value => {
          if (value) {
            this.isLoading = true;
            RemoveVersion(id)
              .then(() => {
                this.$ntf.Success("Версия удалена!");
              })
              .catch(error => {
                this.$ntf.Error("Ошибка при удалении версии.", error);
              })
              .finally(() => {
                this.getVersionList();
                this.isLoading = false;
              });
          }
        });
    }
  },
  created() {
    this.getVersionList();
  },
  props: {}
};
</script>

<style scoped>
.version-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
