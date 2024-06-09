using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSettingsController : SettingsController
{
    private const int MIN_BRIGHTNESS_VALUE = -512;      // according to Odroid N2 documentation
    private const int MAX_BRIGHTNESS_VALUE = 512;       // according to Odroid N2 documentation

    [Space(10)]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _restartAppButton;
    [SerializeField] private Button _restartDeviceButton;
    [SerializeField] private Button _powerOffButton;

    [Space(10)]
    [Tooltip("[0] - brightness, [1] - volume")]
    [SerializeField] private List<SettingsSlider> _sliders;

    public static event Action<string, ButtonFunction> OnButtonPressed;        // <warning message, button function>
        
    public override void Awake()
    {
        base.Awake();

        PanelManager.HidePanels(new List<GameObject> {gameObject, _warningPanel});

        _settingsButton.onClick.AddListener(() => {
            PanelManager.ShowPanel(gameObject);
        });

        _restartAppButton.onClick.AddListener(() => {
            PanelManager.ShowPanel(_warningPanel);
            OnButtonPressed? .Invoke(REBOOT_APP_WARNING_MESSAGE, SettingsManager.RestartApp);
        });

        _restartDeviceButton.onClick.AddListener(() => {
            PanelManager.ShowPanel(_warningPanel);
            OnButtonPressed? .Invoke(REBOOT_DEVICE_WARNING_MESSAGE, SettingsManager.RestartDevice);
        });

        _powerOffButton.onClick.AddListener(() => {
            PanelManager.ShowPanel(_warningPanel);
            OnButtonPressed? .Invoke(SHUTDOWN_WARNING_MESSAGE, SettingsManager.ShutdownDevice);
        });

        _sliders[0].Slider.value = _odroidType == OdroidType.C4 ? SettingsManager.GetScreenBrightnessFloat() : Mathf.InverseLerp(MIN_BRIGHTNESS_VALUE, MAX_BRIGHTNESS_VALUE, SettingsManager.GetScreenBrightnessInt());
        _sliders[1].Slider.value = SettingsManager.GetDeviceVolume();   

        foreach(SettingsSlider slider in _sliders)
        {
            slider.OffButton.onClick.AddListener(() => {
                slider.SetSliderMinValue();
            });

            slider.OnButton.onClick.AddListener(() => {
                slider.SetSliderMaxValue();
            });

            slider.SliderValueText.text = slider.SetSliderValueText();
        }

        _sliders[0].Slider.onValueChanged.AddListener((newValue) => {
                
            if(_odroidType == OdroidType.C4)
            {
                SettingsManager.SetScreenBrightness(newValue);
                _sliders[0].SliderValueText.text = _sliders[0].SetSliderValueText();
                return;
            }
                
            // Odroid N2
            SettingsManager.SetScreenBrightness((int)Mathf.Lerp(MIN_BRIGHTNESS_VALUE, MAX_BRIGHTNESS_VALUE, _sliders[0].Slider.value));
            _sliders[0].SliderValueText.text = _sliders[0].SetSliderValueText();    
        });

        _sliders[1].Slider.onValueChanged.AddListener((newValue) => {
            SettingsManager.SetDeviceVolume(newValue);
            _sliders[1].SliderValueText.text = _sliders[1].SetSliderValueText(); 
        });
    } 
}