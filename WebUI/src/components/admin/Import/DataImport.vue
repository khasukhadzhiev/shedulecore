<template v-if="$Role.EmployeeHasRole('admin')">
  <div>
      <div class="text-left mt-3" v-if="!importProgress.inProcess">
        <label>Выберите версию расписания и файл для импорта</label>
        <div class="row">
          <div class="col-5 text-left">
              <div class="mr-5">Версия расписания</div>
              <multiselect
                v-model="version"
                :options="versionList"
                :allowEmpty="false"
                track-by="id"
                label="name"
                :show-labels="false"
                placeholder="Выбрать версию расписания"
                :multiple="false"
              >
                <template slot="noResult">Версия не найдена!</template>
              </multiselect>              
          </div>
        </div>
        <div class="row mt-3">
          <div class="col-5">
            <b-form-file
              v-model="importingFile"
              :state="Boolean(importingFile)"
              placeholder="Выберите файл для импорта данных"
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
    <template>
      <div class="text-left mt-5 mb-3">
        <label>Структура импортируемого файла приведена ниже. 
          <br/>Каждая строка представляет с собой отдельное занятие группы. 
          <br/>Регистр букв в импортируемом файле не учитывается.
          <br/>Учитывать, что первая строка файла при импорте не используется, импорт идет начиная с второй строки.
        </label>
      </div>
    </template>
    <template>
      <table id="exampleFileStruct" class="small table-bordered">
        <thead>
          <tr>
            <th>Подразделение</th>
            <th>Группа</th>
            <th>Количество обучающихся группы</th>
            <th>ФИО преподавателя (полностью)</th>
            <th>Дисциплина</th>
            <th>Вид занятия <br/><small>(Один из следующих видов: Лекция, Лабораторная, Практическое,Семинарское)</small></th>
            <th>Занятие подгруппы <br/><small>(Ставить "1" если да и "0" если нет)</small></th>
            <th>Занятие по одной неделе	<br/><small>(Ставить "1" если да и "0" если нет)</small></th>
            <th>Параллель <br/><small>(Ставить "1" если да и "0" если нет)</small></th>
            <th>Поток <br/><small>(Ставить "1" если да и "0" если нет)</small></th>
            <th>Группы потока	<br/><small>(Наименование групп через запятую, включая данную группу)</small></th>
            <th>Смена	<br/><small>(Одна из следующих видов: Первая, Вторая, Третья, Четвертая)</small></th>
            <th>Форма обучения <br/><small>(Одна из следующих форм: Очная, Заочная, Очно-заочная)</small></th>
            <th>Отчетность<br/><small>(Один из следующих видов: Экзамен, Зачет, Зачет с оценкой)</small></th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Колледж	</td>
            <td>ПИ-19	</td>
            <td>25</td>
            <td>Плиева Мадина Валерьевна</td>
            <td>Русский язык</td>
            <td>Практическое</td>
            <td>0</td>
            <td>0</td>
            <td>0</td>
            <td>0</td>
            <td></td>
            <td>ПЕРВАЯ</td>
            <td>ОЧНАЯ</td>
            <td>Экзамен</td>
          </tr>
          <tr>
            <td>ФАКУЛЬТЕТ ИНФОРМАЦИОННЫХ ТЕХНОЛОГИЙ</td>
            <td>БИ-19</td>
            <td>28</td>
            <td>Бегуев Сулейман Ахятьевич</td>
            <td>ЧЕЧЕНСКАЯ ТРАДИЦИОННАЯ КУЛЬТУРА И ЭТИКА</td>
            <td>ЛЕКЦИЯ</td>
            <td>0</td>
            <td>1</td>
            <td>0</td>
            <td>1</td>
            <td>ПИ-19-1,ПИ-19-2,ИТИСС-19,БИ-19</td>
            <td>ПЕРВАЯ</td>
            <td>ОЧНАЯ</td>
            <td>Зачет</td>
          </tr>
          <tr>
            <td>ФАКУЛЬТЕТ ИНОСТРАННЫХ ЯЗЫКОВ</td>
            <td>Ин.ЯЗ-19</td>
            <td>28</td>
            <td>Гузуева Элина Руслановна</td>
            <td>ИНФОРМАТИКА</td>
            <td>ЛЕКЦИЯ</td>
            <td>0</td>
            <td>0</td>
            <td>0</td>
            <td>0</td>
            <td></td>
            <td>ПЕРВАЯ</td>
            <td>ОЧНАЯ</td>
            <td>Зачет с оценкой</td>
          </tr>          
        </tbody>
      </table>      
    </template>
  </div>
</template>

<script>
import {
  ImportTimetableData,
  GetImportProgress,
  RemoveImportProgress
} from "../../../service/importDataService";
import { GetVersionList } from "../../../service/versionService";

export default {
  name: "DataImport",
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
      ImportTimetableData(file, this.version.id)
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
    },
    getVersionList() {
      this.isLoading = true;
      GetVersionList()
        .then(response => {
          this.versionList = response.data;
          if (response.data.length > 0) {
            this.version = response.data.find(v => v.isActive);
          }
        })
        .catch(error => {
          this.$ntf.Error("Неудалось получить данные.", error);
        })
        .finally(() => {
          this.isLoading = false;
        });
    }
  },
  created() {
    this.getVersionList();
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
</style>
