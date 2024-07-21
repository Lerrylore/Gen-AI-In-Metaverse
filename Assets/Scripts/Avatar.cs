using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class provides the methods to just activate or deactivate the avatar based on proximity*/
public class Avatar : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public ChatGptManager ChatGptManager;
    public AudioSource AudioSource;
    public GameObject indicator;
    public string voice;
    
    public void ActivateAvatar()
    {
        animator.SetTrigger("waving");
    }
    
    public void DeactivateAvatar()
    {
        animator.SetTrigger("waving");
    }
}
