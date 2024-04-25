using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Microsoft.CognitiveServices.Speech;

public class TextToSpeech : MonoBehaviour
{
    private const string subscriptionKey = "7063a4aac24a426390acbcf9041cad1e";
    private const string region = "eastus"; // e.g., "eastus"
    private const string textToSpeechUrl = "https://" + region + ".tts.speech.microsoft.com/cognitiveservices/v1";

    public string textToSpeak;
    public AudioSource audioSource;

    string access_token;

    public void StartTextToSpeech()
    {
        StartCoroutine("SynthesizeSpeech");
    }

    private IEnumerator SynthesizeSpeech()
    {
        yield return StartCoroutine("GetAccessToken");
        string xmlBody = "<speak version='1.0' xml:lang='en-US'>" +
                         "<voice xml:lang='en-US' xml:gender='Male' name='en-US-ChristopherNeural'>" +
                         textToSpeak + "</voice></speak>";

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(textToSpeechUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(xmlBody));
            webRequest.downloadHandler = new DownloadHandlerAudioClip("", AudioType.WAV);
            webRequest.SetRequestHeader("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
            webRequest.SetRequestHeader("Authorization", "Bearer " + access_token);
            webRequest.SetRequestHeader("Content-Type", "application/ssml+xml");
            webRequest.SetRequestHeader("User-Agent", "AVP TTS Test");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success");
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Text to Speech Error: " + webRequest.error);
            }
        }
    }

    private IEnumerator GetAccessToken()
    {
        Debug.Log("GetAccessToken");
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://eastus.api.cognitive.microsoft.com/sts/v1.0/issueToken", ""))
        {
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            webRequest.SetRequestHeader("Content-Length", "0");
            webRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

            yield return webRequest.SendWebRequest();



            if (webRequest.result == UnityWebRequest.Result.Success)
            {


                access_token = webRequest.downloadHandler.text;
                Debug.Log("access_token " + access_token);
                yield return webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Failed to obtain access token: " + webRequest.error);
                yield return null;
            }
        }

    }
}
