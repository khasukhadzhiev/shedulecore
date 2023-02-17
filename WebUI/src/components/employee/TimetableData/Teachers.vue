<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <div class="row mt-3 text-left" v-if="showAddBlock">
      <div class="col">
        <label>Фамилия</label>
        <b-form-input v-model.trim="teacher.firstName" @input="teacher.firstName = teacher.firstName.toUpperCase()"></b-form-input>
      </div>
      <div class="col">
        <label>Имя</label>
        <b-form-input v-model.trim="teacher.name" @input="teacher.name = teacher.name.toUpperCase()"></b-form-input>
      </div>
      <div class="col">
        <label>Отчество</label>
        <b-form-input v-model.trim="teacher.middleName" @input="teacher.middleName = teacher.middleName.toUpperCase()"></b-form-input>
      </div>
    </div>
    <hr v-if="showAddBlock" />
    <div class="row mb-3 mt-3 text-right">
      <div class="col">
        <a href="#" @click="showAddBlock = !showAddBlock" class="pr-2">
          {{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}} 
        </a>        
        <b-button variant="outline-primary" @click="addTeacher" v-if="showAddBlock">Добавить преподавателя</b-button>
      </div>
    </div>

    <b-table 
    hover
    outlined
    head-variant="light"
    :items="teacherList" 
    :fields="fields" 
    :busy="isLoading" 
    class="text-left">
      <template v-slot:cell(index)="data">{{data.index + 1}}</template>
      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>

      <template v-slot:cell(remove)="data" style="width:50px">
        <span class="btn" @click="removeTeacher(data.item.id)">
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
            <b-form-input v-model="row.item.firstName" @input="row.item.firstName = row.item.firstName.toUpperCase()"></b-form-input>
          </div>
          <div class="col">
            <b-form-input v-model="row.item.name" @input="row.item.name = row.item.name.toUpperCase()"></b-form-input>
          </div>
          <div class="col">
            <b-form-input v-model="row.item.middleName" @input="row.item.middleName = row.item.middleName.toUpperCase()"></b-form-input>
          </div>
        </div>
        <hr>
        <div class="row mt-3">
          <div class="col-2 ml-2 text-left">
            Приоритетные дни
          </div>
          <div class="col">
            <form class="form-inline">
              <b-form-checkbox class="pr-4" :value="parseInt(0)" v-model="row.item.weekDays">Понедельник</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(1)" v-model="row.item.weekDays">Вторник</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(2)" v-model="row.item.weekDays">Среда</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(3)" v-model="row.item.weekDays">Четверг</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(4)" v-model="row.item.weekDays">Пятница</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(5)" v-model="row.item.weekDays">Суббота</b-form-checkbox>
              <b-form-checkbox class="pr-4" :value="parseInt(6)" v-model="row.item.weekDays">Воскресенье</b-form-checkbox>              
            </form>         
          </div>
        </div>
        <hr>
        <div class="row mt-3">
          <div class="col-2 ml-2  text-left">
            Приоритетный порядок занятий
          </div>
          <div class="col">
            <form class="form-inline">
              <b-form-checkbox class="pr-4" v-for="item in version.maxLesson" v-bind:key="item" v-bind:value="parseInt(item.toString())" v-model="row.item.lessonNumbers">{{item}}</b-form-checkbox>    
            </form>
          </div>
        </div>        

        <div class="row mt-2 text-right">
          <div class="col">
            <b-button variant="outline-secondary" class="mr-3" @click="getTeacherList()">Отмена</b-button>
            <b-button variant="outline-primary"  @click="editTeacher(row)">Сохранить</b-button>
          </div>
        </div>          

      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetTeacherList,
  AddTeacher,
  RemoveTeacher,
  EditTeacher
} from "../../../service/timetableDataServices/teacherService";

export default {
  name: "Teachers",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "firstName", label: "Фамилия" },
        { key: "name", label: "Имя" },
        { key: "middleName", label: "Отчество" },
        { key: "show_details", label: ""},
        { key: "remove", label: ""},        
      ],
      isLoading: false,
      teacherList: [],
      teacher: {
        firstName: "",
        name: "",
        middleName: "",
      },
      showAddBlock: false,
    };
  },
  methods: {
    getTeacherList() {
      this.isLoading = true;
      GetTeacherList()
        .then(response => {
          this.teacherList = response.data;
        })
        .catch(error => {
          this.$ntf.Error('Неудалось получить данные', error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    addTeacher() {
      let isEmptyfirstName = this.teacher.firstName.replace(/\s/g, "");
      let isEmptyname = this.teacher.name.replace(/\s/g, "");
      let isEmptymiddleName = this.teacher.middleName.replace(/\s/g, "");
      
      if (isEmptyfirstName === "" || isEmptyname === "" || isEmptymiddleName === "") {
        this.$ntf.Warn("ФИО преподавателя должно быть заполнено.");
        return;
      }          

      this.isLoading = true;
      AddTeacher(this.teacher)
        .then(response => {
          if(response.data){
            this.$ntf.Warn(response.data);
          }
          else{
            this.$ntf.Success('Преподаватель сохранен!');
            this.getTeacherList();
          }          
        })
        .catch(error => {
          this.$ntf.Error('Ошибка при сохранении перподавателя.', error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeTeacher(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить преподавателя?", {
          title: "Удаление преподавателя",
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
            RemoveTeacher(id)
              .then(() => {
                this.$ntf.Success('Преподаватель удален!');
                this.getTeacherList();
              })
              .catch(error => {
                this.$ntf.Error('Ошибка при удалении перподавателя.', error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editTeacher(row){
      let isEmptyfirstName = row.item.firstName.replace(/\s/g, "");
      let isEmptyname = row.item.name.replace(/\s/g, "");
      let isEmptymiddleName = row.item.middleName.replace(/\s/g, "");
      
      if (isEmptyfirstName === "" || isEmptyname === "" || isEmptymiddleName === "") {
        this.$ntf.Warn("ФИО преподавателя должно быть заполнено.");
        return;
      }          

      this.isLoading = true;
      EditTeacher(row.item)
        .then(response => {
          if(response.data){
            this.$ntf.Warn(response.data);
          }
          else{
            this.$ntf.Success('Преподаватель сохранен!');
            this.getTeacherList();
          }                    
        })
        .catch(error => {
          this.$ntf.Error('Ошибка при сохранении перподавателя.', error);
        })
        .finally(() => {
          this.isLoading = false;
        });              
    }
  },
  created() {
    this.getTeacherList();
  },
   props: {
    version:Object
  } 
};
</script>
