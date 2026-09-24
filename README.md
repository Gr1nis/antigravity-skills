# 🚀 Antigravity Global Skills (`Gr1nis`)

Персональный набор глобальных инженерных скиллов для **Google Antigravity** (синхронизация между Windows ПК и MacBook).

---

## 📦 Состав сборки (5 скиллов)

| Скилл | Назначение | Триггеры активации |
| :--- | :--- | :--- |
| **[`vibe-pro`](./vibe-pro/SKILL.md)** | 5-фазный инженерный пайплайн (Бриф → План → Adversarial TDD Red → Green → GitHub Sync + Сабагенты + `PROJECT_STATE.md`) | `режим про`, `по пайплайну`, `pro mode` |
| **[`prompt-master`](./prompt-master/SKILL.md)** | Хирургический оптимизатор промптов и ТЗ (`v1.8.0-ru-compact`) с базой профилей для 30+ ИИ-инструментов и синергией с `vibe-pro` | `промпт-мастер`, `улучши промпт`, `напиши промпт`, `prompt-master` |
| **[`unity-hypercasual-autopilot`](./unity-hypercasual-autopilot/SKILL.md)** | Автопилот разработки игр на Unity C# под **Яндекс Игры (WebGL / YG2)**, **Google Play** и **Steam** | Разработка игр на Unity, Яндекс Игры, гиперказуалки |
| **[`go-mentor`](./go-mentor/SKILL.md)** | Сеньор-ментор (Socratic Study Mode) и продакшен-ассистент (Work Mode) по языку **Go (Golang)** | Изучение Go, ревью Go-кода, микросервисы на Go |
| **[`youtube-music-automation`](./youtube-music-automation/SKILL.md)** | Автоматизация YouTube-канала с нейромузыкой (Suno AI промпты + сборка 1ч+ видео через Python/FFmpeg + таймкоды) | YouTube плейлисты, Suno промпты, сборка музыкальных видео |

---

## 💻 Установка на MacBook (macOS) за 1 команду

Откройте терминал на MacBook и выполните:

```bash
mkdir -p ~/.gemini/config
git clone https://github.com/Gr1nis/antigravity-skills.git ~/.gemini/config/skills
```

*(Если папка `~/.gemini/config/skills` уже существует и пуста или содержит старые файлы, можно выполнить:)*
```bash
rm -rf ~/.gemini/config/skills && git clone https://github.com/Gr1nis/antigravity-skills.git ~/.gemini/config/skills
```

---

## 🔄 Синхронизация изменений между ПК и MacBook

* **Забрать свежие обновления скиллов:**
  ```bash
  git -C ~/.gemini/config/skills pull
  ```
* **Отправить изменения скиллов в облако:**
  ```bash
  cd ~/.gemini/config/skills
  git add .
  git commit -m "chore: update skills"
  git push
  ```
