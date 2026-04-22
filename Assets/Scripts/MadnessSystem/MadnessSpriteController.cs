using UnityEngine;

/// <summary>
/// Контроллер спрайта, реагирующий на изменения рассудка
/// Вешается на объект со SpriteRenderer
/// Подписывается на события MadnessManager и обновляет спрайт автоматически
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class MadnessSpriteController : MonoBehaviour
{
    #region Inspector Fields
    [Header("Данные безумия")]
    [Tooltip("ScriptableObject с парой спрайтов (норма/безумие)")]
    public MadnessSpriteData spriteData;
    
    [Header("Настройки")]
    [Tooltip("Использовать плавный переход вместо мгновенного")]
    public bool useSmoothTransition = false;
    
    [Tooltip("Скорость плавного перехода")]
    [Range(0.1f, 10f)]
    public float transitionSpeed = 5f;
    #endregion
    
    #region Private Fields
    private SpriteRenderer spriteRenderer;
    private Sprite normalSprite;
    private Sprite madnessSprite;
    private Color originalColor;
    private Vector3 originalScale;
    private float currentLerpValue = 0f;
    private int madnessThreshold = 50;
    private Color madnessColorTint = Color.white;
    private float madnessScaleMultiplier = 1f;
    #endregion
    
    #region Lifecycle
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Сохраняем исходные значения
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
        
        // Загружаем данные из ScriptableObject
        if (spriteData != null)
        {
            normalSprite = spriteData.normalSprite;
            madnessSprite = spriteData.madnessSprite;
            madnessThreshold = spriteData.madnessThreshold;
            useSmoothTransition = spriteData.useSmoothTransition;
            madnessColorTint = spriteData.madnessColorTint;
            madnessScaleMultiplier = spriteData.madnessScaleMultiplier;
        }
        else
        {
            // Если данных нет, используем текущий спрайт как нормальный
            normalSprite = spriteRenderer.sprite;
            madnessSprite = null;
            Debug.LogWarning($"[MadnessSpriteController] На объекте {gameObject.name} не заданы MadnessSpriteData!");
        }
    }
    
    private void OnEnable()
    {
        // ПОДПИСКА НА СОБЫТИЯ
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged += UpdateSprite;
            MadnessManager.Instance.OnMadnessEntered += OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited += OnMadnessExited;
            
            // Сразу обновляем спрайт при включении объекта
            UpdateSprite(MadnessManager.Instance.CurrentSanity);
        }
        else
        {
            Debug.LogWarning($"[MadnessSpriteController] MadnessManager не найден на сцене!");
        }
    }
    
    private void OnDisable()
    {
        // ОТПИСКА ОТ СОБЫТИЙ
        if (MadnessManager.Instance != null)
        {
            MadnessManager.Instance.OnSanityChanged -= UpdateSprite;
            MadnessManager.Instance.OnMadnessEntered -= OnMadnessEntered;
            MadnessManager.Instance.OnMadnessExited -= OnMadnessExited;
        }
    }
    
    private void OnDestroy()
    {
        // Дополнительная защита на случай уничтожения объекта
        OnDisable();
    }
    #endregion
    
    #region Event Handlers
    /// <summary>
    /// Вызывается событием OnSanityChanged из MadnessManager
    /// Обновляет спрайт в зависимости от текущего рассудка
    /// </summary>
    private void UpdateSprite(float sanityValue)
    {
        if (spriteRenderer == null) return;
        
        bool shouldBeMadness = sanityValue < madnessThreshold;
        
        if (useSmoothTransition)
        {
            UpdateSmooth(shouldBeMadness);
        }
        else
        {
            UpdateInstant(shouldBeMadness);
        }
    }
    
    /// <summary>
    /// Вызывается при входе в состояние безумия
    /// Можно добавить дополнительные эффекты (звук, частицы)
    /// </summary>
    private void OnMadnessEntered()
    {
        // Опционально: добавить эффекты при входе в безумие
        // AudioManager.Instance.Play("MadnessStart");
    }
    
    /// <summary>
    /// Вызывается при выходе из состояния безумия
    /// </summary>
    private void OnMadnessExited()
    {
        // Опционально: добавить эффекты при выходе из безумия
        // AudioManager.Instance.Play("MadnessEnd");
    }
    #endregion
    
    #region Private Methods
    private void UpdateInstant(bool shouldBeMadness)
    {
        if (shouldBeMadness && madnessSprite != null)
        {
            spriteRenderer.sprite = madnessSprite;
            spriteRenderer.color = madnessColorTint;
            transform.localScale = originalScale * madnessScaleMultiplier;
        }
        else
        {
            spriteRenderer.sprite = normalSprite;
            spriteRenderer.color = originalColor;
            transform.localScale = originalScale;
        }
    }
    
    private void UpdateSmooth(bool shouldBeMadness)
    {
        float targetLerp = shouldBeMadness ? 1f : 0f;
        currentLerpValue = Mathf.Lerp(currentLerpValue, targetLerp, Time.deltaTime * transitionSpeed);
        
        // Интерполяция цвета
        spriteRenderer.color = Color.Lerp(originalColor, madnessColorTint, currentLerpValue);
        
        // Интерполяция масштаба
        transform.localScale = Vector3.Lerp(
            originalScale, 
            originalScale * madnessScaleMultiplier, 
            currentLerpValue
        );
        
        // Для спрайта — переключение при 50% перехода
        if (currentLerpValue > 0.5f && madnessSprite != null)
        {
            spriteRenderer.sprite = madnessSprite;
        }
        else if (currentLerpValue <= 0.5f)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Принудительно обновить спрайт (например, после смены данных)
    /// </summary>
    public void ForceUpdate()
    {
        if (MadnessManager.Instance != null)
        {
            UpdateSprite(MadnessManager.Instance.CurrentSanity);
        }
    }
    
    /// <summary>
    /// Установить новые данные спрайта во время игры
    /// </summary>
    public void SetSpriteData(MadnessSpriteData newData)
    {
        spriteData = newData;
        if (newData != null)
        {
            normalSprite = newData.normalSprite;
            madnessSprite = newData.madnessSprite;
            madnessThreshold = newData.madnessThreshold;
        }
        ForceUpdate();
    }
    #endregion
    
    #region Editor
    private void OnValidate()
    {
        // Обновление в редакторе при изменении полей
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion
}