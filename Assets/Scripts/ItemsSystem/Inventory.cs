using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]private GameObject inventoryPanel;
    [SerializeField] private GameObject ui;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
