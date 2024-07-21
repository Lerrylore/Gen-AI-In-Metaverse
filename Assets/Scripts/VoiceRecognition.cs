//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>

using System;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using UnityEngine.Events;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Collections.Generic;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif
public class ActionPrompt
{
    public String action;
    public float lifetime;

    public ActionPrompt(String action)
    {
        this.action = action;
        this.lifetime = 0;
    }

}
/*This class waits for an input from the user to start a conversation, 
 * ONLY if the user is in the proximity of an avatar (i.e. ActiveAvatar is not null)
 * When the conversation began the user can speak as much as he wants but when there is detected at least 1 second of silence
    the avatar stops listening and it sends the text to the Chat-GPT component, attaching the action prompts in the buffer*/
public class VoiceRecognition : MonoBehaviour
{
    [SerializeField] private InputActionReference recordInputAction;
    public float maxLifetime;
    
    private List<ActionPrompt> actions = new List<ActionPrompt>();
    [System.Serializable]
    public class StringResponse : UnityEvent<string> { }
    // Hook up the two properties below with a Text and Button object in your UI.
    [HideInInspector] public StringResponse endRecognition;
    [HideInInspector] public bool recordingAvailable = true;
    public ChatGptManager chatGptManager;
    
    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;

    private bool micPermissionGranted = false;

    [SerializeField] private bool debugActions = false;
    public bool isActive = false;

#if PLATFORM_ANDROID || PLATFORM_IOS
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    public async void SpeechToText(InputAction.CallbackContext ctx)
    {
        // Active bool for proximity chat
        if (!isActive) return;

        if (recordingAvailable)
        {
            recordingAvailable = false;
            string Region = EnvUtility.AskForKey(APIType.Region);
            string AzureKey = EnvUtility.AskForKey(APIType.Azure);
            var configurator = new Config(AzureKey, Region, "en - GB - EthanNeural");
            var speech = configurator.GetSpeechConfig();
            // Make sure to dispose the recognizer after use!

            Debug.Log("Now listening...");
            EventManager.instance.UpdateStatus(255,255,0); //Yellow
            using (var recognizer = new SpeechRecognizer(speech))
            {
                lock (threadLocker)
                {
                    waitingForReco = true;
                }

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result.
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.

                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(true);

                // Checks result.
                string newMessage = string.Empty;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    newMessage = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                   
                    newMessage = "[user is silent]";
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

                if (newMessage == string.Empty)
                {
                    message = "Please say: 'There was an error in the speech recognition'";
                }

            }

            foreach (var action in actions)
            {
                message += " ";
                message += action.action;
            }
            
            SaveManager.instance.AddUserMessage(message);

            chatGptManager.AskChatGPT(message);
        }
    }

    void Start()
    {
        
            // Continue with normal initialization, Text and Button objects are present.
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            message = "Waiting for mic permission";
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#elif PLATFORM_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
#else
            micPermissionGranted = true;
        
#endif


    }

    void Update()
    {
        Debug.Log(isActive);
        if(debugActions) Debug.Log(actions.Count);

#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#elif PLATFORM_IOS
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#endif


        UpdateActionsLifetime();

    }
    private void UpdateActionsLifetime()
    {
        foreach (var action in actions)
        {
            action.lifetime += Time.deltaTime;
           
        }
        DeleteActions();
    }

    private void DeleteActions()
    {
      actions.RemoveAll(action => action.lifetime > maxLifetime);  
    }

    public void AddAction(String action)
    {
        bool isNew = true;
        ActionPrompt resetAction = null;
        foreach (ActionPrompt actionString in actions)
        {
            if (actionString.action.Equals(action))
            {
                isNew = false;
                resetAction = actionString;
            }
        }
        if (isNew)
        {
            actions.Add(new ActionPrompt(action));
            Debug.Log("Added");
        }
        else
        {
            resetAction.lifetime = 0;
            Debug.Log("Reset");
        }
        
        
    }


        private void OnEnable()
    {
        SceneLoader.instance.sceneChanged = true;
        recordInputAction.action.started += SpeechToText;
    }
    
    private void OnDisable()
    {
        recordInputAction.action.started -= SpeechToText;
    }
}

