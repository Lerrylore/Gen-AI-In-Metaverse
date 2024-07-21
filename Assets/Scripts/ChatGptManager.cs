using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Azure;
using UnityEngine;
using UnityEngine.Events;
using Azure.AI.OpenAI;
using DefaultNamespace;
using UnityEngine.InputSystem;
/*This class handles everything gpt related.
First it takes as an input the text from the Voice recognition component (STT)
and then it attach all the actions that are in the buffer as a string of text, then it will all be sent to GPT APIs and the response
will be given to the Text-To-Speech component in order to be played*/
public class ChatGptManager : MonoBehaviour
{
    public GameObject VoiceManager;
    private VoiceRecognition voiceRecognition;
    public SystemMessage SystemMessage;
    private List<ChatMessage> messages = new();
    private OpenAIClient openAI;
    private AnimationManager animationManager;
    private String completeSystemPrompt;
    [System.Serializable]
    public class StringResponse : UnityEvent<string> { }
    [HideInInspector]public StringResponse gptResponse;
    private bool isInitiated = false;

    public void StartConversation()
    {
        if (!isInitiated)
        {
            ChatMessage systemMessage = new ChatMessage(ChatRole.System, SystemMessage.GetSystemMessage());

           
            Debug.Log(systemMessage.Content);
            messages.Add(systemMessage);
            isInitiated = true;
        }
    }
    public async void AskChatGPT(string newText)
    {
        Debug.Log("[USER] " + newText);
        
        ChatMessage newMessage = new ChatMessage(ChatRole.User, newText);
        messages.Add(newMessage);

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);

        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        try
        {
            EventManager.instance.UpdateStatus(255,0,0); //RED
            Response<ChatCompletions> response = await openAI.GetChatCompletionsAsync(chatCompletionsOptions, cancellationTokenSource.Token);

            ChatCompletions completion = response.Value;

            if (completion.Choices != null && completion.Choices.Count > 0)
            {
                var responseMessage = completion.Choices.First().Message;
                messages.Add(responseMessage);
                
                cancellationTokenSource.Dispose(); // no need to keep tracking the time to respond
                
                SaveManager.instance.AddAvatarMessage(name, responseMessage.Content);
            
                gptResponse.Invoke(RemoveCommandStrings(responseMessage.Content));
                animationManager.FetchAnimations(responseMessage.Content);
                
                Debug.Log("[GPT] " + responseMessage.Content);
            }
        } 
        catch (OperationCanceledException)
        {
            messages.RemoveAt(messages.Count - 1);
            ResetChatLength();
            EventManager.instance.UpdateStatus(255,0,255); //Purple
            gptResponse.Invoke(RemoveCommandStrings("I'm sorry. I didn't catch that. Please say that again."));
            Debug.Log("[GPTManager] Token expired. Asked GPT to keep shorter answers.");
            voiceRecognition.recordingAvailable = true;
        }
        catch (Exception)
        {
            messages.RemoveAt(messages.Count - 1);
            EventManager.instance.UpdateStatus(75, 0, 130); //Indigo
            gptResponse.Invoke(RemoveCommandStrings("I'm sorry. I didn't catch that. Please say that again."));
            Debug.Log("[GPTManager] Generic error occurred.");
            voiceRecognition.recordingAvailable = true;
        }
        finally
        {
            cancellationTokenSource.Dispose();
            
        }
    }
    // Start is called before the first frame update
    public void RecognitionEnd(string speechToText)
    {
        AskChatGPT(speechToText);
    }
    
    private string RemoveCommandStrings(string input)
    {
        string patternCommand = @"\[.*?\]";
        string patternAction = @"\*.*?\*";
        string result = Regex.Replace(input, patternCommand, "");
        result = Regex.Replace(result, patternAction, "");
        return result;
    }
    
    private void ResetChatLength()
    {
        // seems like the conversation doesn't reset completely after all
        var resetMessage = new ChatMessage(ChatRole.System, SystemMessage.GetSystemMessage());
        messages.Add(resetMessage);

        var chatCompletionsOptions = new ChatCompletionsOptions("gpt-3.5-turbo", messages);
        openAI.GetChatCompletions(chatCompletionsOptions);

        Debug.Log("[SYSTEM] Asked GPT to reset to an acceptable length.");
    }
    
    void Start()
    {
        string key = EnvUtility.AskForKey(APIType.OpenAI);
        if (key == null)
        {
            Debug.Log("ERROR: KeyType not existing");
        }
        openAI = new OpenAIClient(key);
        TextToSpeech method = VoiceManager.GetComponent<TextToSpeech>();
        voiceRecognition = VoiceManager.GetComponent<VoiceRecognition>();
        animationManager = GetComponent<AnimationManager>();
        gptResponse.AddListener(method.TTS);
        voiceRecognition.endRecognition.AddListener(RecognitionEnd);
        
        
    }

}
