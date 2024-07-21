//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static Config;

/* This class take as an input the Chat-GPT response and codifies it in an AudioSource (this can be found also in the github of Azure services)
 playing the audio on the correct audiosource set on the active avatar.*/
public class TextToSpeech : MonoBehaviour
{
    // Hook up the four properties below with a Text, InputField, Button and AudioSource object in your UI.
    public AudioSource audioSource;
    [HideInInspector] public string voice;

    private const int SampleRate = 24000;
    private bool audioSourceNeedStop;
    private string message;
    private string chatGptMessage;
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    private VoiceRecognition voiceRecon;
    public string text;

    [ContextMenu("Test")]
    public void test()
    {
        TTS(text);
    }
    public void TTS(string message)
    {
        voiceRecon = GetComponent<VoiceRecognition>();
        string Region = EnvUtility.AskForKey(APIType.Region);
        string AzureKey = EnvUtility.AskForKey(APIType.Azure);
        var config = new Config(AzureKey, Region, voice);
        speechConfig = config.GetSpeechConfig();

        /*
        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);
        speechConfig.SpeechSynthesisVoiceName = "en-GB-ElliotNeural";
        speechConfig.SetProperty("style", "");
        */

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };
        // We can't await the task without blocking the main Unity thread, so we'll call a coroutine to
        // monitor completion and play audio when it's ready.
        var speakTask = synthesizer.StartSpeakingTextAsync(message);
        chatGptMessage = message;
        StartCoroutine(SpeakRoutine(speakTask));
    }
    
    IEnumerator SpeakRoutine(Task<SpeechSynthesisResult> speakTask)
    {
        

        var startTime = DateTime.Now;

        while (!speakTask.IsCompleted)
        {
            yield return null;
        }

        var result = speakTask.Result;
        {
            if (result.Reason == ResultReason.SynthesizingAudioStarted)
            {
                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var audioDataStream = AudioDataStream.FromResult(result);
                while (!audioDataStream.CanReadData(4092 * 2)) // audio clip requires 4096 samples before it's ready to play
                {
                    yield return null;
                }
                var isFirstAudioChunk = true;

                var audioClip = AudioClip.Create(
                    "Speech",
                    SampleRate * 600, // Can speak 10mins audio as maximum
                    1,
                    SampleRate,
                    true,
                    (float[] audioChunk) =>
                    {
                        var chunkSize = audioChunk.Length;
                        var audioChunkBytes = new byte[chunkSize * 2];
                        var readBytes = audioDataStream.ReadData(audioChunkBytes);
                        if (isFirstAudioChunk && readBytes > 0)
                        {
                            var endTime = DateTime.Now;
                            var latency = endTime.Subtract(startTime).TotalMilliseconds;
                            message = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                            isFirstAudioChunk = false;
                        }

                        for (int i = 0; i < chunkSize; ++i)
                        {
                            if (i < readBytes / 2)
                            {
                                audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                            }
                            else
                            {
                                audioChunk[i] = 0.0f;
                            }
                        }

                        if (readBytes == 0)
                        {
                            Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                            audioSourceNeedStop = true;
                        }
                    });
                audioSource.clip = audioClip;
                audioSource.Play();
    
            }
        }
    }

    void Start()
    {
        //speakButton.onClick.AddListener(ButtonClick);
       
    }

    

    void Update()
    {
        
        if (audioSourceNeedStop)
        {
            audioSource.Stop();
            audioSourceNeedStop = false;
            EventManager.instance.UpdateStatus(0, 255, 0); //Green
            voiceRecon.recordingAvailable = true;
        }
        
        
    }



    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
   
}
// </code>
