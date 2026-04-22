using UnityEngine;
using UnityEngine.UI;

public class DialogObject : ClickableObject
{
    public int dialogIndex;
    public string dialogTitle;
    public string dialogText;
    public string[] answerOptions;

    private DialogManager dialogManager;

    public override bool OnClick()
    {
        if(!base.OnClick()) return false;
        
        if (dialogManager == null)
            dialogManager = FindObjectOfType<DialogManager>();
        
        if (dialogManager != null)
            dialogManager.CreateDialog(dialogIndex, dialogTitle, dialogText, answerOptions);
        else
            Debug.LogError("DialogManager не найден на сцене!");
        
        return true;
    }
}
