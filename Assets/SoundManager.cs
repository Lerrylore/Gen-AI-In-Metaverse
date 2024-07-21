using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*This class is useful only to be linked with the menu*/
public class SoundManager : MonoBehaviour
{

    public Slider slider;
    public AudioSource source;
    // Start is called before the first frame updat
    private void Start()
    {
        source.volume= SceneLoader.instance.GetVolume();
        slider.value = source.volume;
    }
    // Update is called once per frame
    public void UpdateVolume()
    {
        source.volume = slider.value;
    }
}
