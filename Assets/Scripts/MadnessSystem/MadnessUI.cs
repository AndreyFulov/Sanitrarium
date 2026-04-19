using UnityEngine.UI;
using UnityEngine;

public class MadnessUI : MonoBehaviour
{
    private Image madnessCircle;
    private float _currentFill = 0f;
    private float _targetFill = 0f;
    private float transitionTime = 0f;
    public void ChangeMindUI(InfoUI info)
    {
        _targetFill = Mathf.Clamp(MadnessManager.instance.madness, info.minMadness,info.maxMadness) / 100f;
        transitionTime = info.transition;
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
