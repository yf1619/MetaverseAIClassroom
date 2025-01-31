using System.Linq;
using Vipenti.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Logger : Singleton<Logger>
{
    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    private bool enableDebug = true;

    private int currentLines = 0;

    private ScrollRect scrollRect;

    void Awake()
    {
        if (debugAreaText == null)
        {
            debugAreaText = GetComponent<TextMeshProUGUI>();
        }
        debugAreaText.text = string.Empty;

        scrollRect = this.GetComponentInParent<ScrollRect>();
    }

    void OnEnable()
    {        
        debugAreaText.enabled = true;
        enabled = true;

        if (enabled)
        {
            debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled</color>\n";
        }
    }

    public void LogInfo(string message)
    {
        ClearLines();
        currentLines++;
        debugAreaText.text += $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss")}</color> <color=\"white\">{message}</color>\n";
        Layout();
    }

    public void LogError(string message)
    {
        ClearLines();
        currentLines++;
        debugAreaText.text += $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss")} {message}</color>\n";
        Layout();
    }

    public void LogWarning(string message)
    {
        ClearLines();
        currentLines++;
        debugAreaText.text += $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss")} {message}</color>\n";
        Layout();
    }

    private void ClearLines()
    {
        // if (debugAreaText.text.Length >= 1000 || currentLines >= 12)
        // {
        //     debugAreaText.text = "";
        //     currentLines = 0;
        // }
    }
    private void Layout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        scrollRect.verticalNormalizedPosition = 0f;
    }
}