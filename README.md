# VKTestApp
Test task for VK
# Some moments about realisation
<br>
POST запрос пример JSON {
  "login": "string",
  "password": "string",
  "userGroupId": 2
}
<br>
userGropuId = 2 - User
<br>
userGropuId = 1 - Admin
<br>
Пагинация сделана с помощью query параметров запроса pageNumber и pageSIze (если pageSize = 0, то вернутся все пользователи)
