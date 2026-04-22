using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DocumentManager : MonoBehaviour
{
    public GameObject ui;
    public GameObject documentPanel; 
    public TextMeshProUGUI documentTitle;
    public TextMeshProUGUI documentText;

    private void Start()
    {
        documentPanel.SetActive(false);
    }

    public void ShowDocument(string title, string text)
    {
        if (ui != null)
            ui.SetActive(false);
        
        if (documentPanel != null)
            documentPanel.SetActive(true);
        
        if (documentTitle != null)
            documentTitle.text = title;
        
        if (documentText != null)
            documentText.text = text;
    }

    public void CloseDocument()
    {
        if (documentPanel != null)
            documentPanel.SetActive(false);
        
        if (ui != null)
            ui.SetActive(true);
    }
}
