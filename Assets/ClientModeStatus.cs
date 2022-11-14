using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientModeStatus : MonoBehaviour
{
    public static ClientModeStatus Singleton { get; private set; }

    TMPro.TextMeshProUGUI ClientModeText;

    private void Awake()
    {
        Singleton = this;

        ClientModeText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void SetText(string text, bool isStatic = false) => ClientModeText.text = text;
    public static void SetText(string text)
    {
        if (Singleton != null) Singleton.SetText(text);
        else Debug.LogWarning("No singleton found!");
    }
}
