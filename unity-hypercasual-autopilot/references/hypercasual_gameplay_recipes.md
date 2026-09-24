# Рецепты гиперказуальных механик на Unity C#

Готовые, автономные компоненты для типовых механик гиперказуальных игр. Каждый скрипт можно сразу прикрепить к объекту на сцене.

---

## 1. Контроллер раннера (Swerve Runner Controller)
Персонаж непрерывно бежит вперед по оси Z, а игрок свайпом или мышью управляет смещением влево/вправо по оси X с ограничением по ширине трассы.

```csharp
using UnityEngine;

namespace Hypercasual.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class SwerveRunnerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float forwardSpeed = 7f;
        [SerializeField] private float swerveSpeed = 15f;
        [SerializeField] private float maxRoadWidth = 3f;

        private Rigidbody _rb;
        private Vector2 _lastTouchPos;
        private float _swerveInput;
        private bool _isPlaying = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.isKinematic = true;
        }

        private void Update()
        {
            if (!_isPlaying) return;

            // Обработка ввода (работает на ПК и смартфонах)
            if (Input.GetMouseButtonDown(0))
            {
                _lastTouchPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                float deltaX = Input.mousePosition.x - _lastTouchPos.x;
                _swerveInput = deltaX / Screen.width;
                _lastTouchPos = Input.mousePosition;
            }
            else
            {
                _swerveInput = 0f;
            }

            // Движение вперед + смещение влево/вправо
            Vector3 pos = transform.position;
            pos.z += forwardSpeed * timeDelta();
            pos.x += _swerveInput * swerveSpeed;
            pos.x = Mathf.Clamp(pos.x, -maxRoadWidth, maxRoadWidth);

            transform.position = pos;
        }

        private float timeDelta() => Time.deltaTime;

        public void StopRunning() => _isPlaying = false;
    }
}
```

---

## 2. Механика тап-тайминга (Tap Timing / Stacker)
Блок движется туда-обратно. По тапу игрока блок фиксируется, проверяется точность попадания, начисляются очки или запускается комбо.

```csharp
using System;
using UnityEngine;

namespace Hypercasual.Gameplay
{
    public class TapTimingBlock : MonoBehaviour
    {
        [Header("Oscillation")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float travelDistance = 2.5f;

        public static event Action<bool> OnBlockPlaced; // true = perfect, false = miss

        private bool _isStopped = false;
        private float _startX;

        private void Start()
        {
            _startX = transform.position.x;
        }

        private void Update()
        {
            if (_isStopped) return;

            // Гармоническое движение туда-обратно
            float newX = _startX + Mathf.PingPong(Time.time * moveSpeed, travelDistance * 2f) - travelDistance;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            if (Input.GetMouseButtonDown(0))
            {
                StopAndEvaluate();
            }
        }

        private void StopAndEvaluate()
        {
            _isStopped = true;
            float distanceFromCenter = Mathf.Abs(transform.position.x - _startX);

            // Если отклонение меньше 0.2 единиц - идеальное попадание
            if (distanceFromCenter < 0.25f)
            {
                // Примагничиваем точно в центр
                transform.position = new Vector3(_startX, transform.position.y, transform.position.z);
                OnBlockPlaced?.Invoke(true);
            }
            else if (distanceFromCenter > 1.2f)
            {
                // Промах (падение)
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                OnBlockPlaced?.Invoke(false);
            }
            else
            {
                OnBlockPlaced?.Invoke(false);
            }
        }
    }
}
```

---

## 3. Математика оффлайн-дохода для Idle-игр (Offline Earnings)
Вычисляет, сколько времени игрок отсутствовал в игре, и начисляет монеты при входе с лимитом времени (например, максимум 8 часов оффлайна).

```csharp
using System;
using UnityEngine;
using Hypercasual.Core;

namespace Hypercasual.Gameplay
{
    public class IdleOfflineIncome : MonoBehaviour
    {
        [Header("Idle Rates")]
        [Tooltip("Монет в минуту при пассивном доходе")]
        [SerializeField] private int coinsPerMinute = 10;
        [Tooltip("Максимальное время оффлайн-накопления в часах")]
        [SerializeField] private float maxOfflineHours = 8f;

        public static event Action<int> OnOfflineRewardAvailable;

        private void Start()
        {
            CalculateOfflineEarnings();
        }

        private void CalculateOfflineEarnings()
        {
            string lastDateStr = SaveManager.Data.LastLoginDate;
            if (string.IsNullOrEmpty(lastDateStr)) return;

            if (DateTime.TryParse(lastDateStr, out DateTime lastLogin))
            {
                TimeSpan elapsed = DateTime.UtcNow - lastLogin;
                double minutes = elapsed.TotalMinutes;

                if (minutes >= 5.0) // Начисляем, только если игрока не было больше 5 минут
                {
                    double maxMinutes = maxOfflineHours * 60.0;
                    double validMinutes = Math.Min(minutes, maxMinutes);
                    int totalCoinsEarned = (int)(validMinutes * coinsPerMinute);

                    if (totalCoinsEarned > 0)
                    {
                        SaveManager.Instance.AddCoins(totalCoinsEarned);
                        OnOfflineRewardAvailable?.Invoke(totalCoinsEarned);
                        Debug.Log($"[Idle] Начислено за {validMinutes:F0} мин оффлайна: +{totalCoinsEarned} монет");
                    }
                }
            }
        }
    }
}
```

---

## 4. Сочность игры: Встряска камеры и Squash & Stretch
Компонент «Game Feel» без сторонних ассетов. Добавляет эффект удара, подбора монеты или прыжка.

```csharp
using System.Collections;
using UnityEngine;

namespace Hypercasual.Effects
{
    public class SimpleJuice : MonoBehaviour
    {
        public static SimpleJuice Instance { get; private set; }

        private void Awake() => Instance = this;

        /// <summary>
        /// Встряска камеры
        /// </summary>
        public void ShakeCamera(float duration = 0.15f, float magnitude = 0.2f)
        {
            StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            Camera cam = Camera.main;
            if (cam == null) yield break;

            Vector3 originalPos = cam.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = (UnityEngine.Random.value * 2f - 1f) * magnitude;
                float y = (UnityEngine.Random.value * 2f - 1f) * magnitude;
                cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            cam.transform.localPosition = originalPos;
        }

        /// <summary>
        /// Эффект сплющивания/растяжения объекта (Squash and Stretch)
        /// </summary>
        public void SquashAndStretch(Transform target, Vector3 stretchScale, float speed = 10f)
        {
            StartCoroutine(SquashRoutine(target, stretchScale, speed));
        }

        private IEnumerator SquashRoutine(Transform target, Vector3 stretchScale, float speed)
        {
            if (target == null) yield break;
            Vector3 originalScale = target.localScale;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                target.localScale = Vector3.Lerp(originalScale, Vector3.Scale(originalScale, stretchScale), Mathf.Sin(t * Mathf.PI));
                yield return null;
            }
            target.localScale = originalScale;
        }
    }
}
```
