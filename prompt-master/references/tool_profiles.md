# Полный каталог профилей ИИ-инструментов (Tool Profiles)

Этот справочник загружается агентом по требованию, когда пользователь просит составить или адаптировать промпт под конкретный внешний инструмент из списка ниже.

---

## 1. Языковые и Reasoning-модели (LLM)

### Claude (claude.ai, Claude API, Claude 5 / актуальные модели)
- По умолчанию используй **Claude Opus 5** (`claude-opus-5`) для сложного агентного кодинга, **Claude Fable 5** (`claude-fable-5`) для долгих автономных задач, **Claude Sonnet 5** (`claude-sonnet-5`) для баланса скорости и интеллекта, **Claude Haiku 4.5** для быстрых задач.
- Используй XML-теги (`<context>`, `<task>`, `<constraints>`, `<output_format>`) для смешанного контента.
- Для длинного контекста помещай исходные документы ПЕРЕД вопросом внутри XML-тегов.
- Предпочитай позитивные инструкции («сделай так») вместо длинных списков запретов.
- Не запрашивай скрытый Chain-of-Thought; проси краткое обоснование, доказательства и проверки.

### ChatGPT / GPT-5.6 / OpenAI GPT
- Семейство GPT-5.6: **Sol** (`gpt-5.6-sol` / `gpt-5.6`) для флагманских задач, **Terra** (`gpt-5.6-terra`) для повседневных, **Luna** (`gpt-5.6-luna`) для быстрых потоковых задач.
- Используй 4 компактные секции: `Goal`, `Context`, `Constraints`, `Done`.
- Задавай четкие границы автономности: что можно менять локально, а что требует подтверждения.

### o3 / o4-mini / DeepSeek-R1 / Qwen3 (Thinking Mode)
- **ТОЛЬКО короткие и чистые инструкции** — модели рассуждают внутри на тысячах токенов.
- **НИКОГДА не добавляй CoT** («думай шаг за шагом») — это ухудшает качество.
- Указывай только цель и критерий готовности (`Done When`). Системный промпт — строго до 200 слов.
- Для DeepSeek-R1 / MiniMax при необходимости добавляй: `"Output only the final answer, no <think> reasoning tags."`

### Grok / Grok 4.6 / xAI
- Структура: `Goal`, `Context/Input`, `Constraints`, `Tools/Permissions`, `Done`.
- Для актуальных фактов явно требуй включения `Web Search` или `X Search` и цитирования источников.
- Указывай уровень `reasoning effort` (`low`, `medium`, `high`, `xhigh`) только если пользователь работает через API.

### Gemini 2.x / Gemini 3 Pro
- Сильная сторона — огромный контекст и мультимодальность.
- Защита от галлюцинаций цитат: `"Cite only sources you are certain of. If uncertain, say [uncertain]."`
- Защита от ухода из формата: всегда прикладывай жесткий шаблон вывода (`Format Lock`) с примером.

### Qwen 2.5 (Instruct) / Ollama / Llama / Mistral (Локальные модели)
- **Ollama**: ВСЕГДА уточняй, какая именно модель запущена (Llama3, Mistral, Qwen2.5-Coder), и выдавай `SYSTEM` промпт для `Modelfile`. Температура `0.1` для кода, `0.7–0.8` для креатива.
- Избегай глубокой вложенности — плоская и короткая структура работает лучше всего.

---

## 2. Агентные IDE и Кодинг-агенты

### Antigravity (Google Agent-First IDE)
- Описывай **конечный результат (outcome) и инварианты**, а не микро-шаги.
- Требуй создания артефакта (`implementation_plan.md`) перед крупными изменениями.
- Для веб/UI задач включай проверку: `"Verify UI at 375px and 1440px"`.
- Четко задавай границы автономности и список файлов (`File Scope`).

### Claude Code / Codex CLI / Cline / Devin
- Обязательная формула: `Starting State` + `Target State` + `File Scope` (какие файлы трогать, какие НЕ трогать) + `Verification Commands` + `Stop Conditions`.
- Стоп-триггеры обязательны: `"Stop and ask before deleting any file, adding any dependency, or changing DB schema."`

### Cursor / Windsurf / GitHub Copilot
- **Cursor / Windsurf**: Точный путь к файлу + имя функции + текущее поведение + желаемое изменение + `Do-not-touch list` + `Done when`.
- **GitHub Copilot**: Точная сигнатура функции + docstring с типами входов/выходов и граничными случаями прямо перед местом вызова.

### Bolt / v0 / Lovable / Figma Make / Google Stitch
- Генераторы полного стека склонны к раздуванию кода — жестко ограничивай скоуп: `"Do not add authentication, dark mode, or extra libraries not explicitly listed."`
- Указывай точные hex-цвета, отступы в px, шрифты и состояния hover/scroll.

---

## 3. Генераторы Изображений, 3D, Видео, Аудио и Автоматизации

### Image AI (Midjourney, DALL-E 3, Stable Diffusion, ComfyUI, SeeDream)
- **Промпт всегда на английском языке.**
- **Midjourney**: Дескрипторы через запятую (не проза). Порядок: `Subject -> Style -> Mood -> Lighting -> Composition -> --ar 16:9 --v 6 --style raw --no [unwanted]`.
- **DALL-E 3**: Связное описание по планам (foreground, midground, background) + `"do not include text unless specified"`.
- **Stable Diffusion / ComfyUI**: Два отдельных блока (`Positive Prompt` с весами `(word:1.2)` и обязательный `Negative Prompt`), `CFG 7-12`, `Steps 20-40`.
- **Редактирование по референсу**: Описывай ТОЛЬКО дельту (что меняется и что обязано остаться неизменным).

### 3D AI и Игровые Движки (Meshy, Tripo, Rodin, Unity AI, BlenderGPT)
- **Meshy / Tripo / Rodin**: `Style (low-poly / stylized / PBR realistic) + Subject + Key features + Material + Export format (GLB/FBX) + A-pose/T-pose (для персонажей)` + Negative: `"no background, no base, no floating parts"`.
- **Unity AI (Unity 6.2+)**: Четко разделяй `/ask` (вопросы по проекту), `/run` (автоматизация Editor-действий), `/code` (генерация C# скриптов с указанием namespace и компонентов).

### Video AI (Sora, Runway Gen-3, Kling, LTX Video, Dream Machine)
- Описывай сцену языком кинорежиссера: **движение камеры** (`static shot`, `slow dolly in`, `crane shot`), физика движения объекта, освещение, фокусное расстояние объектива и цветокоррекция.

### Voice & Music AI (ElevenLabs, Suno)
- **ElevenLabs**: Указывай эмоцию, темп речи, паузы и акценты (SSML-разметка), а не абстрактные метафоры.
- **Suno**: Жанр, BPM, ведущие инструменты, атмосфера, структура (`[Intro]`, `[Drop]`, `[Outro]`), флаг `Instrumental` для фоновой музыки.

### Workflow AI & Browser Agents (n8n, Make, Zapier, Perplexity Comet, OpenAI Atlas)
- **n8n / Make / Zapier**: Нумерованная цепочка `Trigger App (Event) -> Action App (Action) -> Точный маппинг полей JSON`.
- **Browser Agents**: Описывай конечную цель поиска/действия и жесткий стоп-кран: `"Ask me before submitting any form, making any purchase, or sending any message."`
