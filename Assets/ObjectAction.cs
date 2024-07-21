using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*This simple clas just exposes the method addAction in order to make voiceRec updating his buffer*/
public class ObjectAction : MonoBehaviour
{
    public String action;
    private VoiceRecognition voiceRec;
    // Start is called before the first frame update
    void Start()
    {
        GameObject voiceManager = GameObject.FindGameObjectWithTag("STT");
        voiceRec = voiceManager.GetComponent<VoiceRecognition>();
    }
    public void AddAction()
    {
        voiceRec.AddAction(action);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
