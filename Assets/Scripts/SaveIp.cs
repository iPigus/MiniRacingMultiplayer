using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveIp : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI ipText;
    public void Start() => ipText.text = PlayerPrefs.GetString("ip");
    public void IpChanged(string ip) => PlayerPrefs.SetString("ip", ip);
}
