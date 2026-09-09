# 🎵 Музыкальный портал

## Запуск (корень)

**Один файл в корне — `MusicPortal.sln` (или `MusicPortal.slnx` для VS 2022+):**
- `MusicPortal.sln` — 5 проектов: `Music_portal` (сайт `https://localhost:7078`), `Music.API` (`https://localhost:7090/swagger`), `Music.DataAccess`, `Music.bisLog`, `Tests`
- `MusicPortal.slnx` — то же, новый XML-формат

**Как запустить:**
1. `dotnet ef database update --project Music.DataAccess --startup-project Music.API` (1 раз, создает `MusicPortalDb` + `admin/admin123`)
2. **Visual Studio:** открыть `MusicPortal.sln` в **корне** → `F5` (авто `Multiple startup projects`: `Music.API` + `Music_portal`) → `https://localhost:7078`
3. **CLI:** `dotnet run --project Music.API/Music.API.csproj` + `dotnet run --project Music_portal/Music_portal.csproj`
4. **React (отдельно):** `npm --prefix React_site install && npm --prefix React_site run dev` → `http://localhost:5173` (опционально, `SPA` уже в `Music_portal/Admin/Spa`)

**Не запускать:** старые `Music.API.sln` (корень) и `Music_portal/Music_portal.sln` (в подпапке) — оставлены для совместимости, но главный — `MusicPortal.sln` в корне.
