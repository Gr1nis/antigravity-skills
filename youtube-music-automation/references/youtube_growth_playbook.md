# YouTube Music Channel Growth & SEO Playbook

Практическое руководство по упаковке и продвижению длинных (1+ час) музыкальных плейлистов на YouTube.

---

## 1. Анатомия вирусного заголовка (High-CTR Titles)

На YouTube в музыкальной нише заголовок должен отвечать на вопрос: **«Какое эмоциональное состояние или какую задачу закроет этот плейлист?»**.

### Формулы заголовков:
1. **[Эмоция / Действие] + [Стиль] + [Контекст / Время суток]**:
   * *midnight coding sessions ~ lofi hip hop / chill beats to code/relax to*
   * *it's 2am and you are walking in Tokyo rain [chill synthwave mix]*
   * *coffee shop jazz on a rainy autumn morning ~ relax, read, study [1 HOUR]*
2. **Идентификация со зрителем (Role / Identity)**:
   * *music for programmers who code late at night*
   * *synthwave for driving through an empty neon city*
   * *deep focus ambient music for studying and reading (no words)*

---

## 2. Идеальная структура описания (Description Template)

YouTube алгоритмы сканируют первые 3 строки описания для ранжирования в поиске и рекомендованных:

```text
Escape into a world of chill beats and relaxing vibes. Perfect for studying, coding, reading, or falling asleep. 
If you enjoyed this mix, don't forget to like and subscribe for weekly playlists! ☕🌧️

Tracklist:
00:00 01 Rainy Window
03:42 02 Midnight Coding
07:15 03 Tokyo Neon Lights
11:04 04 Coffee Steam
... [вставить содержимое timestamps.txt]

✨ Visuals & Audio:
All audio tracks generated with Suno AI, mixed and mastered with EBU R128 loudness normalization.
Cover art created with AI imaging tools.

#lofi #chillbeats #studymusic #relaxingmusic #ambient #codingmusic
```

---

## 3. Правила оформления превью (Thumbnail)

* **Цветовая палитра**: Теплые приглушенные тона для Lo-Fi (оранжевый, глубокий синий, цвет дождя, кофейный). Неоновый фиолетовый/бирюзовый для Synthwave.
* **Сюжетность**: Одинокий персонаж за столом у окна с чашкой кофе; вид из окна поезда на ночной город; неоновая машина на трассе.
* **Текст на обложке**: Либо минимальный (1-2 слова: `LATE NIGHT`, `RAINY DAY`, `FOCUS`), либо **вообще без текста**. Минималистичные эстетичные обложки без надписей в музыкальной нише часто имеют CTR выше 8-10%.

---

## 4. Секреты удержания (Retention) на часовых видео

1. **Главы YouTube (Chapters)**: Наличие таймкодов в описании (начиная строго с `00:00`) автоматически разбивает видео на главы в плеере. Это ранжируется алгоритмами YouTube в Google Search!
2. **Закрепленный комментарий (Pinned Comment)**:
   * Напишите вопрос зрителям: *«Где вы сейчас слушаете этот микс и какая у вас сейчас погода? Всем продуктивного дня!»*.
   * Музыкальные стримы и плейлисты собирают огромную вовлеченность в комментариях, превращая канал в уютное комьюнити.
3. **Бесшовные переходы**: Никаких пауз тишины между треками. Скрипт `build_playlist_video.py` автоматически сглаживает громкость и переходы.
