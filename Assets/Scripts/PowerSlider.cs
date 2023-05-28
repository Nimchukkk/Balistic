using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _forcePercentText;
    
    private Slider _powerSlider;

    private void Awake()
    {
        _powerSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        ForcePercentTextUpdate();
    }

    private void ForcePercentTextUpdate()
    {
        var multiplierForEquating = 100 / _powerSlider.maxValue;
        var forcePercentValue = multiplierForEquating * _powerSlider.value;
        _forcePercentText.text = forcePercentValue.ToString(forcePercentValue < 100 ? "00" : "000");
    }

    public void SetMaxValue(float force)
    {
        _powerSlider.maxValue = force;
    }

    public void SetValue(float force)
    {
        _powerSlider.value = force;
    }
}