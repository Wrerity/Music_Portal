# Music.API — REST API для административного управления музыкальным порталом

Web API (.NET 9) для управления музыкальным порталом. Использует ту же базу данных и те же бизнес-сервисы
(`Music.bisLog`), репозитории и DTO, что и MVC-приложение `Music_portal`.

## Состав

- `Music.API` — новый Web API проект (этот проект).
- `Music.bisLog` — бизнес-сервисы и DTO (существующий проект, не изменяется).
- `Music.DataAccess` — Entity Framework Core, репозитории, Unit of Work (существующий проект, не изменяется).

## Требования

- .NET 9 SDK (достаточно .NET 10 SDK для сборки, при запуске нужен runtime .NET 9).
- SQL Server (по умолчанию `localhost\SQLEXPRESS`, база `MusicPortalDb` — создаётся миграциями MVC-приложения).

## Запуск

```
dotnet restore
dotnet run --project Music.API
```

Swagger будет доступен по адресу: `http://localhost:5090/swagger` (профиль `http`)
или `https://localhost:7090/swagger` (профиль `https`).

Или открыть решение `Music.API.sln` в Visual Studio и запустить профиль `Music.API`.

## Аутентификация (JWT Bearer)

1. Отправьте `POST /api/auth/login` с телом:
   ```json
   { "username": "admin", "password": "..." }
   ```
2. В ответе вы получите `{ token, tokenType, expiresAt, user }`.
3. В Swagger нажмите кнопку **Authorize** и введите `Bearer <token>` (или только сам токен —
   схема настроена как HTTP Bearer).
4. Для остальных запросов передавайте заголовок `Authorization: Bearer <token>`.

Срок действия токена по умолчанию — 8 часов (`JwtSettings:ExpiryMinutes`).

Claims в токене:
- `ClaimTypes.NameIdentifier` — `UserId` (int, как строку),
- `ClaimTypes.Name` — имя пользователя,
- `ClaimTypes.Role` — роль (`Admin` / `User`).

Админские эндпоинты защищены атрибутом `[Authorize(Roles = "Admin")]` и проверяют именно `ClaimTypes.Role`.

## Первый вход администратора

В сидовых данных базы есть пользователь `admin` с ролью `Admin`, но его пароль захеширован и неизвестен.
Варианты:

1. **BootstrapAdmin (рекомендуется для разработки).** В `appsettings.json` включите секцию
   `BootstrapAdmin` (при первом запуске будет создан новый администратор, если пользователь с таким именем не существует):
   ```json
   "BootstrapAdmin": {
     "Enabled": true,
     "Username": "apiadmin",
     "Password": "ChangeMe123!"
   }
   ```
   После создания администратора отключите опцию.

2. **Reset password (админский эндпоинт).** `POST /api/auth/reset-password` (только для роли `Admin`)
   с телом `{ "username": "...", "newPassword": "..." }`.

3. **Change password.** Любой авторизованный пользователь может сменить свой пароль:
   `POST /api/auth/change-password` с телом `{ "oldPassword": "...", "newPassword": "..." }`.

## Папка с аудиофайлами (`uploads`)

Сервисы (`SongService`, `UserService`) сохраняют и читают файлы из папки `uploads/`
относительно **рабочего каталога** процесса (`Directory.GetCurrentDirectory()`).

По умолчанию API создаёт свою папку `uploads/` рядом с собой.

Чтобы API работал с теми же физическими файлами, что и MVC-приложение, укажите абсолютный путь
к папке загрузок MVC в конфигурации `Uploads:SharedPath`. При старте API создаст junction
(без прав администратора) `uploads` -> указанная папка:

```json
"Uploads": {
  "SharedPath": "C:\\Users\\bitse\\source\\repos\\Music_portal\\uploads"
}
```

Если `SharedPath` пуст или недоступен — будет создана обычная локальная папка `uploads`.

## Эндпоинты

### Публичные (без авторизации)

| Метод | Маршрут | Описание |
|---|---|---|
| POST | `/api/auth/register` | Регистрация (`RegisterRequestDto`). Создаёт пользователя с `IsApproved = false`. |
| POST | `/api/auth/login` | Вход, возвращает JWT-токен и `UserDto`. |
| GET | `/api/songs` | Каталог песен. Параметры: `search`, `genreIds` (повторяющиеся int или `1,2`), `sortBy` (`date`/`title`/`popularity`), `page`, `pageSize`. |
| GET | `/api/songs/{id}` | Детали песни (`SongDetailDto`). |
| GET | `/api/songs/{id}/stream` | Стриминг аудио с поддержкой range-запросов (инкрементирует `PlayCount`). |
| GET | `/api/genres` | Жанры со счётчиками песен. |
| GET | `/api/genres/light` | Жанры без счётчиков. |
| GET | `/api/genres/{id}` | Жанр по id. |
| GET | `/api/authors` | Авторы со счётчиками песен. |
| GET | `/api/authors/light` | Авторы без счётчиков. |
| GET | `/api/authors/{id}` | Автор по id. |

### Авторизованные (любой залогиненный пользователь)

| Метод | Маршрут | Описание |
|---|---|---|
| POST | `/api/auth/change-password` | Смена собственного пароля. |
| GET | `/api/songs/my` | Песни текущего пользователя (UserId берётся из claims). |
| POST | `/api/songs` | Создание песни. `multipart/form-data`: `Title`, `Duration`, `Lyrics`, `AuthorIds[]`, `GenreIds[]`, `NewAuthorName`, `AudioFile` (.mp3/.wav, ≤ 20 МБ). |
| PUT | `/api/songs/{id}` | Обновление (владелец или админ). `multipart/form-data`, файл опционален. |
| DELETE | `/api/songs/{id}` | Удаление (владелец или админ). |
| GET | `/api/songs/{id}/download` | Скачивание файла (инкрементирует `PlayCount`). |

### Только администратор

| Метод | Маршрут | Описание |
|---|---|---|
| POST | `/api/auth/reset-password` | Сброс пароля любого пользователя. |
| GET | `/api/users` | Список пользователей (`search`, `page`, `pageSize`). |
| GET | `/api/users/pending` | Неодобренные пользователи. |
| GET | `/api/users/{id}` | Пользователь. |
| POST | `/api/users` | Создание пользователя (`CreateUserDto`). |
| PUT | `/api/users/{id}` | Обновление пользователя (`UpdateUserDto`). |
| DELETE | `/api/users/{id}` | Удаление пользователя (вместе с песнями и файлами). |
| POST | `/api/users/{id}/activate` | Активация (одобрение) пользователя. |
| POST | `/api/users/{id}/reject` | Отклонение (удаление заявки). |
| GET | `/api/admin/songs` | Все песни (`search`, `page`, `pageSize`). |
| GET | `/api/admin/songs/{id}` | Данные песни для редактирования (`AdminSongEditDataDto`). |
| PUT | `/api/admin/songs/{id}` | Админское обновление песни (`multipart/form-data`, поле `UserId` опционально). |
| DELETE | `/api/admin/songs/{id}` | Админское удаление песни. |
| POST | `/api/genres` | Создание жанра. |
| PUT | `/api/genres/{id}` | Обновление жанра. |
| DELETE | `/api/genres/{id}` | Удаление жанра. |
| POST | `/api/authors` | Создание автора. |
| PUT | `/api/authors/{id}` | Обновление автора. |
| DELETE | `/api/authors/{id}` | Удаление автора. |

## Формат ошибок

Все ошибки возвращаются в формате RFC 7807 (`application/problem+json`):

- `400 Bad Request` — невалидный запрос (включая `OperationResult.Fail` от сервисов).
- `401 Unauthorized` — неверные учётные данные/отсутствует токен.
- `403 Forbidden` — недостаточно прав.
- `404 Not Found` — ресурс не найден (в том числе `OperationResult.Fail` с сообщением «... не найден»).
- `500 Internal Server Error` — непредвиденная ошибка (через `GlobalExceptionMiddleware`).

## Примеры запросов

### Логин

```bash
curl -X POST http://localhost:5090/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"apiadmin","password":"ChangeMe123!"}'
```

### Создание песни (multipart)

```bash
curl -X POST http://localhost:5090/api/songs \
  -H "Authorization: Bearer <token>" \
  -F "Title=Моя песня" \
  -F "Duration=200" \
  -F "GenreIds=1" \
  -F "GenreIds=2" \
  -F "AuthorIds=3" \
  -F "AudioFile=@song.mp3;type=audio/mpeg"
```

### Каталог с фильтрами

```
GET /api/songs?search=rock&genreIds=2&genreIds=4&sortBy=popularity&page=1
```

## Конфигурация (`appsettings.json`)

| Секция | Назначение |
|---|---|
| `ConnectionStrings:DefaultConnection` | Строка подключения к SQL Server (та же, что у MVC). |
| `JwtSettings` | `SecretKey`, `Issuer`, `Audience`, `ExpiryMinutes` для подписи/проверки токенов. |
| `Uploads:SharedPath` | Абсолютный путь к общей папке загрузок (опционально). |
| `BootstrapAdmin` | Создание администратора при первом запуске (`Enabled`, `Username`, `Password`). |
| `Cors:AllowedOrigins` | Разрешённые origin'ы (`"*"` — разрешить все, или список). |

**Важно:** замените `JwtSettings:SecretKey` на собственный длинный секрет перед деплоем.