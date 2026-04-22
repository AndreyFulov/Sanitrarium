using UnityEngine;

[DefaultExecutionOrder(20)]
public class UI_InventoryGrid : MonoBehaviour
{
    [Header("Настройки сетки")]
    public int gridWidth = 5;
    public int gridHeight = 3;
    public UI_Slot slotPrefab;

    private void Awake()
    {
        // Синхронизируем размеры с менеджером
        Inventory.Instance.gridWidth = gridWidth;
        Inventory.Instance.gridHeight = gridHeight;

        // Создаём слоты динамически, если в инспекторе пусто
        if (transform.childCount == 0)
        {
            for (int i = 0; i < gridWidth * gridHeight; i++)
            {
                var slot = Instantiate(slotPrefab, transform);
                slot.slotIndex = i;
            }
        }
        Debug.Log("Инвентарь инициализирован!");
    }

    private void Start()
    {
        // 🟢 Подписываемся на событие полной перерисовки
        Inventory.Instance.OnInventoryRefresh += RefreshAllSlots;
        
        // Принудительно обновляем UI при старте
        RefreshAllSlots();
    }

    private void OnDestroy()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryRefresh -= RefreshAllSlots;
    }

    private void RefreshAllSlots()
    {
        foreach (UI_Slot slot in GetComponentsInChildren<UI_Slot>())
        {
            slot.ForceRefresh();
        }
    }
}