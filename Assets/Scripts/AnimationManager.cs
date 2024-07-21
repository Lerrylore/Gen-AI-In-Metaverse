using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*This class handles the animations given by the commands retrieved by ChatGPTManager,
 every time an avatar responds with a tag that it is recognized an animation will play on the current avatar.*/
public class AnimationManager : MonoBehaviour
{
    public int delay = 2;
    public List<string> commands = new();
    public Animator animator;
    private List<string> pendingAnimations = new();
    private bool isPlaying = false;

    public void FetchAnimations(string message)
    {
        foreach (string command in commands)
        {
            if(message.Contains(command))
            {
                // if the message contains a command, add it to the pending animations list
                pendingAnimations.Add(command.Substring(1, command.Length-2));
            }
            
        }
    }

    private void Update()
    {
        if (pendingAnimations.Any() && !isPlaying) // if there are pending animations
        {
            StartCoroutine(PlayAnimation(pendingAnimations[pendingAnimations.Count-1]));
            pendingAnimations.RemoveAt(pendingAnimations.Count - 1);
        }
    }
    //We use coroutines to handle the queue of multiple animations.
    IEnumerator PlayAnimation(string animation)
    {
        isPlaying = true;
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(animation);
        // Wait for the animation to end
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        isPlaying = false;
    }
}
