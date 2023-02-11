using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddTextSample : MonoBehaviour
{
    [SerializeField] private GameObject _obj;

    void Awake()
    {
        var tmp = _obj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Adding dynamic texts works fine too.";
        tmp.color = Color.black;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
