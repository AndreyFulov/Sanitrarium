using UnityEngine;

/// <summary>
/// Данные для системы безумия. Создаётся через правый клик в Project окне
/// Хранит спрайты для нормального состояния и состояния безумия
/// </summary>
[CreateAssetMenu(fileName = "NewMadnessData", menuName = "Sanatorium/Madness Sprite Data")]
public class MadnessSpriteData : ScriptableObject
{
    [Header("Спрайты")]
    [Tooltip("Спрайт в нормальном состоянии рассудка")]
    public Sprite normalSprite;
    
    [Tooltip("Спрайт в состоянии безумия")]
    public Sprite madnessSprite;
    
    [Header("Настройки переключения")]
    [Tooltip("Порог рассудка для переключения (0-100)")]
    [Range(0, 100)]
    public int madnessThreshold = 50;
    
    [Tooltip("Использовать плавный переход вместо мгновенного")]
    public bool useSmoothTransition = false;
    
    [Header("Визуальные эффекты")]
    [Tooltip("Цветовая тонировка в режиме безумия")]
    public Color madnessColorTint = new Color(1f, 0.7f, 0.7f);
    
    [Tooltip("Множитель масштаба в режиме безумия")]
    public float madnessScaleMultiplier = 1.0f;
}