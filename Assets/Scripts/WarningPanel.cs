using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SettingsController;

public class WarningPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _cancelButton;

    private ButtonFunction _function;

    private void Awake()
    {
        CustomSettingsController.OnButtonPressed += (message, function) => {
            _messageText.text = message;
            _function = function;
        };

        AdminSettingsController.OnButtonPressed += (message, function) => {
            _messageText.text = message;
            _function = function;
        };

        _yesButton.onClick.AddListener(() => {
            _function();
        });

        _cancelButton.onClick.AddListener(() => {
            PanelManager.HidePanel(gameObject);
            _function = null;
        });
    }
}