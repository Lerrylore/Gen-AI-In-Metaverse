using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Dropdown SceneDropdown;
    public Slider VolumeSlider;


    private void Start()
    {
        VolumeSlider.value = SceneLoader.instance.GetVolume();
    }

    public void SetDropdownValue()
    {
        // Park index is 1, apartment is 2
        SceneLoader.instance.SetScene(SceneDropdown.value + 1);
    }
    
    public void SetVolumeValue()
    {
        SceneLoader.instance.SetVolume(VolumeSlider.value);
    }
    
    public void SetActiveInterests(string value)
    {
        SceneLoader.instance.SetActiveInterests(value);
    }

    public void LoadMenuScene()
    {
        SceneLoader.instance.LoadMenuScene();
        SaveManager.instance.CreateAndSaveFile();
    }

    public void ReloadScene()
    {
        SceneLoader.instance.ReloadScene();
    }

    public void SceneLoad()
    {
        SceneLoader.instance.SceneLoad();
    }

    public void SaveConversation()
    {
        SaveManager.instance.CreateAndSaveFile();
    }
}
