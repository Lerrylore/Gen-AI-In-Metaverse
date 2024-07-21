using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/*This class exposes the method to update the indicator, everytime there is a change in the flow of the actions other 
 scripts signal this change using UpdateStatus*/
public class EventManager : Singleton<EventManager>
{
    public GameObject indicator;

    // Start is called before the first frame update
   
    public void UpdateStatus(float r, float g, float b)
    {
        indicator.GetComponent<MeshRenderer>().material.color = new Color(r,g,b);
    }

    
}
