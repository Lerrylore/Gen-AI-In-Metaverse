using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private float volume = 0.5f;
    private int selectedScene = 1;
    [HideInInspector] public bool sceneChanged = false;
    private List<string> interests = new List<string>();
    Scene newScene;
    public void SceneLoad()
    {
        // Check for selected interest
        if (interests.Count == 0) return;
        
        // Save conversation
        SaveManager.instance.CreateAndSaveFile();
        
        SceneManager.LoadSceneAsync(selectedScene);
        StartCoroutine(WaitForChangeScene());
    }
    IEnumerator WaitForChangeScene()
    {
        while(!sceneChanged)
        {
            yield return new WaitForEndOfFrame(); 
        }
        GameObject[] avatar = GameObject.FindGameObjectsWithTag("Avatar");
        
        // Deactivate all avatars
        foreach(GameObject go in avatar)
        {
            Debug.Log("Deactivated: " + go.name);
            go.SetActive(false);
        }

        // Activate selected avatars
        foreach (string subject in interests)
        {
            foreach(GameObject go in avatar)
            {
                if (go.name.Contains(subject))
                {
                    Debug.Log("Deactivated: " + go.name);
                    // checks if avatar contains the interest in his name
                    go.SetActive(true);
                }
            }
        }
        
        

        interests.Clear();
        sceneChanged = false;
        selectedScene = 1;
    }
    public void SetScene(int scene)
    {
        selectedScene = scene;
    }
    
    public void SetVolume(float value)
    {
        volume = value;
    }

    public void SetActiveInterests(string value)
    {
        if (interests.Contains(value))
        {
            interests.Remove(value);
            Debug.Log("Removed" + value);
        }
        else
        {
            interests.Add(value);
            Debug.Log("Added" + value);
        }
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetVolume()
    {
        return volume;
    }
   
}
