using UnityEngine.UI;
using UnityEngine;

public class MadnessUI : MonoBehaviour
{
    private Image madnessCircle;
    private float _currentFill = 0f;
    private float _targetFill = 0f;
    public float transitionTime = 0f;
    public void ChangeMindUI()
    {
        _targetFill = Mathf.Clamp(MadnessManager.Instance.currentSanity, MadnessManager.Instance.minSanity,MadnessManager.Instance.maxSanity) / 100f;
    }
    void Update()
    {
        if(Mathf.Approximately(_currentFill,_targetFill)) return;

        float distance = Mathf.Abs(_targetFill - _currentFill);
        float step = distance / transitionTime * Time.deltaTime;

        _currentFill = Mathf.MoveTowards(_currentFill,_targetFill,step);
        madnessCircle.fillAmount = _currentFill;
    }
    void Start()
    {
        madnessCircle = GetComponent<Image>();
    }
}
