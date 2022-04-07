using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Date : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        text.text = System.DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss ");
    }
}
