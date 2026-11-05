
# 🥗 FridgeChef

FridgeChef — вебзастосунок для підбору рецептів за продуктами, які є вдома, керування холодильником, розрахунку калорій і БЖВ та створення списків покупок.

Проєкт розробляється командою з використанням Blazor WebAssembly та ASP.NET Core Web API.

## ✨ Функціонал

### 🥦 Розумний холодильник (Smart Fridge)
- Додавання, перегляд і керування продуктами.
- Організація інгредієнтів у холодильнику.
- Підбір рецептів відповідно до наявних продуктів.
- Фільтрація рецептів за різними критеріями.

### 🍽️ Рецепти (Recipes)
- Перегляд доступних рецептів.
- Інформація про інгредієнти та приготування.
- Пошук страв відповідно до потреб користувача.

### 🔥 Калькулятор калорій та БЖВ
- Розрахунок добової потреби в калоріях.
- Визначення рекомендованої кількості білків, жирів і вуглеводів.
- Урахування персональних параметрів користувача.

### 🛒 Список покупок (Shopping List)
- Додавання та видалення товарів.
- Перегляд списку необхідних продуктів.
- Позначення придбаних товарів.

### 👤 Авторизація та користувачі
- Реєстрація нового користувача.
- Вхід за email і паролем.
- Безпечне хешування паролів на сервері.
- Cookie-based authentication.
- Перевірка поточної сесії.
- Вихід з облікового запису.
- API для керування користувачами.

### ⚙️ Особистий кабінет
- Перегляд профілю користувача.
- Керування персональними даними.

## 📱 Сторінки застосунку

1. **Home** — головна сторінка та презентація можливостей.
2. **Login / Registration** — авторизація та реєстрація.
3. **Fridge** — керування продуктами.
4. **Recipes** — перегляд і підбір рецептів.
5. **Shopping List** — список покупок.
6. **Nutrition Calculator** — розрахунок калорій і БЖВ.
7. **Profile** — особистий кабінет користувача.

## 🛠️ Технології

| Компонент | Технологія |
|-----------|------------|
| Frontend | Blazor WebAssembly (.NET 8) |
| UI | MudBlazor |
| Backend | ASP.NET Core Web API |
| Мова програмування | C# |
| API | REST / Minimal API |
| Авторизація | ASP.NET Core Cookie Authentication |
| Документація API | Swagger / OpenAPI |
| Тестування | bUnit, .NET Tests |
| Контроль версій | Git, GitHub |

### Зберігання даних

На поточному етапі сервер використовує **in-memory storage**.

Дані зберігаються в пам'яті процесу та очищаються після перезапуску API.

PostgreSQL та Entity Framework Core можуть бути додані на наступних етапах розробки.

## 📂 Структура проєкту

```text
fridge-chef/
├── FridgeChef/              # Blazor WebAssembly frontend
│   ├── Pages/
│   ├── Components/
│   ├── Services/
│   ├── Models/
│   └── wwwroot/
│
├── FridgeChef.Api/          # ASP.NET Core Web API
│   ├── Features/
│   │   ├── Auth/
│   │   ├── Users/
│   │   ├── FridgeItems/
│   │   ├── ShoppingItems/
│   │   ├── Courses/
│   │   └── Profiles/
│   ├── Middleware/
│   └── Program.cs
│
├── FridgeChef.Tests/        # Frontend tests
├── FridgeChef.Api.Tests/    # Backend tests (if present)
└── README.md
```

## 🚀 Локальний запуск

### 1. Вимоги

- .NET SDK 8.0
- Git
- Visual Studio або Visual Studio Code

### 2. Клонування репозиторію

```bash
git clone https://github.com/soloshkamala/fridge-chef.git
cd fridge-chef
```

### 3. Відновлення залежностей

```bash
dotnet restore
```

### 4. Запуск Backend API

У першому терміналі:

```bash
dotnet run --project FridgeChef.Api
```

Адреса API для локальної конфігурації:

http://localhost:5039

Swagger UI:

http://localhost:5039/swagger

### 5. Запуск Frontend

У другому терміналі:

```bash
dotnet run --project FridgeChef
```

Адреса клієнта:

http://localhost:5050

Обидва застосунки мають працювати одночасно.

## 🔐 Authentication API

| Метод | Endpoint | Опис |
|-------|----------|------|
| POST | `/api/users` | Реєстрація користувача |
| POST | `/api/auth/login` | Вхід в обліковий запис |
| GET | `/api/auth/session` | Перевірка сесії |
| POST | `/api/auth/logout` | Вихід із системи |

### Приклад реєстрації

```json
{
  "email": "test@example.com",
  "password": "password123",
  "displayName": "Test User"
}
```

### Приклад входу

```json
{
  "email": "test@example.com",
  "password": "password123"
}
```

Після успішного входу сервер встановлює authentication cookie.

Для браузерних запитів, які використовують cookie-авторизацію, потрібно передавати credentials.

## 🌐 API та CORS

Frontend і Backend запускаються на різних портах.

Для локальної розробки:

- Frontend: `http://localhost:5050`
- Backend: `http://localhost:5039`

Сервер налаштований на CORS із підтримкою credentials для дозволених адрес клієнта.

У Development доступна документація Swagger/OpenAPI.

## 🧪 Тестування

Для запуску тестів:

```bash
dotnet test
```

Проєкт містить автоматизовані тести клієнтських компонентів та серверної логіки.

## 🌍 Локалізація

Застосунок підтримує дві мови:

- 🇺🇦 Українська
- 🇬🇧 Англійська

## 📌 Подальший розвиток

- Інтеграція PostgreSQL.
- Використання Entity Framework Core.
- Розширення можливостей підбору рецептів.
- Персоналізоване планування харчування.
- Покращення профілю користувача.
- Розширення автоматизованих тестів.

## 👥 Командна розробка

Проєкт розробляється командою з використанням GitHub.

Нові можливості реалізуються в окремих feature-гілках і додаються до `main` через Pull Request.
