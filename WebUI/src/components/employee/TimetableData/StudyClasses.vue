<template>
  <div v-if="$Role.EmployeeHasRole('employee')">
    <template v-if="showAddBlock">
      <div class="row mt-3 text-left">
        <div class="col">
          <label>Наименование группы</label>
          <b-form-input
            v-model="studyClass.name"
            type="text"
            @input="studyClass.name = studyClass.name.toUpperCase()"
          ></b-form-input>
        </div>
        <div class="col">
          <label>Количество учащихся</label>
          <b-form-input v-model.number="studyClass.studentsCount" type="number" min="1"></b-form-input>
        </div>
        <div class="col" v-if="version.showClassShift">
          <label>Смена обучения</label>
          <b-form-select
            v-model="studyClass.classShiftId"
            :options="classShiftList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
        <div class="col" v-if="version.showEducationForm">
          <label>Форма обучения</label>
          <b-form-select
            v-model="studyClass.educationFormId"
            :options="educationFormList"
            value-field="id"
            text-field="name"
            disabled-field="notEnabled"
          ></b-form-select>
        </div>
      </div>
    </template>
    <hr v-if="showAddBlock" />
    <div class="row mt-3 mb-3 text-right">
      <div class="col">
        <a
          href="#"
          @click="showAddBlock = !showAddBlock"
          class="pr-2"
        >{{showAddBlock ? "Скрыть панель добавления" : "Показать панель добавления"}}</a>
        <b-button
          variant="outline-primary"
          @click="addStudyClass"
          v-if="showAddBlock"
        >Добавить группу</b-button>
      </div>
    </div>

    <div class="row text-left mb-3" v-if="subdivisionList.length > 1">
      <div class="col-4">
          <multiselect
            v-model="studyClass.subdivision"
            :options="subdivisionList"
            :allowEmpty="false"
            track-by="id"
            label="name"
            :show-labels="false"
            placeholder="Выбрать подразделение"
            :multiple="false"
          >
            <template slot="noResult">Подразделение не найдено!</template>
          </multiselect>        
      </div>
    </div>

    <b-table
      hover
      outlined
      head-variant="light"
      :items="studyClassList"
      :fields="computedFields"
      :busy="isLoading"
      class="text-left"
    >
      <template v-slot:cell(index)="data">{{data.index + 1}}</template>
      <template v-slot:cell(classShift)="data">{{data.item.classShift.name}}</template>
      <template v-slot:cell(educationForm)="data">{{data.item.educationForm.name}}</template>
      <template
        v-slot:cell(subdivision)="data"
        v-if="subdivisionList.length > 1"
      >{{data.item.subdivision.name}}</template>
      <template v-slot:table-busy>
        <div class="text-center my-2">
          <b-spinner variant="primary" label="Text Centered"></b-spinner>
        </div>
      </template>

      <template v-slot:cell(remove)="data" style="width:50px">
        <span class="btn" @click="removeStudyClass(data.item.id)">
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
            <b-form-input v-model.number="row.item.studentsCount" type="number" min="1"></b-form-input>
          </div>
          <div class="col" v-if="version.showClassShift">
            <b-form-select
              v-model="row.item.classShiftId"
              :options="classShiftList"
              class="mb-3"
              value-field="id"
              text-field="name"
              disabled-field="notEnabled"
            ></b-form-select>
          </div>
          <div class="col" v-if="version.showEducationForm">
            <b-form-select
              v-model="row.item.educationFormId"
              :options="educationFormList"
              class="mb-3"
              value-field="id"
              text-field="name"
              disabled-field="notEnabled"
            ></b-form-select>
          </div>
        </div>
        <div class="row" v-if="subdivisionList.length > 1">
          <div class="col-4">
            <label class="ml-3">Подразделение</label>
            <multiselect
              v-model="row.item.subdivision"
              :options="subdivisionList"
              :allowEmpty="false"
              track-by="id"
              label="name"
              :show-labels="false"
              placeholder="Выбрать подразделение"
              :multiple="false"
            >
              <template slot="noResult">Подразделение не найдено!</template>
            </multiselect>                 
          </div>
        </div>
        <div class="row mt-2 text-right">
          <div class="col">
            <b-button
              variant="outline-secondary"
              class="mr-3"
              @click="getStudyClassListBySubdivision()"
            >Отмена</b-button>
            <b-button variant="outline-primary" @click="editStudyClass(row)">Сохранить</b-button>
          </div>
        </div>
      </template>
    </b-table>
  </div>
</template>

<script>
import {
  GetStudyClassListBySubdivision,
  AddStudyClass,
  EditStudyClass,
  RemoveStudyClass,
  GetClassShiftList,
  GetEducationFormList
} from "../../../service/timetableDataServices/studyClassService";
import { GetCurrentEmployeeSubdivisionList } from "../../../service/subdivisionService";

export default {
  name: "StudyClasses",
  data: function() {
    return {
      fields: [
        { key: "index", label: "№" },
        { key: "name", label: "Наименование группы" },
        { key: "studentsCount", label: "Количество учащихся" },
        { key: "subdivision", label: "Подразделение", },
        { key: "classShift", label: "Смена обучения" },
        { key: "educationForm", label: "Форма обучения" },
        { key: "show_details", label: "" },
        { key: "remove", label: "" }
      ],
      studyClass: {
        name: "",
        studentsCount: 1,
        educationFormId: 1,
        classShiftId: 1,
        subdivision: {}
      },
      subdivisionList: [],
      isLoading: false,
      studyClassList: [],
      showAddBlock: false,
      educationFormList: [],
      classShiftList: []
    };
  },
  computed: {
    computedFields() {
      let result = this.fields;

      if (this.subdivisionList.length <= 1) {
        result = result.filter(field => field.key !== "subdivision");
      }

      if (!this.version.showClassShift) {
        result = result.filter(field => field.key !== "classShift");
      }
      
      if (!this.version.showEducationForm) {
        result = result.filter(field => field.key !== "educationForm");
      }      

      return result;
    }
  },
  watch:{
    'studyClass.subdivision': function() {
      this.getStudyClassListBySubdivision();
    },
  },
  methods: {
    getStudyClassListBySubdivision() {
      this.isLoading = true;
      GetStudyClassListBySubdivision(this.studyClass.subdivision.id)
        .then(response => {
          this.studyClassList = [];
          response.data.forEach(elements => {
            if(elements.length > 0){
              elements.forEach(item => {
                let exist = this.studyClassList.find(s => s.id === item.id);
                if(exist == null){
                  this.studyClassList.push(item);
                }
              });
            }
          });
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    getCurrentEmployeeSubdivisionList() {
      GetCurrentEmployeeSubdivisionList()
        .then(response => {
          this.subdivisionList = response.data;
          this.studyClass.subdivision = response.data[0];
          this.getStudyClassListBySubdivision();
        })
        .finally(() => {
          this.isLoading = false;
        });
    },

    getClassShiftList() {
      GetClassShiftList()
        .then(response => {
          this.classShiftList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        });
    },

    getEducationFormList() {
      GetEducationFormList()
        .then(response => {
          this.educationFormList = response.data;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        });
    },
    addStudyClass() {
      let isNameEmpty = this.studyClass.name.replace(/\s/g, "");
      let isStudentCountEmpty = this.studyClass.studentsCount < 1;
      
      if (isNameEmpty === "" || isStudentCountEmpty) {
        this.$ntf.Warn("Наименование и количество студентов должно быть заполнено.");
        return;
      }      
      this.isLoading = true;
      AddStudyClass(this.studyClass)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            this.$ntf.Success("Группа сохранена!");
            this.getStudyClassListBySubdivision();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении группы.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    removeStudyClass(id) {
      this.$bvModal
        .msgBoxConfirm("Вы уверены, что хотите удалить группу?", {
          title: "Удаление группы",
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
            RemoveStudyClass(id)
              .then(() => {
                this.$ntf.Success("Группа удалена!");
                this.getStudyClassListBySubdivision();
              })
              .catch(error => {
                this.$ntf.Error("Ошибка при удалении группы.", error);
              })
              .finally(() => {
                this.isLoading = false;
              });
          }
        });
    },
    editStudyClass(row) {

      let isNameEmpty = row.item.name.replace(/\s/g, "");
      let isStudentCountEmpty = row.item.studentsCount < 1;
      
      if (isNameEmpty === "" || isStudentCountEmpty) {
        this.$ntf.Warn("Наименование и количество студентов должно быть заполнено.");
        return;
      }    

      this.isLoading = true;
      EditStudyClass(row.item)
        .then(response => {
          if (response.data) {
            this.$ntf.Warn(response.data);
          } else {
            row.item.educationForm = this.educationFormList.find(
              e => e.id == row.item.educationFormId
            );
            row.item.classShift = this.classShiftList.find(
              c => c.id == row.item.classShiftId
            );
            this.$ntf.Success("Группа сохранена!");
            this.getStudyClassListBySubdivision();
          }
        })
        .catch(error => {
          this.$ntf.Error("Ошибка при сохранении группы.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  },
  created() {
    this.getClassShiftList();
    this.getCurrentEmployeeSubdivisionList();
    this.getEducationFormList();
  },
  props: {
    version: Object
  }
};
</script>