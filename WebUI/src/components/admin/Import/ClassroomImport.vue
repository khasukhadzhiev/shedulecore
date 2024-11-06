<template v-if="$Role.EmployeeHasRole('admin')">
  <div>
    <template>
      <div class="text-left mt-3" v-if="!importProgress.inProcess">
        <label>Выберите файл для импорта аудиторий</label>
        <div class="row mt-3">
          <div class="col-5">
            <b-form-file
              v-model="importingFile"
              :state="Boolean(importingFile)"
              placeholder="Выберите файл для импорта аудиторий"
              browse-text="Выбрать"
            ></b-form-file>
          </div>
          <div class="col text-left">
            <b-button variant="outline-primary" @click="importData" :disabled="importingFile === null">Импортировать</b-button>
          </div>
        </div>
        <template v-if="isLoading">
          <hr />
          <div class="text-center my-2">
            <b-spinner variant="primary" label="Text Centered"></b-spinner>
          </div>
        </template>
      </div>
      <template v-else>
        <div class="row">
          <div class="col text-left mb-3 mt-3">
            <b-button
              variant="outline-secondary"
              class="mr-3"
              @click="getImportProgress()"
            >Обновить прогресс</b-button>      
            <b-button
              variant="outline-primary"
              @click="removeImportProgress()"
              v-if="importProgress.importFinished"
            >Импортировать новый файл</b-button>
          </div>
        </div>
        <ul class="import-progress">
          <li>Всего строк: {{importProgress.totalLessonCount}}  из них импортированы: {{importProgress.checkedLessonCount}}</li>
          <li></li>
          <li v-if="importProgress.importError" class="text-danger">Импорт отменен. Возникла ошибка.  {{importProgress.errorMessage}}</li>
          <li v-if="importProgress.importFinished && !importProgress.importError" class="text-success">Импорт прошел успешно, все данные сохранены.</li>
        </ul>
        <template v-if="isImportProgressLoading">
          <div class="text-center my-2">
            <b-spinner variant="primary" label="Text Centered"></b-spinner>
          </div>
        </template>
      </template>
    </template>
    <template>
      <div class="text-left mt-5 mb-3">
        <label>Структура импортируемого файла приведена ниже. 
          <br/>Каждая строка представляет с собой отдельную аудиторию. 
          <br/>Регистр букв в импортируемом файле не учитывается.
          <br/>Учитывать, что первая строка файла при импорте не используется, импорт идет начиная с второй строки.
        </label>
      </div>
    </template>
    <template>
      <table id="exampleFileStruct" class="small table-bordered">
        <thead>
          <tr>
            <th>Корпус</th>
            <th>Тип аудитории</th>
            <th>Наименование</th>
            <th>Количество посадочных мест</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Колледж	</td>
            <td>ПИ-19	</td>
            <td>25</td>
            <td>Плиева Мадина Валерьевна</td>
          </tr>
          <tr>
            <td>ФАКУЛЬТЕТ ИНФОРМАЦИОННЫХ ТЕХНОЛОГИЙ</td>
            <td>БИ-19</td>
            <td>28</td>
            <td>Бегуев Сулейман Ахятьевич</td>
          </tr>
          <tr>
            <td>ФАКУЛЬТЕТ ИНОСТРАННЫХ ЯЗЫКОВ</td>
            <td>Ин.ЯЗ-19</td>
            <td>28</td>
            <td>Гузуева Элина Руслановна</td>
          </tr>          
        </tbody>
      </table>      
    </template>
  </div>
</template>

<script>
import {
  ImportClassroomList,
  GetImportProgress,
  RemoveImportProgress
} from "../../../service/importDataService";

export default {
  name: "ClassroomImport",
  data: function() {
    return {
      importingFile: null,
      isLoading: false,
      importProgress: {
        inProcess: false,
        importFinished: false
      },
      isImportProgressLoading: false,
      version:{},
      versionList: [],
    };
  },
  computed: {
    showProgress() {
      if (
        Object.keys(this.importProgress).length > 0 &&
        !this.importProgress.importFinished
      ) {
        return true;
      } else {
        return false;
      }
    }
  },
  methods: {
    importData() {
      this.isLoading = true;
      let file = new FormData();
      file.append("file", this.importingFile);
      this.importProgress.inProcess = true;
      this.importProgress.importFinished = false;
      ImportClassroomList(file, this.version.id)
        .then(() => {
          this.$ntf.Success("Данные импортированы.");
        })
        .catch(error => {
          this.$ntf.Error("Неудалось импортировать данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
          this.importingFile = null;
          this.getImportProgress();
        });
    },
    getImportProgress() {
      this.isImportProgressLoading = true;
      GetImportProgress()
        .then(response => {
          this.importProgress = Object.create(response.data);   
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить прогресс импорта.", error);
        })
        .finally(() => {
          this.isImportProgressLoading = false;
        });
    },
    removeImportProgress() {
      RemoveImportProgress()
        .then(() => {
          this.getImportProgress();
          this.importProgress.inProcess = false;
          this.importProgress.importFinished = false;
        })
        .catch(error => {
          this.$ntf.Error("Неудалось удалить прогресс импорта.", error);
        });
    }
  },
  created() {
    this.getImportProgress();
  },
  props: {}
};
</script>

<style scoped>
.import-progress {
  padding: 0;
  margin: 0;
  list-style: none;
  font-weight: 600;
}

.import-progress li {
  padding: 0;
  margin: 0;
  text-align: left;
}

.import-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}
</style>
