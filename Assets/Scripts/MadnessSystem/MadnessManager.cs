using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Глобальный менеджер системы безумия
/// Хранит текущее значение рассудка и уведомляет все подписанные объекты об изменениях
/// НЕ хранит ссылки на объекты сцены - работает через события
/// </summary>
public class MadnessManager : MonoBehaviour
{
    #region Singleton
    public static MadnessManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Sanity State
    [Header("Настройки рассудка")]
    [Range(0, 100)]
    [Tooltip("Текущее значение рассудка")]
    public float currentSanity = 100f;

    [Range(0, 100)]
    [Tooltip("Минимальное значение рассудка")]
    public float minSanity = 0f;

    [Range(0, 100)]
    [Tooltip("Максимальное значение рассудка")]
    public float maxSanity = 100f;

    [Tooltip("Автоматическое восстановление рассудка (в секунду)")]
    public float sanityRegenRate = 0f;

    /// <summary>
    /// Текущее значение рассудка (только чтение, использовать ChangeSanity для изменения)
    /// </summary>
    public float CurrentSanity => currentSanity;

    /// <summary>
    /// Находится ли игрок в состоянии безумия
    /// </summary>
    public bool IsInMadness => currentSanity < 50f;
    #endregion

    #region Events
    /// <summary>
    /// Событие вызывается при любом изменении рассудка
    /// Передаёт новое значение рассудка (0-100).
    /// </summary>
    public event Action<float> OnSanityChanged;

    /// <summary>
    /// Событие вызывается при переходе в состояние безумия
    /// </summary>
    public event Action OnMadnessEntered;

    /// <summary>
    /// Событие вызывается при выходе из состояния безумия
    /// </summary>
    public event Action OnMadnessExited;

    /// <summary>
    /// Событие вызывается при достижении критического уровня рассудка
    /// </summary>
    public event Action OnCriticalSanity;
    #endregion

    #region Private Fields
    private bool wasInMadness = false;
    private const float CRITICAL_SANITY_THRESHOLD = 20f;
    private bool hasReachedCritical = false;
    #endregion

    #region Lifecycle
    private void Start()
    {
        wasInMadness = IsInMadness;
    }

    private void Update()
    {
        // Автоматическое восстановление рассудка
        if (sanityRegenRate > 0 && currentSanity < maxSanity)
        {
            ChangeSanity(sanityRegenRate * Time.deltaTime);
        }

        // Проверка перехода через порог безумия
        CheckMadnessThreshold();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Изменить рассудок на указанное значение.
    /// Положительное значение — восстановление, отрицательное — потеря
    /// </summary>
    public void ChangeSanity(float delta)
    {
        float oldValue = currentSanity;
        currentSanity = Mathf.Clamp(currentSanity + delta, minSanity, maxSanity);

        // Вызываем событие только если значение изменилось значительно
        if (Mathf.Abs(currentSanity - oldValue) > 0.1f)
        {
            OnSanityChanged?.Invoke(currentSanity);
        }
    }

    /// <summary>
    /// Установить конкретное значение рассудка
    /// </summary>
    public void SetSanity(float value)
    {
        ChangeSanity(value - currentSanity);
    }

    /// <summary>
    /// Мгновенно вогнать в состояние безумия (для тестов или скриптов)
    /// </summary>
    public void ForceMadness()
    {
        SetSanity(49f);
    }

    /// <summary>
    /// Полностью восстановить рассудок
    /// </summary>
    public void RestoreFullSanity()
    {
        SetSanity(maxSanity);
    }
    #endregion

    #region Private Methods
    private void CheckMadnessThreshold()
    {
        bool isInMadness = IsInMadness;

        // Переход в безумие
        if (isInMadness && !wasInMadness)
        {
            wasInMadness = true;
            OnMadnessEntered?.Invoke();
            Debug.Log("[MadnessManager] Игрок вошёл в состояние безумия");
        }
        // Выход из безумия
        else if (!isInMadness && wasInMadness)
        {
            wasInMadness = false;
            OnMadnessExited?.Invoke();
            Debug.Log("[MadnessManager] Игрок вышел из состояния безумия");
        }

        // Критический уровень рассудка
        if (currentSanity <= CRITICAL_SANITY_THRESHOLD && !hasReachedCritical)
        {
            hasReachedCritical = true;
            OnCriticalSanity?.Invoke();
            Debug.Log("[MadnessManager] Критический уровень рассудка!");
        }
    }
    #endregion

    #region Debug
    /// <summary>
    /// Отладочный UI для тестирования в редакторе
    /// Удалить или отключить перед релизом!
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 250, 200));
        GUILayout.Box("Madness Manager (DEBUG)");

        GUILayout.Label($"Рассудок: {currentSanity:F1} / {maxSanity}");
        GUILayout.Label($"Статус: {(IsInMadness ? "БЕЗУМИЕ" : "НОРМА")}");

        GUILayout.Space(10);

        if (GUILayout.Button("-10 Рассудка"))
            ChangeSanity(-10f);

        if (GUILayout.Button("+10 Рассудка"))
            ChangeSanity(10f);

        if (GUILayout.Button("Вогнать в безумие"))
            ForceMadness();

        if (GUILayout.Button("Полное восстановление"))
            RestoreFullSanity();

        GUILayout.EndArea();
    }
    #endregion
}