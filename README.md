🥗 FridgeChef
FridgeChef — вебзастосунок для підбору рецептів за продуктами, які є вдома, керування холодильником, розрахунку калорій і БЖВ та створення списків покупок.

Проєкт розробляється командою на Blazor WebAssembly та ASP.NET Core Web API.
Стан розробки: частина функцій перебуває в розробці. Інтеграція Microsoft Entra ID та збереження користувачів у PostgreSQL реалізовані в гілці feature/entra-auth; до злиття цієї гілки з main вони можуть бути відсутні в основній версії.

✨ Функціонал
🥦 Розумний холодильник (Smart Fridge)
- Додавання, перегляд і керування продуктами.
- Організація інгредієнтів у холодильнику.
- Підбір рецептів відповідно до наявних продуктів.
- Фільтрація рецептів за різними критеріями.
🍽️ Рецепти (Recipes)
- Перегляд доступних рецептів.
- Інформація про інгредієнти та приготування.
- Пошук страв відповідно до потреб користувача.
🔥 Калькулятор калорій та БЖВ
- Розрахунок добової потреби в калоріях.
- Визначення рекомендованої кількості білків, жирів і вуглеводів.
- Урахування персональних параметрів користувача.
🛒 Список покупок (Shopping List)
- Додавання та видалення товарів.
- Перегляд списку необхідних продуктів.
- Позначення придбаних товарів.
👤 Авторизація та користувачі
- Вхід через Microsoft Entra ID із використанням MSAL у Blazor WebAssembly.
- Передавання JWT Access Token до Backend API.
- Перевірка JWT і політик доступу на сервері.
- Створення запису користувача в PostgreSQL під час першого успішного звернення до GET /api/profiles/me.
- Зв'язування користувача з Microsoft-ідентичністю за EntraObjectId і EntraTenantId.
- Отримання внутрішнього ідентифікатора користувача через CurrentUserService.
- У кодовій базі також збережено попередній механізм входу за email і паролем із cookie-сесією (legacy).
⚙️ Особистий кабінет
- Перегляд профілю користувача.
- Керування персональними даними.
📱 Сторінки застосунку
Home, Login / Registration, Fridge, Recipes, Shopping List, Nutrition Calculator, Profile.
🛠️ Технології
Компонент	Технологія
Frontend	Blazor WebAssembly (.NET 8)
UI	MudBlazor
Backend	ASP.NET Core Web API (.NET 8)
Мова програмування	C#
API	REST / Minimal API
База даних	PostgreSQL
ORM	Entity Framework Core (EF Core)
Microsoft-автентифікація	Microsoft Entra ID, MSAL, Microsoft Identity Web
Авторизація API	JWT Bearer, scopes, roles
Попередній механізм входу	ASP.NET Core Cookie Authentication
Документація API	Swagger / OpenAPI
Тестування	bUnit, .NET Tests
Контроль версій	Git, GitHub


🗄️ Зберігання даних
Backend використовує PostgreSQL і Entity Framework Core для роботи з реляційними даними.
- FridgeChefDbContext забезпечує доступ до таблиць через EF Core.
- EF Core Migrations керують змінами схеми бази даних.
- Міграція AddEntraIdentityToUsers додає до таблиці Users поля EntraObjectId і EntraTenantId.
- Унікальний індекс на пару (EntraTenantId, EntraObjectId) запобігає дублюванню однієї Microsoft-ідентичності.
- Поля допускають NULL для сумісності з наявними записами.
Частина старої логіки авторизації ще використовує in-memory storage. Не всі функції застосунку вже переведено на новий механізм ідентифікації користувача.

📂 Структура проєкту
fridge-chef/
├── FridgeChef/                     # Blazor WebAssembly frontend
│   ├── Pages/
│   ├── Components/
│   ├── Services/
│   ├── Models/
│   └── wwwroot/
├── FridgeChef.Api/                 # ASP.NET Core Web API
│   ├── Data/
│   │   ├── Entities/
│   │   └── FridgeChefDbContext.cs
│   ├── Features/
│   │   ├── Auth/
│   │   ├── Users/
│   │   ├── FridgeItems/
│   │   ├── ShoppingItems/
│   │   ├── Courses/
│   │   └── Profiles/
│   │       ├── GetMyProfile.cs
│   │       └── ProfileEndpoints.cs
│   ├── Migrations/
│   ├── Middleware/
│   ├── Services/
│   │   └── CurrentUserService.cs
│   └── Program.cs
├── FridgeChef.Tests/               # Frontend tests
└── README.md
🚀 Локальний запуск
1. Вимоги
- .NET SDK 8.0
- Git
- Visual Studio або Visual Studio Code
- Запущений PostgreSQL (наприклад, через Docker Desktop)
- Доступ до налаштованих реєстрацій застосунків у Microsoft Entra ID для тестування Microsoft-входу
2. Клонування репозиторію
git clone https://github.com/soloshkamala/fridge-chef.git
cd fridge-chef
Щоб перевірити зміни Microsoft-авторизації до їхнього злиття з main:
git switch feature/entra-auth
3. Відновлення залежностей
dotnet restore
4. Налаштування PostgreSQL
Запустіть локальний сервер PostgreSQL і налаштуйте ConnectionStrings:DefaultConnection для FridgeChef.Api через .NET User Secrets або змінні середовища. Не додавайте паролі до Git.

Після налаштування підключення застосуйте міграції:
dotnet ef database update --project FridgeChef.Api --startup-project FridgeChef.Api
Для команди dotnet ef потрібен встановлений інструмент EF Core CLI.
5. Налаштування Microsoft Entra ID
Для Microsoft-входу потрібні налаштовані реєстрації SPA і Backend API в Microsoft Entra ID, відповідні ClientId, TenantId, redirect URI та делегований scope access_as_user.

Налаштування frontend і backend мають відповідати одне одному. Не публікуйте токени, client secrets або паролі. ClientId і TenantId самі по собі не є секретними ключами.
6. Запуск Backend API
У першому терміналі:
dotnet run --project FridgeChef.Api --urls http://localhost:5039
Backend: http://localhost:5039
Swagger UI (за наявності у Development): http://localhost:5039/swagger
7. Запуск Frontend
У другому терміналі:
dotnet run --project FridgeChef --urls http://localhost:5050
Frontend: http://localhost:5050

Обидва застосунки повинні працювати одночасно.
🔐 Microsoft Entra ID Authentication
Схема роботи
1. Користувач входить через Microsoft Entra ID.
2. MSAL на frontend отримує Access Token для Backend API.
3. Frontend надсилає токен у заголовку Authorization: Bearer <token>.
4. Backend перевіряє токен та застосовану політику авторизації.
5. GET /api/profiles/me шукає користувача за Microsoft Object ID і Tenant ID або створює його запис у PostgreSQL.
6. CurrentUserService знаходить внутрішній UserId для вже збереженого користувача.
Endpoint поточного користувача
Метод	Endpoint	Політика	Опис
GET	/api/profiles/me	ApiRead	Повертає поточного користувача або створює його запис під час першого звернення


Приклад відповіді:
{
  "userId": 1,
  "name": "Example User",
  "email": "user@example.com"
}
Політики авторизації
Політика	Умови
ApiRead	Автентифікований користувач зі scope access_as_user
ApiWrite	Scope access_as_user і роль User або Admin
ApiAdmin	Scope access_as_user і роль Admin


Політики діють лише на endpoints, де вони застосовані. Наявність політики в Program.cs сама по собі не захищає всі маршрути.
Поточне обмеження: endpoints холодильника та частина інших маршрутів усе ще використовують старий підхід до визначення числового UserId. Їх потрібно окремо інтегрувати з CurrentUserService і перевірити захист авторизацією.

🔑 Legacy Authentication API
У проєкті збережені попередні маршрути для входу за email і паролем та cookie-сесії:
Метод	Endpoint	Опис
POST	/api/users	Реєстрація користувача
POST	/api/auth/login	Вхід в обліковий запис
GET	/api/auth/session	Перевірка сесії
POST	/api/auth/logout	Вихід із системи


Приклад реєстрації:
{
  "email": "test@example.com",
  "password": "password123",
  "displayName": "Test User"
}
Приклад входу:
{
  "email": "test@example.com",
  "password": "password123"
}
Ці маршрути належать до попереднього механізму авторизації й не слід плутати їх із Microsoft Entra ID та JWT.
🌐 API та CORS
Frontend і Backend запускаються на різних портах:
- Frontend: http://localhost:5050
- Backend: http://localhost:5039
Backend використовує CORS для дозволених адрес клієнта. Для запитів зі старою cookie-авторизацією потрібне передавання credentials; для Microsoft-авторизації API використовує Bearer-токен.
🧪 Тестування
Для запуску автоматизованих тестів:
dotnet test
Під час локальної перевірки гілки feature/entra-auth успішно виконано збірку Backend API без помилок і авторизований запит GET /api/profiles/me, який повернув внутрішній ID користувача з PostgreSQL.

Тимчасова сторінка AuthTest.razor використовувалася для ручної перевірки, але не включена до коміту 4e3343f.
🌍 Локалізація
Застосунок підтримує українську 🇺🇦 та англійську 🇬🇧 мови.
📌 Подальший розвиток
- Інтеграція CurrentUserService з усіма endpoints, які працюють із персональними даними.
- Захист маршрутів холодильника та інших ресурсів відповідними політиками JWT-авторизації.
- Узгодження нового Microsoft-входу з попередньою cookie-авторизацією.
- Розширення підбору рецептів і персоналізованого планування харчування.
- Покращення профілю користувача.
- Автоматизовані тести автентифікації, авторизації та роботи з базою даних.
👥 Командна розробка
Проєкт розробляється командою з використанням GitHub. Нові можливості реалізуються в окремих feature/* гілках і додаються до main через Pull Request після перевірки.
