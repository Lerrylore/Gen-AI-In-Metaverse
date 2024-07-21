using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*This script activates the related gameObject everytime the button related to recordInputAction is pressed.*/
public class HandMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference recordInputAction;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject lineRenderer;
    private bool isActive;
    
    private void OnEnable()
    {
        recordInputAction.action.started += ShowMenu;
    }
    
    private void OnDisable()
    {
        recordInputAction.action.started -= ShowMenu;
    }

    private void ShowMenu(InputAction.CallbackContext ctx)
    {
        if (isActive)
        {
            // Deactivate
            lineRenderer.SetActive(true);
            menu.SetActive(false);
            isActive = false;
            return;
        }
        
        // Activate
        lineRenderer.SetActive(false);
        menu.SetActive(true);
        isActive = true;
    }
}
