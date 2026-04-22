using UnityEngine;

public class DocumentObject : ClickableObject
{
    public string documentTitle;
    public string documentText;
    
    private DocumentManager documentManager;

    public override bool OnClick()
    {
        if(!base.OnClick()) return false;
        
        if (documentManager == null)
            documentManager = FindObjectOfType<DocumentManager>();
        
        if (documentManager != null)
            documentManager.ShowDocument(documentTitle, documentText);
        else
            Debug.LogError("DocumentManager не найден на сцене!");
        
        return true;
    }

    public void OnClickVoid()
    {
        OnClick();
    }
}
