---
name: youtube-music-automation
description: >-
  Use this skill when creating automated YouTube music videos, generating Suno AI music prompts,
  assembling 1-hour+ playlist videos with crossfades and loudness normalization via Python and FFmpeg,
  and generating timestamped tracklists, SEO titles, tags, and descriptions.
---

# YouTube Music Automation Pipeline (`youtube-music-automation`)

Этот скилл настраивает агента на **полную автоматизацию конвейера YouTube-канала с нейромузыкой**: от генерации идей и промптов для Suno до автоматического монтажа видео длиной 1+ час через Python/FFmpeg и составления SEO-описаний с точными таймкодами.

---

## 1. Пайплайн производства «От треков до загрузки»

```text
[1. Генерация в Suno] 
   └── Промпты по стилям (Lo-Fi, Synthwave, Chillhop, Ambient)
        └── Скачивание MP3/WAV в папку input_tracks/
             │
[2. Автоматическая сборка в 1 клик (build_playlist_video.py)]
   ├── Нормализация громкости (EBU R128 / -14 LUFS под YouTube)
   ├── Плавные кроссфейды между треками (fade-out / fade-in 2-3 сек)
   ├── Наложение фонового изображения или видео-лупа (1080p MP4)
   └── Авто-генерация файла timestamps.txt с точными таймкодами
        │
[3. Публикация и SEO]
   └── Генерация названий с высоким CTR, ключевых тегов и закрепленного комментария
```

---

## 2. Инструменты и скрипты скилла

* [build_playlist_video.py](./scripts/build_playlist_video.py) — Автономный Python-скрипт. Не требует CapCut или ручного монтажа. Принимает папку с треками и картинку/видеофон, на выходе дает готовый MP4-файл и `timestamps.txt`.
* [setup_ffmpeg.ps1](./scripts/setup_ffmpeg.ps1) — PowerShell скрипт установки FFmpeg в Windows за 1 команду через Windows Package Manager (`winget`).
* [Suno Prompt Bible](./references/suno_prompt_bible.md) — База проверенных промптов для Suno v3.5/v4 под фоновую инструментальную музыку без слов (Study, Relaxation, Cyberpunk, Sleep).
* [YouTube Growth & SEO Playbook](./references/youtube_growth_playbook.md) — Правила составления кликабельных названий, шаблоны описаний, теги и стратегии удержания зрителей на длинных плейлистах.

---

## 3. Быстрый старт сборки видео

### Шаг 1: Проверка и установка FFmpeg (если еще не установлен)
Запустить в терминале:
```powershell
powershell -ExecutionPolicy Bypass -File "~/.gemini/config/skills/youtube-music-automation/scripts/setup_ffmpeg.ps1"
```
Или напрямую через `winget`:
```powershell
winget install Gyan.FFmpeg
```

### Шаг 2: Подготовка материалов
1. Создать любую рабочую папку (например, `my_lofi_video`).
2. Поместить туда папку `tracks` с треками из Suno (например `01_cozy_rain.mp3`, `02_midnight_coffee.mp3`...).
3. Положить фоновое изображение `cover.png` (или `cover.jpg`, либо короткий видео-луп `loop.mp4`).

### Шаг 3: Запуск сборки
```powershell
python "~/.gemini/config/skills/youtube-music-automation/scripts/build_playlist_video.py" --input-dir "tracks" --cover "cover.png" --output "lofi_1hour.mp4"
```

Скрипт:
1. Вычислит длительность каждого трека.
2. Сгенерирует `timestamps.txt` с готовой разметкой глав для YouTube:
   ```text
   00:00 01 Cozy Rain
   03:45 02 Midnight Coffee
   07:12 03 Tokyo Neon
   ...
   ```
3. Выровняет громкость звука до стандарта YouTube (-14 LUFS).
4. Отрендерит 1080p MP4 видео с аппаратным или процессорным кодеком.

---

## 4. Как агент помогает в роли продюсера

При обращении пользователя по видео и каналу:
1. **Подбор концепции и промптов**: Агент выдает 10-15 промптов для Suno в едином стиле, чтобы плейлист звучал цельно, без резких перепадов темпа и жанра.
2. **Создание обложек**: Агент может сгенерировать промпт для нейросети (Midjourney / Stable Diffusion / DALL-E / Imagen) для создания привлекательного превью (Thumbnail).
3. **Оформление выпуска**: Агент берет готовый `timestamps.txt` и генерирует законченный блок для публикации: Название, Описание со ссылками, Хештеги и Закрепленный комментарий.
