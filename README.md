## Общее описание логики работы решения

Система позволяет формировать расписания как в ручном так и автоматическом режиме.

Ручное формирование основано на Drag&Drop, перетаскивая занятия на сетку расписания они сохранют указанную позицию. Так формируется расписания для каждого класса.

<img src="manual-shaping-process.gif"  width="500" height="500">

Автоматическое формирование основано на генетических алгоритмах. 
1. При запуске автоматического формирования, система создает 5 разных расписаний со случайным расположением занятий на сетке расписания.
2. Далее каждое расписание проходит проверку соотвествия всем требованиям, если сформировано оптимальное расписание все данные сохраняются и проверка завершается
3. Если оптимальное расписание не обнаружено, то происходит скрещивание всех 5-ти вариантов расписания и из них формируется 10 новых вариантов из которых в свою очередь выберается 5 лучших.
4. Процесс снова начинается с пункта 2 этого списка.

<img src="gen-algo-process.png"  width="500" height="500">


## Требования к окружению для запуска продукта

Backend: кроссплатформенное решение реализованное на .Net Core 3.1, для развертывания можно использовать традиционный IIS, либо кросс-платформенный веб-сервер Kestrel. Я использую IIS Express из VS Community. 

Frontend: реализовано на vuejs 3. Необходим Node.js.

DB: используется MS Sql Server 2016 Developer. Можно использовать Postgres Sql, но тестирование с этой СУБД еще не проводилось, лучше придерживаться MS Sql Server.


## Сценарий сборки и запуска проекта

Backend:

Solution состоит из 5 проектов.
1. BL (Бизнес логика. Здесь собраны сервисы, в которых и реализована вся бизнес-логика).
2. CoreAPI (Главный проект backenda. Здесь реализованы контроллеры, через которые и осуществляется взаимодействие с frontend. Данный проект является запускаемым(стартовым, запуск этого проекта запускает backend). Если по какой-либо причине это не так, нужно через контекстное меню проекта выбрать его запускаемым).
3. DAL (Здесь собраны модели БД, контект и миграции).
4. DTL (Здесь реализована логика копирования(Mapping) сущностей БД в объекты для передачи на frontend и обратно).
5. Infrastructure (Здесь собраны логика для учета прогресса формирования расписания и прочее).

Запуск backenda

1. Запустить проект CoreAPI в Visual Studio.
2. При первом запуске проект сам создаст БД с необходимыми данными для работы. Строка подключения к серверу БД лежит тут(CoreAPI/appsettings.json).
3. После сборки и запуска проекта (если проект настроен на запуск браузера), то откроеся браузер с `кодом 404`. Это нормально.
4. Все. Backend поднят и работает. Пора приступать с frontend.

Frontend:

1. Установить Node.js (https://nodejs.org/en/blog/release/v14.17.3/)
2. Запустить `npm intstall` из папки WebUI для установки Vuejs и всех компонентов.
3. После установки собрать проект и запустить `npm run serve`
4. После сборки и запуска перейту по адресу `http://localhost:8080/`

Все готово. Теперь система работает по адресу `http://localhost:8080/`. Приятного использования.

Данные авторизации:
`login: admin`
`password: Admin`



## Используемые наборы данных
Данные для импорта, по которым можно формировать расписание лежат в тут же. Файл [Timetable-Data.xlsx](Timetable-Data.xlsx).


## Добавление миграций
dotnet ef migrations add ‘InitializeDB’ -s CoreAPI/CoreAPI.csproj -p DAL/DAL.csproj

Запускать команду нужно из папки самого решения.
-s это стартап проект
-p это проект где находится логика данных

