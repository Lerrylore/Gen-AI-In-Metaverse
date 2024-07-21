using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SystemMessage: MonoBehaviour
    {
        public TextAsset header;
        public TextAsset place;
        public TextAsset gender;
        public TextAsset action;
        public TextAsset background;
        public TextAsset footer;
        public String name;

        private string allText;

        private void Start()
        {
            String stringName = "Your name is " + name + "\n \n";
            allText = header.ToString() + stringName + place.ToString() + gender.ToString() + action.ToString() + background.ToString() + "\n \n" + footer.ToString();
        }
        public String GetSystemMessage()
        {
            return allText;
        }
    }
}