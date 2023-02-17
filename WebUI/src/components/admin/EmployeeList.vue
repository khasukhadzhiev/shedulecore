<template>
  <div class="employee-content" v-if="$Role.EmployeeHasRole('admin')">
    <h5 class="mt-2 text-left">Сотрудники</h5>
    <div class="mt-3 text-left" v-if="showAddBlock">
      <label>Введите данные сотрудника</label>
      <div class="row text-left mt-2">
        <div class="col">
          <label>Фамилия</label>
          <b-form-input
            v-model="employee.firstName"
            required 
            @input="employee.firstName = employee.firstName.toUpperCase()"
          ></b-form-input>
        </div>
        <div class="col">
          <label>Имя</label>
          <b-form-input
            v-model="employee.name"
            @input="employee.name = employee.name.toUpperCase()"
          ></b-form-input>
        </div>
        <div class="col">
          <label>Отчество</label>
          <b-form-input
            v-model="employee.middleName"
            @input="employee.middleName = employee.middleName.toUpperCase()"
          ></b-form-input>
        </div>
      </div>
      <hr />
      <div class="row">
        <div class="col text-left">
          Роли пользователя
          <form class="form-inline mt-2">
            <b-form-checkbox
              class="mr-2"
              v-for="item in roleList"
              v-bind:key="item.id"
              v-bind:value="item"
              v-model="employee.roles"
            >{{item.displayName}}</b-form-checkbox>
            <b-form-checkbox v-model="employee.isValid" class="pr-3 ml-5">Действующий пользователь</b-form-checkbox>
          </form>
        </div>
      </div>
      <hr />
      <div class="row text-left mt-3">
        <div class="col-3">
          <label>Логин</label>
          <b-form-input v-model="employee.account.login"></b-form-input>
        </div>
        <div class="col-3">
          <label>Пароль</label>
          <b-form-input id="password-input" v-model="employee.account.password"></b-form-input>
        </div>
      </div>
      <hr />
      <div class="row">
        <div class="col text-left">
          Связанные подразделения
          <div class="mt-2 text-left">
            <ul class="subdivision-list">
              <li v-for="item in subdivisionList" v-bind:key="item.id" class="text-left">
                <b-form-checkbox
                  v-bind:value="item"
                  v-model="employee.subdivisionList"
                >{{item.name}}</b-form-checkbox>
              </li>
            </ul>
          </div>
        </div>
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
          @click="addEmployee"
          v-if="showAddBlock"
        >Добавить сотрудника</b-button>
      </div>
    </div>

    <b-table
      hover
      outlined
      head-variant="light"
      :items="employeeList"
      :fields="fields"
      :busy="isLoading"
      class="text-left"
    >
      <template v-slot:cell(index)="data">{{data.index + 1}}</template>
      <template v-slot:cell(remove)="data" style="width:50px">
        <span class="btn" @click="removeEmployee(data.item.id)">
          <b-icon-trash variant="danger"></b-icon-trash>
        </span>
      </template>

      <template v-slot:cell(show_details)="row" style="width:50px">
        <span class="btn" @click="row.toggleDetails">
          <b-icon icon="pencil"></b-icon>
        </span>
      </template>

      <template v-slot:row-details="row">
        <div class="mt-3 text-left">
          <div class="row text-left mt-2">
            <div class="col text-left">
              <label>Фамилия</label>
              <b-form-input
                v-model="row.item.firstName"
                @input="row.item.firstName = row.item.firstName.toUpperCase()"
              ></b-form-input>
            </div>
            <div class="col text-left">
              <label>Имя</label>
              <b-form-input
                v-model="row.item.name"
                @input="row.item.name = row.item.name.toUpperCase()"
              ></b-form-input>
            </div>
            <div class="col text-left">
              <label>Отчество</label>
              <b-form-input
                v-model="row.item.middleName"
                @input="row.item.middleName = row.item.middleName.toUpperCase()"
              ></b-form-input>
            </div>
          </div>
          <hr />
          <div class="row">
            <div class="col text-left">
              Роли пользователя
              <form class="form-inline mt-2">
                <b-form-checkbox
                  class="mr-2"
                  v-for="item in roleList"
                  v-bind:key="item.id"
                  v-bind:value="item"
                  v-model="row.item.roles"
                >{{item.displayName}}</b-form-checkbox>

                <b-form-checkbox
                  v-model="row.item.isValid"
                  class="pr-3 ml-5"
                >Действующий пользователь</b-form-checkbox>
                <b-form-checkbox
                  v-model="changePassword"
                  @click="changePassword != changePassword"
                  class="pr-3"
                >Изменить пароль</b-form-checkbox>
              </form>
            </div>
          </div>
          <hr />
          <div class="row text-left mt-3">
            <div class="col-3">
              <label>Логин</label>
              <b-form-input v-model="row.item.account.login"></b-form-input>
            </div>
            <div class="col-3" v-if="changePassword">
              <label>Пароль</label>
              <b-form-input id="password-input" v-model="row.item.account.password"></b-form-input>
            </div>
          </div>
          <hr />
          <div class="row">
            <div class="col text-left">
              Связанные подразделения
              <div class="mt-2 text-left">
                <ul class="subdivision-list">
                  <li v-for="item in subdivisionList" v-bind:key="item.id" class="text-left">
                    <b-form-checkbox
                      v-bind:value="item"
                      v-model="row.item.subdivisionList"
                    >{{item.name}}</b-form-checkbox>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
        <hr />
        <div class="row mt-2 text-right">
          <div class="col">
            <b-button variant="outline-secondary" class="mr-3" @click="getEmployeesList()">Отмена</b-button>
            <b-button variant="outline-primary" @click="editEmployee(row.item)">Сохранить</b-button>
          </div>
        </div>
      </template>

      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetEmployeeList,
  AddEmployee,
  EditEmployee,
  RemoveEmployee
} from "../../service/employeeService";
import { GetSubdivisionList } from "../../service/subdivisionService";

import { GetRoleList } from "../../service/roleService";

export default {
  name: "EmployeeList",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "firstName", label: "Фамилия", sortable: true },
        { key: "name", label: "Имя", sortable: true },
        { key: "middleName", label: "Отчество", sortable: true },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      isLoading: true,
      showAddBlock: false,
      employeeList: [],
      subdivisionList: [],
      employee: {
        id: null,
        roles: [],
        firstName:"",
        name:"",
        middleName:"",
        account: {
          login: "",
          password: ""
        },
        subdivisionList: [],
        isValid: false
      },
      changePassword: false,
      roleList: []
    };
  },
  computed: {},
  methods: {
    getEmployeesList() {
      GetEmployeeList()
        .then(response => {
          this.employeeList = response.data;
          this.newPassword = "";
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    getRoleList() {
      GetRoleList()
        .then(response => {
          this.roleList = response.data;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    getSubdivisionList() {
      GetSubdivisionList()
        .then(response => {
          this.subdivisionList = response.data;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    addEmployee() {
      let firstName = this.employee.firstName.replace(/\s/g, "");
      let name = this.employee.name.replace(/\s/g, "");
      let login = this.employee.account.login.replace(/\s/g, "");
      let password = this.employee.account.password.replace(/\s/g, "");

      if (firstName === "" || name === "" || login === "" || password === "") {
        this.$ntf.Warn(
          "Необходимо ввести ФИО, Логин и Пароль нового сотрудника."
        );
        return;
      }

      this.isLoading = true;
      AddEmployee(this.employee)
        .then(() => {
          this.$ntf.Success("Сотрудник добавлен!");
          this.getEmployeesList();
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении сотрудника.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    editEmployee(employee) {

      let firstName = employee.firstName.replace(/\s/g, "");
      let name = employee.name.replace(/\s/g, "");
      let login = employee.account.login.replace(/\s/g, "");
      let password = employee.account.password.replace(/\s/g, "");

      if (firstName === "" || name === "" || login === "" || password === "") {
        this.$ntf.Warn(
          "Необходимо ввести ФИО, Логин и Пароль нового сотрудника."
        );
        return;
      }

      this.isLoading = true;
      EditEmployee(employee)
        .then(() => {
          this.$ntf.Success("Пользователь сохранен!");
          this.getEmployeesList();
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении пользователя.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    removeEmployee(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить этого сотрудника?", {
          title: "Удаление сотрудника",
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
            RemoveEmployee(id)
              .then(() => {
                this.$ntf.Success("Сотрудник удален!");
                this.getEmployeesList();
              })
              .catch(error => {
                this.$ntf.Error("Неудалось удалить сотрудника.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    }
  },
  created() {
    this.getRoleList();
    this.getSubdivisionList();
    this.getEmployeesList();
  },
  props: {}
};
</script>
<style scoped>
.subdivision-list {
  list-style: none;
  padding: 0 0 0 1rem;
  margin: 0;
}
.subdivision-list li {
  padding: 0 0 0 0.5rem;
}

.employee-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>