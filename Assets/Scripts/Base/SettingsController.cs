using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingsSlider
{
    public string Name;
    public Slider Slider;
    public TextMeshProUGUI SliderValueText;
    public Button OffButton;
    public Button OnButton;

    private const int SLIDER_MULTIPLIER = 100;

    ///<summary>Sets slider value to minimum.</summary>
    public void SetSliderMinValue()
    {
        Slider.value = Slider.minValue;
    }

    ///<summary>Sets slider value to maximum.</summary>
    public void SetSliderMaxValue()
    {
        Slider.value = Slider.maxValue;
    }

    ///<summary>Gets formatted slider value text.</summary>
    public string SetSliderValueText() => Name + ": " + (Slider.value * SLIDER_MULTIPLIER).ToString("F0") + "%";
}

public enum OdroidType
{
    C4,
    N2
}
    
public class SettingsController : MonoBehaviour
{
    protected const string REBOOT_APP_WARNING_MESSAGE = "Are you sure you want to restart this application?";
    protected const string REBOOT_DEVICE_WARNING_MESSAGE = "Are you sure you want to restart this device?";
    protected const string SHUTDOWN_WARNING_MESSAGE = "Are you sure you want to shutdown this device?";

    [SerializeField] protected OdroidType _odroidType = OdroidType.N2;
    [SerializeField] protected GameObject _warningPanel;
    [SerializeField] protected Button _closePanelButton;

    public delegate void ButtonFunction();

    public virtual void Awake()
    {
        _closePanelButton.onClick.AddListener(() => {
            PanelManager.HidePanels(new List<GameObject> {gameObject, _warningPanel});
        });  
    }
}