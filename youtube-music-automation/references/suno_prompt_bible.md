# Suno AI Prompt Bible: Инструментальная музыка для YouTube плейлистов

Это руководство содержит проверенные формулы промптов и метатегов для генерации качественной фоновой музыки без нежелательного вокала на Suno (v3.5 и v4).

---

## 1. Главные правила для фоновых плейлистов в Suno

1. **Режим инструментала**: Всегда включайте переключатель `Instrumental` в Suno.
2. **Защита от случайного пения**: В поле текста (`Lyrics`) всегда указывайте структурные теги:
   ```text
   [Instrumental Intro]
   [Chill Melody]
   [Smooth Bassline]
   [Gentle Beat]
   [Atmospheric Outro]
   [Fade Out]
   [End]
   ```
3. **Консистентность плейлиста**: Не смешивайте кардинально разные BPM в одном выпуске. Для Lo-Fi держите 75-85 BPM, для Synthwave 95-115 BPM, для Ambient 60-70 BPM.

---

## 2. Готовые стили и промпты по жанрам

### Жанр 1: Lo-Fi Hip Hop / Study & Chill
* **Настроение**: Уютное, ностальгическое, для учебы и концентрации (как Lofi Girl).
* **Поле Style of Music**:
  ```text
  lo-fi hip hop, chillhop, jazzy chords, mellow rhodes electric piano, warm vintage vinyl crackle, gentle slow boom bap drum beat, tape saturation, smooth upright bass, 80 bpm, instrumental only, relaxing study vibes
  ```

### Жанр 2: Synthwave / Retrowave / Late Night Drive
* **Настроение**: Неоновый город 80-х, ночная поездка по автостраде, ностальгия.
* **Поле Style of Music**:
  ```text
  retrowave, chill synthwave, 80s analog synthesizers, juno-106 pads, gated reverb snare, smooth bass arpeggio, dreamy nostalgic melody, 100 bpm, night drive aesthetic, instrumental
  ```

### Жанр 3: Deep Focus Ambient / Space Drone
* **Настроение**: Глубокое погружение, программирование, отсутствие отвлекающих звуков.
* **Поле Style of Music**:
  ```text
  deep ambient soundscape, generative modular synth, evolving lush pads, distant sub bass, gentle reverb swell, space drone, 60 bpm, no drums, no beat, meditative, deep flow state, instrumental
  ```

### Жанр 4: Cozy Coffee Shop Jazz / Rainy Day
* **Настроение**: Дождь за окном, теплое кафе, мягкий джаз.
* **Поле Style of Music**:
  ```text
  smooth acoustic jazz, brushed snare drums, gentle grand piano chords, muted trumpet melody, warm double bass, cozy rainy coffee shop ambiance, slow tempo, 75 bpm, organic warm acoustics, instrumental
  ```

### Жанр 5: Cyberpunk / Darksynth Hacking Beats
* **Настроение**: Футуристический хакерский бит, мрачный драйв для гейминга и кодинга.
* **Поле Style of Music**:
  ```text
  cyberpunk midtempo, dark electro synth, distorted analog bass, crisp punchy industrial drums, dark atmosphere, glitch elements, 105 bpm, cinematic futuristic action, instrumental
  ```

### Жанр 6: 432 Hz Sleep & Meditation
* **Настроение**: Засыпание, снятие стресса, глубокая релаксация.
* **Поле Style of Music**:
  ```text
  432Hz healing soundscape, gentle singing bowls, warm celestial pads, soft harp glissando, ultra-slow binaural breathing rhythm, peaceful sleep music, delta waves, completely calming, no percussion, instrumental
  ```

---

## 3. Секретные теги для улучшения качества звука в Suno
Добавляйте эти теги в конец строки `Style of Music`:
* `studio master` — повышает четкость микса.
* `warm analog mix` — сглаживает цифровой резкий верх (hi-hats).
* `wide stereo imaging` — расширяет панораму, делая звук объемным в наушниках.
* `tape saturation, subtle vinyl dust` — добавляет аналоговую теплоту для винтажных жанров.
