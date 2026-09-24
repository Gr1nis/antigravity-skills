---
name: unity-hypercasual-autopilot
description: >-
  Use this skill when developing Unity C# hypercasual or casual games for Yandex Games (WebGL / PluginYG / YG2),
  Google Play, and Steam with a turnkey, minimal-maintenance approach. Generates robust, self-contained scripts,
  monetization templates, WebGL optimization, build guides, and publisher-ready architecture.
---

# Unity Hypercasual Autopilot (`unity-hypercasual-autopilot`)

Этот скилл настраивает агента на **автономную разработку гиперказуальных и казуальных игр на Unity (C#) «под ключ»** с прицелом на релиз в **Яндекс Играх (WebGL)**, **Google Play** и **Steam** для генерации пассивного дохода с минимальной поддержкой.

---

## 1. Философия разработки: «Минимум рутины, максимум автономности»

Пользователь ожидает, что агент берет на себя всю черновую работу по написанию кода, архитектуре и логике, минимизируя ручные действия в редакторе Unity:

1. **Самодостаточные компоненты (Self-contained MonoBehaviours)**:
   * Скрипты не должны требовать 10 шагов ручной настройки в инспекторе.
   * Использовать `[RequireComponent(...)]`, `GetComponent<T>()` в `Awake()` или `TryGetComponent<T>()` с fallback-поведением.
   * Использовать атрибуты `[Header]`, `[Tooltip]`, `[Range]` и `[SerializeField]` с понятными дефолтными значениями, чтобы код работал «из коробки» при перетаскивании на GameObject.
2. **Слабая связанность через события (Decoupled Event Architecture)**:
   * Не писать спагетти-ссылки между синглтонами.
   * Использовать стандартные события C# (`public static event Action<int> OnScoreChanged;`) или безопасные брокеры событий, чтобы удаление или отсутствие UI не ломало физику и логику игры.
3. **«Сделал и забыл» (No-Maintenance Design)**:
   * Устойчивость к ошибкам: если звуковой файл или спрайт не назначен, игра не должна падать с `NullReferenceException` — код обязан мягко обработать `if (clip != null) audioSource.PlayOneShot(clip);`.
   * Автосохранение и корректная пауза звука при потере фокуса вкладки браузера (`OnApplicationFocus`, `OnApplicationPause`) — критично для прохождения модерации Яндекс Игр.

---

## 2. Ключевые модули скилла

В скилл включены готовые производственные скрипты и руководства:

* [SaveManager.cs](./scripts/SaveManager.cs) — Универсальный менеджер сохранений (JSON + шифрование XOR/Base64 + fallback на `PlayerPrefs` / облако Яндекс Игр).
* [AdManager.cs](./scripts/AdManager.cs) — Готовая обертка монетизации (Яндекс Игры PluginYG / YG2, AdMob, Unity Ads): межстраничная реклама (Interstitial с кулдауном и паузой звука), вознаграждение за просмотр (Rewarded Ad: 2x монет или возрождение).
* [Рецепты гиперказуальных механик](./references/hypercasual_gameplay_recipes.md) — Готовые контроллеры: раннер (runner swipe + мышь/клавиатура для WebGL), стакинг предметов, idle-кликер с формулой оффлайн-дохода, тап-тайминг.
* [Чеклист Яндекс Игр (WebGL / YG2 / Модерация)](./references/yandex_games_checklist.md) — Все правила модерации Яндекса (пауза звука в вкладке, запрет кнопки Выход, `GameReadyAPI`, сжатие WebGL до 15 МБ, RU/EN/TR локализация).
* [Чеклист релиза в Google Play и Steam](./references/release_checklist.md) — Пошаговый гайд по генерации Keystore, настройке Gradle, Android 14+ (API 34+), а также интеграции Steamworks SDK.

---

## 3. Стандарты написания Unity C# кода

При создании любого скрипта для игры агент обязан соблюдать:

1. **Именование и структура класса**:
   ```csharp
   using System;
   using System.Collections;
   using UnityEngine;

   namespace Hypercasual.Core
   {
       [SelectionBase]
       [DisallowMultipleComponent]
       public class PlayerController : MonoBehaviour
       {
           [Header("Movement Settings")]
           [SerializeField] private float forwardSpeed = 8f;
           [SerializeField] private float horizontalSpeed = 5f;
           [SerializeField] private float sideLimit = 3.5f;

           // Публичные события
           public static event Action OnPlayerDied;
           public static event Action<int> OnCoinCollected;

           private void Awake() { ... }
           private void Update() { ... }
       }
   }
   ```
2. **Оптимизация под мобильные устройства (60 FPS гарантировано)**:
   * **Никакого Garbage Collection в цикле**: запрещены `new` объектов, аллокации строк (`"Score: " + score`), вызовы LINQ в `Update()`.
   * Использовать объектные пулы (`ObjectPool<T>` или простейшие списки неактивных объектов) для пуль, монеток и эффектов.
   * Кэшировать компоненты в `Awake()`: `private Transform _transform; private Rigidbody _rb;`.
   * Избегать `FindObjectOfType` и `GameObject.Find` во время игрового процесса.

3. **Game Feel ("Juice") — обязательный элемент гиперказуалок**:
   * В каждый контроллер агент добавляет сочность: микро-встряску камеры (`CameraShake`), легкий скейл при тапе (Squash & Stretch), питч-модуляцию звука монет (`audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f)`), всплывающие цифры очков.

---

## 4. Алгоритм создания новой игры с нуля

Когда пользователь говорит: «Сделай игру [жанр/идея]»:
1. **Гейм-дизайн документ (1 минута)**: Предложить ключевой кор-луп (Core Loop) из 3 шагов: Действие -> Награда -> Прокачка.
2. **Скрипты логики**: Написать базовый контроллер персонажа, спавнер препятствий/монет, менеджер уровней/смерти.
3. **Монетизация & UI**: Подключить `AdManager` (показ рекламы каждые $N$ проигрышей) и `SaveManager`.
4. **Инструкция в Unity**: Выдать краткую инструкцию (например: «1. Создайте 3D Plane. 2. Создайте куб с тегом Player и прикрепите этот скрипт. 3. Нажмите Play»).
