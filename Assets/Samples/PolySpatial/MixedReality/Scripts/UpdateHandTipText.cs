using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateHandTipText : MonoBehaviour
{
    TextMeshPro tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    public void UpdateTipsText(string tips)
    {
        tmp.text = tips;
    }
}
