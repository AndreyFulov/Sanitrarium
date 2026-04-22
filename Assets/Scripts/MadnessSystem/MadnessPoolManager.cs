using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер пулов объектов для системы безумия.
/// Хранит списки объектов для нормального состояния и состояния безумия.
/// При изменении рассудка активирует нужные объекты и деактивирует ненужные.
/// 
/// Использование:
/// 1. Добавить компонент на пустой объект на сцене
/// 2. Перетащить объекты в соответствующие списки в Inspector
/// 3. Готово — объекты будут переключаться автоматически
/// </summary>
public class MadnessPoolManager : MonoBehaviour
{
    #region Inspector Fields
    [Header("Объекты для нормального состояния (рассудок высокий)")]
    [Tooltip("Эти объекты будут активны, когда рассудок >= threshold")]
    public List<GameObject> normalObjects = new List<GameObject>();
    
    [Tooltip("Порог рассудка для нормальных объектов (по умолчанию 50)")]
    [Range(0, 100)]
    public int normalThreshold = 50;
    
    [Header("Объекты для состояния безумия (рассудок низкий)")]
    [Tooltip("Эти объекты будут активны, когда рассудок < threshold")]
    public List<GameObject> madnessObjects = new List<GameObject>();
    
    [Tooltip("Порог рассудка для объектов безумия (по умолчанию 50)")]
    [Range(0, 100)]
    public int madnessThreshold = 50;
    
    [Header("Настройки")]
    [Tooltip("Если включено — объекты переключаются мгновенно, если нет — с небольшой задержкой")]
    public bool instantSwitch = true;
    #endregion
    
    #region Private Fields
    private bool isInitialized = false;
    #endregion
    
    #region Lifecycle
    private void Awake()
    {
        // Убедимся, что менеджер не уничтожается при смене сцены (опционально)
        // DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        // Подписка на событие изменения рассудка
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged += UpdatePools;
            
            // Инициализация при старте
            if (!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }
        }
        else
        {
            Debug.LogWarning("[MadnessPoolManager] MadnessManager не найден! Убедитесь, что он есть на сцене.");
        }
    }
    
    private void OnDisable()
    {
        // Отписка от событий
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged -= UpdatePools;
        }
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Инициализация: устанавливает начальную видимость объектов
    /// </summary>
    public void Initialize()
    {
        if (MadnessManager.Instance == null) return;
        
        float currentSanity = MadnessManager.Instance.CurrentSanity;
        UpdatePools(currentSanity);
    }
    
    /// <summary>
    /// Принудительно обновить состояние пулов
    /// </summary>
    public void ForceUpdate()
    {
        if (MadnessManager.Instance != null)
        {
            UpdatePools(MadnessManager.Instance.CurrentSanity);
        }
    }
    
    /// <summary>
    /// Добавить объект в список нормальных объектов во время игры
    /// </summary>
    public void AddToNormalPool(GameObject obj)
    {
        if (obj != null && !normalObjects.Contains(obj))
        {
            normalObjects.Add(obj);
            // Сразу применяем текущее состояние
            UpdateObjectVisibility(obj, MadnessManager.Instance?.CurrentSanity ?? 100f, true);
        }
    }
    
    /// <summary>
    /// Добавить объект в список объектов безумия во время игры
    /// </summary>
    public void AddToMadnessPool(GameObject obj)
    {
        if (obj != null && !madnessObjects.Contains(obj))
        {
            madnessObjects.Add(obj);
            // Сразу применяем текущее состояние
            UpdateObjectVisibility(obj, MadnessManager.Instance?.CurrentSanity ?? 100f, false);
        }
    }
    
    /// <summary>
    /// Удалить объект из всех пулов
    /// </summary>
    public void RemoveFromPools(GameObject obj)
    {
        normalObjects.Remove(obj);
        madnessObjects.Remove(obj);
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Обновляет видимость всех объектов в пулах при изменении рассудка
    /// </summary>
    private void UpdatePools(float sanityValue)
    {
        // Обновляем нормальные объекты
        foreach (var obj in normalObjects)
        {
            if (obj != null)
            {
                UpdateObjectVisibility(obj, sanityValue, true);
            }
        }
        
        // Обновляем объекты безумия
        foreach (var obj in madnessObjects)
        {
            if (obj != null)
            {
                UpdateObjectVisibility(obj, sanityValue, false);
            }
        }
    }
    
    /// <summary>
    /// Обновляет видимость одного объекта в зависимости от рассудка
    /// </summary>
    private void UpdateObjectVisibility(GameObject obj, float sanityValue, bool isNormalObject)
    {
        if (obj == null) return;
        
        bool shouldBeActive;
        
        if (isNormalObject)
        {
            // Нормальный объект: виден когда рассудок ВЫШЕ порога
            shouldBeActive = sanityValue >= normalThreshold;
        }
        else
        {
            // Объект безумия: виден когда рассудок НИЖЕ порога
            shouldBeActive = sanityValue < madnessThreshold;
        }
        
        // Применяем изменение только если состояние отличается
        if (obj.activeSelf != shouldBeActive)
        {
            obj.SetActive(shouldBeActive);
        }
    }
    #endregion
    
    #region Editor Helpers
    private void Reset()
    {
        // Дефолтные значения при добавлении компонента
        normalThreshold = 50;
        madnessThreshold = 50;
        instantSwitch = true;
    }
    
    private void OnValidate()
    {
        // Авто-коррекция порогов в редакторе
        normalThreshold = Mathf.Clamp(normalThreshold, 0, 100);
        madnessThreshold = Mathf.Clamp(madnessThreshold, 0, 100);
    }
    #endregion
}