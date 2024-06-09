using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{
    public static void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public static void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public static void ShowPanels(List<GameObject> panels)
    {
        foreach(GameObject panel in panels)
        {
            ShowPanel(panel);
        }
    }

    public static void HidePanels(List<GameObject> panels)
    {
        foreach(GameObject panel in panels)
        {
            HidePanel(panel);
        }
    }
}