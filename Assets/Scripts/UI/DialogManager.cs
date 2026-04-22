using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public GameObject optionsPanel;    
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI dialogText;
    public GameObject optionButtonPrefab;

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    public void CreateDialog(int dialogIndex, string title, string text, string[] options)
    {
        ClearOptions();
        
        titleText.text = title;
        dialogText.text = text;
        dialogPanel.SetActive(true);
        
        for (int i = 0; i < options.Length; i++)
        {
            int index = i;
            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsPanel.transform);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = options[i];
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(dialogIndex, index));
        }
    }
    
    private void OnOptionSelected(int dialogIndex, int optionIndex)
    {
        Debug.Log($"Выбран вариант {optionIndex}");
        CloseDialog();
    }
    
    public void CloseDialog()
    {
        ClearOptions();
        dialogPanel.SetActive(false);
    }
    
    private void ClearOptions()
    {
        foreach (Transform child in optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
