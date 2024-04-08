using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILogManager : MonoBehaviour
{
    TextMeshPro logUI;
    // Start is called before the first frame update
    void Start()
    {
        logUI = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    public void UpdateLog(string log)
    {
        logUI.text = log;
    }
}
