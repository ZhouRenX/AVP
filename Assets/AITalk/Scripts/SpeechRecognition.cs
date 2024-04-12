//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;

public class SpeechToText : MonoBehaviour
{
    public Button RecordButton;

    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;

    private bool micPermissionGranted = false;

    // 文本识别语言 中文=zh-CN 日文=ja-JP 英文=en-US
    [SerializeField] private string language = "zh-CN";
    [SerializeField] private string subscriptionKey = "填写你的Azure创建语音服务的密钥";
    [SerializeField] private string region = "填写你的Azure创建语音服务的区域";
    private SpeechConfig speechConfig;


    public string recognizedText = string.Empty;

    void Start()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SpeechRecognitionLanguage = language;


        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            message = "Waiting for mic permission";
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
        else
        {
            micPermissionGranted = true;
            message = "Got mic permission";
        }

        RecordButton.onClick.AddListener(ButtonClick);
    }


    void Update()
    {
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "Got mic permission";
        }

        lock (threadLocker)
        {
            if (RecordButton != null)
            {
                RecordButton.interactable = !waitingForReco && micPermissionGranted;
            }
        }
    }

    public async void ButtonClick()
    {
        // Make sure to dispose the recognizer after use!
        using (var recognizer = new SpeechRecognizer(speechConfig))
        {
            lock (threadLocker)
            {
                recognizedText = string.Empty;
                waitingForReco = true;
            }

            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result.
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query.
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            string newMessage = string.Empty;

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                newMessage = result.Text;

                // 在别的线程更新输入框的值，都会导致输入栏不显示更新后的值，必须点击后才可以显示，所以使用了一个插件实现在主线程更新输入框的值
                recognizedText += newMessage;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                newMessage = "NOMATCH: Speech could not be recognized.";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            }

            lock (threadLocker)
            {
                message = newMessage;
                waitingForReco = false;
            }
        }
    }
}