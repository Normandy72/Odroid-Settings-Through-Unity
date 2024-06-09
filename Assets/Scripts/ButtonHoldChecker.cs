using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoldChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float MAX_HOLD_TIME = 10f;

    [SerializeField] private GameObject _panelToShow;
    private Coroutine _holdCoroutine;

    private void Awake()
    {
        PanelManager.HidePanel(_panelToShow);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _holdCoroutine = StartCoroutine(HoldCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
        }
    }

    private IEnumerator HoldCoroutine()
    {
        yield return new WaitForSeconds(MAX_HOLD_TIME);

        PanelManager.ShowPanel(_panelToShow);
    }
}