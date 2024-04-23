using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechRecognizer : MonoBehaviour
{
    [Header("Azure Cognitive Services Speech Settings")]
    public string subscriptionKey = "YOUR_SUBSCRIPTION_KEY_HERE";
    public string serviceRegion = "YOUR_SERVICE_REGION_HERE";
    public string language = "en-US";
    public string mode = "conversation";

    [Header("Unity Microphone Settings")]
    public int microphoneIndex = 0;
    public int recordingLength = 5;

    private AudioClip recording;
    private AudioSource audioSource;

    private void Start()
    {
        // Initialize the audio source
        audioSource = GetComponent<AudioSource>();
    }

    public void StartRecord()
    {
        recording = Microphone.Start(null, false, recordingLength, 44100);
    }

    public void StopRecord()
    {
        // Stop recording audio
        Microphone.End(null);

        // Convert audio clip to WAV format
        byte[] audioData = WavUtility.FromAudioClip(recording);

        // Send audio data to speech recognition API
        StartCoroutine(SendAudioData(audioData));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Start recording audio

        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {

        }
    }

    private IEnumerator SendAudioData(byte[] audioData)
    {
        // Construct the request URL
        string url = "https://" + serviceRegion + ".stt.speech.microsoft.com/speech/recognition/" + mode + "/cognitiveservices/v1?language=" + language;

        // Create the request object
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "application/octet-stream");
        request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.SetRequestHeader("Content-Type", "audio/wav; codec=audio/pcm; samplerate=44100");

        // Attach the audio data to the request
        request.uploadHandler = new UploadHandlerRaw(audioData);
        request.uploadHandler.contentType = "application/octet-stream";

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Speech recognition request failed: " + request.error);
            yield break;
        }

        // Parse the response JSON and extract the recognition result
        string json = request.downloadHandler.text;
        SpeechRecognitionResult result = JsonUtility.FromJson<SpeechRecognitionResult>(json);
        string recognizedText = result.DisplayText;

        // Display the recognized text in the console
        Debug.Log("Recognized text: " + recognizedText);
    }
}

[System.Serializable]
public class SpeechRecognitionResult
{
    public string RecognitionStatus;
    public string DisplayText;
}
