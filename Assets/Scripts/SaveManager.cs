using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif

public class SaveManager : Singleton<SaveManager>
{
    private String folderName = "Conversations";
    private String conversationBuffer;

    // Adds a message from an avatar to the save buffer
    public void AddAvatarMessage(String avatarName, String text)
    {
        string entry = $"- {avatarName}: {text} \n\n";
        conversationBuffer += entry;
    }
    
    // Adds a message from the user to the save buffer
    public void AddUserMessage(String text)
    {
        string entry = $"- User: {text} \n\n";
        conversationBuffer += entry;
    }

    public void AddProximityEnterMessage(String avatarName)
    {
        string entry = $"-- User reaches {avatarName} --\n\n";
        conversationBuffer += entry;
    }
    
    public void AddProximityExitMessage(String avatarName)
    {
        string entry = $"-- User leaves {avatarName} --\n\n";
        conversationBuffer += entry;
    }

    private void OnApplicationQuit()
    {
        CreateAndSaveFile();
    }

    public void CreateAndSaveFile()
    {
        // If folder not exists, create it
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create file based on current date
        string fileName = $"Conversation_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.txt";
        
        string filePath = Path.Combine(folderPath, fileName);
        
        string fileContent = conversationBuffer;

        // Write the files under Android\data\com.DefaultCompany.GenAIInTheMetaverse\files\Conversations
        try
        {
            File.WriteAllText(filePath, fileContent);
            
            // Reset conversation after successful save
            conversationBuffer = String.Empty;
            Debug.Log($"Successfully written file at: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during save: {e.Message}");
        }
    }
}
