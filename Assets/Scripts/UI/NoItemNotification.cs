using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NoItemNotification : MonoBehaviour
{
    private TMP_Text text;
    void OnDisable()
    {
        Inventory.Instance.InventoryMessage -= Show;
    }
    void OnEnable()
    {
        Inventory.Instance.InventoryMessage += Show;
    }
    public void Show(string msg)
    {
        text.text = msg;
        gameObject.SetActive(true);
        StartCoroutine(ShowNotification());
    }
    private void Start() {
        gameObject.SetActive(false);
        text = GetComponent<TMP_Text>();
    }
    IEnumerator ShowNotification()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
