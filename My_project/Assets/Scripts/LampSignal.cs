using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampSignal : MonoBehaviour {
    public Text status;
    public Image image; 
    void Update(){
	if(status.text == "ON")
            image.color = new Color(0f, 255f, 0f);
	else image.color = new Color(255f,0f,0f);
    }
}
