using UnityEngine;

public class MadnessManager : MonoBehaviour
{
    // Приватное поле для хранения реального значения
    private float _madness;

    // Публичное свойство с валидацией
    public float madness
    {
        get { return _madness; }
        set
        {
            // Клэмпим значение в допустимых границах
            _madness = Mathf.Clamp(value, minMadness, maxMadness);
        }
    }

    [Header("Настройка")]
    public MadnessUI ui;
    [SerializeField] private float UItransitionTime = 0.5f;
    [SerializeField] private float maxMadness = 100f;
    [SerializeField] private float minMadness = 0f;

    public static MadnessManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMadness(float v)
    {
        madness += v; // теперь работает корректно
        ui?.ChangeMindUI(GetInfoUI());
    }

    public InfoUI GetInfoUI()
    {
        return new InfoUI 
        { 
            transition = UItransitionTime, 
            maxMadness = maxMadness, 
            minMadness = minMadness 
        };
    }
}

public class InfoUI
{
    public float transition;
    public float maxMadness;
    public float minMadness;
}