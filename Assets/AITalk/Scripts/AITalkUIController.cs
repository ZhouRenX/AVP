using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AITalkUIController : MonoBehaviour
{
    public Text myContent;
    public Text _GPTContent;
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMyContent(string msg)
    {
        myContent.text = string.Format("You: {0}", msg);
    }
    public void SetAIContent(string msg)
    {
        _GPTContent.text = string.Format("GPT: {0}", msg);
    }
}
