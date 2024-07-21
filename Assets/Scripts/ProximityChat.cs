using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

/*This class set the current avatar that can talk to the user based on a simple proximity*/
public class ProximityChat : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private List<Avatar> avatars;
    [SerializeField] private VoiceRecognition voiceRecognition;
    [SerializeField] private TextToSpeech textToSpeech;

    private Avatar activeAvatar = null;
    

    private void Update()
    {
        // Check for exit proximity if already talking to an avatar
        if (activeAvatar != null && voiceRecognition.isActive)
        {
            var distance = Vector3.Distance(activeAvatar.transform.position, transform.position);
            if (distance >= minDistance)
            {
                SaveManager.instance.AddProximityExitMessage(activeAvatar.name);
                
                activeAvatar.DeactivateAvatar();
                activeAvatar = null;
                voiceRecognition.isActive = false;
                EventManager.instance.UpdateStatus(255,255,255); //White
            }
            return;
        }
        
        // If not talking to an avatar, check for avatars in proximity
        foreach (var avatar in avatars)
        {
            // Check if avatar is close enough to the player
            var distance = Vector3.Distance(avatar.transform.position, transform.position);
            if (distance < minDistance)
            {
                activeAvatar = avatar;
                activeAvatar.ActivateAvatar();
                activeAvatar.ChatGptManager.StartConversation();
                voiceRecognition.chatGptManager = activeAvatar.ChatGptManager;
                textToSpeech.audioSource = avatar.AudioSource;
                textToSpeech.voice = avatar.voice;
                voiceRecognition.isActive = true;
                EventManager.instance.indicator = activeAvatar.indicator;
                EventManager.instance.UpdateStatus(0, 255, 0);//Green
                
                SaveManager.instance.AddProximityEnterMessage(activeAvatar.name);
                
                break;
            }
        }
    }
}
