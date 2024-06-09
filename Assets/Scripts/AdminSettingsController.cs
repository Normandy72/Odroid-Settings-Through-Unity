using System;
using UnityEngine;
using UnityEngine.UI;

public class AdminSettingsController : SettingsController
{
    [Space(10)]
    [SerializeField] private Button _openAndroidSettingsButton;
    [SerializeField] private Button _shutdownDeviceButton;

    public static event Action<string, ButtonFunction> OnButtonPressed;        // <warning message, button function>

    public override void Awake()
    {
        base.Awake();

        PanelManager.HidePanel(_warningPanel);

        _openAndroidSettingsButton.onClick.AddListener(() => {
            SettingsManager.OpenSettings();
        });

        _shutdownDeviceButton.onClick.AddListener(() => {
            PanelManager.ShowPanel(_warningPanel);
            OnButtonPressed? .Invoke(SHUTDOWN_WARNING_MESSAGE, SettingsManager.ShutdownDevice);
        });          
    }
}