using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class PostData
{
    public string model;
    public List<Message> messages;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
    public Message() { }
    public Message(string _role, string _content)
    {
        this.role = _role;
        this.content = _content;
    }
}

[Serializable]
public class ReceiveData
{
    public string id;
    public string created;
    public string model;
    public List<Choice> choices;

    [Serializable]
    public class Choice
    {
        public string index;
        public Message message;
        public string finish_reason;
    }
}


public class GetOpenAI : MonoBehaviour
{
    [SerializeField]
    private string m_OpenAI_Key = "";
    [SerializeField]
    private string m_API_Url = "https://api.openai.com/v1/chat/completions";
    [SerializeField]
    private string m_GPT_Modle = "gpt-3.5-turbo";
    [SerializeField]
    private string prompt;

    [SerializeField]
    private List<Message> m_MessageList;
    // Start is called before the first frame update
    void Start()
    {
        m_MessageList.Add(new Message("system", prompt));
    }

    // Update is called once per frame
    public IEnumerator GetPostData(string _postWord, Action<string> _callback)
    {
        m_MessageList.Add(new Message("user", _postWord));

        var request = new UnityWebRequest(m_API_Url, "POST");
        PostData _postData = new PostData
        {
            model = m_GPT_Modle,
            messages = m_MessageList
        };

        string _jsonText = JsonUtility.ToJson(_postData);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", string.Format("Bearer {0)", m_OpenAI_Key));
        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {
            string _msg = request.downloadHandler.text;
            ReceiveData _receiveData = JsonUtility.FromJson<ReceiveData>(_msg);
            if (_receiveData != null && _receiveData.choices.Count > 0)
            {
                string _receiveMsg = _receiveData.choices[0].message.content;

                m_MessageList.Add(new Message("assistant", _receiveMsg));
                _callback(_receiveMsg);
            }
        }
    }
}
