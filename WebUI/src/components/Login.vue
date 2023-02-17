<template>
<div class="row login-content">
  <b-card class="card-center mt-5">
    <div class="title">
      Вход в систему
    </div>
    <div>
      <b-form-input v-model="data.login" type="text" class="mb-3" placeholder="Логин"></b-form-input>
      <b-form-input v-model="data.password" type="password" class="mb-3" placeholder="Пароль"></b-form-input>
    </div>

    <b-button variant="outline-primary" :disabled="isLoading" @click="SingIn()">
      <b-spinner v-if="isLoading" small></b-spinner>
      Войти
    </b-button>
  </b-card>
</div>

</template>

<script>
import { Login } from '../service/accountService';

export default {
  name: 'Login',
  components:{
  },
  
  data: function () {
    return {
      data:{
        login: "",
        password: "",
      },
      jwtObj:{},
      isLoading:false,
    }
  },

  methods:{
    SingIn(){
      this.isLoading = true;
      Login(this.data).then(result =>{
        this.$store.dispatch('SET_EMPLOYEE_DATA', result.data);
        this.SetLocalStorage(result.data);
        this.$router.push({ name: "user"});
      }).catch(error =>{
        this.$ntf.Error("Неудалось авторизоваться.", error);
      })
      .finally(() => {
        this.isLoading = false;
      });
    },
    SetLocalStorage(employeeData){
      localStorage.setItem('employee-fullName', employeeData.fullName);
      localStorage.setItem('employee-token', employeeData.token);
      localStorage.setItem('employee-employeeRoles', employeeData.employeeRoles);
      localStorage.setItem('employee-isAutentificated', employeeData.isAutentificated);
      localStorage.setItem('employee-expiration', employeeData.expiration);      
    }
  },
  props: {},
}
</script>

<style scoped>
.login-content {
  padding: 1rem;
  box-shadow: 0 0.15rem 1.75rem 0 rgba(31, 45, 65, 0.15);
  margin: 1rem;
  background-color: white;
  position: relative;
}

.card-center {
  margin: 0 auto; /* Added */
  float: none; /* Added */
  margin-bottom: 10px; /* Added */
  padding-top: 30px;
  width: 400px;
}
.title{
  padding: 10px;
  margin-top:-5em;
  margin-bottom: 20px;
  background-color: white;
  border: 1px solid rgba(0, 0, 0, 0.125);
  border-radius: 0.25rem;
}
</style>
