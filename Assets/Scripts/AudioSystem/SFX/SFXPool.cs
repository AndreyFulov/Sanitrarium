using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Пул аудиоисточников для эффективного воспроизведения SFX.
/// Предотвращает создание/уничтожение AudioSource во время игры.
/// </summary>
public class SFXPool : MonoBehaviour
{
    [Header("Настройки пула")]
    [Tooltip("Размер пула (сколько AudioSource заранее создать)")]
    public int poolSize = 20;
    
    [Tooltip("Родительский объект для пула")]
    public Transform poolParent;
    
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private List<AudioSource> allSources = new List<AudioSource>();
	
	/// <summary>
	/// Количество доступных AudioSource в пуле
	/// </summary>
	public int AvailableSourcesCount => availableSources.Count;

	/// <summary>
	/// Общее количество источников в пуле
	/// </summary>
	public int TotalSourcesCount => allSources.Count;
    
    private void Awake()
    {
        InitializePool();
    }
    
    private void InitializePool()
    {
        if (poolParent == null)
        {
            poolParent = transform;
        }
        
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = CreateAudioSource();
            availableSources.Enqueue(source);
            allSources.Add(source);
        }
        
        Debug.Log($"[SFXPool] Инициализировано {poolSize} аудиоисточников");
    }
    
    private AudioSource CreateAudioSource()
    {
        GameObject sourceObj = new GameObject("SFX_Source");
        sourceObj.transform.SetParent(poolParent);
        sourceObj.transform.localPosition = Vector3.zero;
        
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D по умолчанию
        
        return source;
    }
    
    /// <summary>
    /// Получить свободный AudioSource из пула
    /// </summary>
    public AudioSource GetSource()
    {
        if (availableSources.Count > 0)
        {
            return availableSources.Dequeue();
        }
        else
        {
            // Если пул пуст, создаём новый источник (расширение)
            Debug.LogWarning("[SFXPool] Пул пуст! Создаём дополнительный AudioSource");
            AudioSource newSource = CreateAudioSource();
            allSources.Add(newSource);
            return newSource;
        }
    }
    
    /// <summary>
    /// Вернуть AudioSource обратно в пул после воспроизведения
    /// </summary>
    public void ReturnSource(AudioSource source)
    {
        if (!allSources.Contains(source))
        {
            Debug.LogWarning("[SFXPool] Попытка вернуть неизвестный источник!");
            return;
        }
        
        source.Stop();
        source.clip = null;
        availableSources.Enqueue(source);
    }
    
    /// <summary>
    /// Очистить весь пул (при смене сцены)
    /// </summary>
    public void ClearAll()
    {
        foreach (var source in allSources)
        {
            source.Stop();
        }
        
        availableSources.Clear();
        foreach (var source in allSources)
        {
            availableSources.Enqueue(source);
        }
    }
}