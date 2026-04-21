using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject ui;
    public static Inventory Instance { get; private set; }

    public void SwitchInventoryPanel()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        ui.SetActive(!inventoryPanel.activeSelf);
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
        ui.SetActive(true);
    }

    [Header("Настройки сетки")]
    public int gridWidth = 6;
    public int gridHeight = 4;

    [Header("🧪 Тестовые данные (для редактора)")]
    [Tooltip("Перетащите сюда любой ItemSO для кнопки '➕ Добавить тестовый предмет'")]
    public ItemSO testItem;

    public struct SlotData
    {
        public ItemSO item;
        public int quantity;
        public bool IsEmpty => item == null;
    }

    private SlotData[] _slots;

    // События для внешних систем и UI
    public event Action<int, SlotData> OnSlotChanged;
    public event Action OnInventoryRefresh;
    public event Action<string, ItemSO> OnItemAdded;
    public event Action<string> OnItemRemoved;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        int totalSlots = gridWidth * gridHeight;
        _slots = new SlotData[totalSlots];
        for (int i = 0; i < totalSlots; i++)
            _slots[i] = new SlotData();

        OnInventoryRefresh?.Invoke();
    }

    // 🟢 PUBLIC API: Добавить предмет в первый свободный слот
    public bool AddItem(ItemSO item, int quantity = 1)
    {
        if (item == null) return false;
        int slotIndex = FindFirstEmptySlot();
        if (slotIndex == -1)
        {
            Debug.LogWarning("[Inventory] Нет свободных слотов!");
            return false;
        }

        _slots[slotIndex].item = item;
        _slots[slotIndex].quantity = quantity;
        OnSlotChanged?.Invoke(slotIndex, _slots[slotIndex]);
        OnItemAdded?.Invoke(item.itemId, item);
        return true;
    }

    // 🔴 PUBLIC API: Забрать/удалить предмет по слоту
    public bool RemoveItem(int slotIndex, int amount = 1)
    {
        if (!IsValidSlot(slotIndex) || _slots[slotIndex].IsEmpty) return false;

        _slots[slotIndex].quantity -= amount;
        if (_slots[slotIndex].quantity <= 0)
        {
            var removedItem = _slots[slotIndex].item;
            _slots[slotIndex] = new SlotData();
            OnSlotChanged?.Invoke(slotIndex, _slots[slotIndex]);
            OnItemRemoved?.Invoke(removedItem.itemId);
        }
        else
        {
            OnSlotChanged?.Invoke(slotIndex, _slots[slotIndex]);
        }
        return true;
    }

    // 🔍 PUBLIC API: Получить данные слота
    public SlotData GetSlot(int index) => IsValidSlot(index) ? _slots[index] : throw new IndexOutOfRangeException();

    // 🔍 PUBLIC API: Проверить, есть ли место
    public bool HasEmptySlot() => FindFirstEmptySlot() != -1;

    // 🔍 PUBLIC API: Получить ID предмета в руке (если нужно)
    public string GetCurrentHandItem(int slotIndex) => GetSlot(slotIndex).item?.itemId;

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < _slots.Length; i++)
            if (_slots[i].IsEmpty) return i;
        return -1;
    }

    private bool IsValidSlot(int index) => index >= 0 && index < _slots.Length;

    // ─────────────────────────────────────────────────────
    // 🎮 EDITOR BUTTONS (не попадают в билд благодаря #if UNITY_EDITOR)
    // ─────────────────────────────────────────────────────
#if UNITY_EDITOR
    [ContextMenu("➕ Добавить тестовый предмет")]
    private void AddTestItemEditor()
    {
        if (Application.isPlaying)
        {
            if (testItem != null)
            {
                bool success = AddItem(testItem, 1);
                Debug.Log(success 
                    ? $"[Editor] Добавлен предмет: {testItem.displayName}" 
                    : "[Editor] Не удалось добавить предмет (инвентарь полон?)");
            }
            else
            {
                Debug.LogWarning("[Editor] Не назначен testItem в инспекторе!");
            }
        }
        else
        {
            Debug.Log("[Editor] Запустите сцену, чтобы добавить предмет.");
        }
    }

    [ContextMenu("🗑️ Очистить инвентарь")]
    private void ClearInventoryEditor()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (!_slots[i].IsEmpty)
                {
                    _slots[i] = new SlotData();
                    OnSlotChanged?.Invoke(i, _slots[i]);
                }
            }
            OnInventoryRefresh?.Invoke();
            Debug.Log("[Editor] Инвентарь очищен.");
        }
    }

    [ContextMenu("📋 Вывести статус инвентаря")]
    private void DebugInventoryEditor()
    {
        if (Application.isPlaying)
        {
            int filled = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (!_slots[i].IsEmpty) filled++;
            }
            Debug.Log($"[Inventory Debug] Слотов: {_slots.Length}, Заполнено: {filled}, Свободно: {_slots.Length - filled}");
        }
    }
#endif
}