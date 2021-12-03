using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventurerPanel : MonoBehaviour
{
    public Adventurer adventurer;
    public Text textAction;
    public Text textPercent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (adventurer == null) return;
        textAction.text = adventurer.actionString;

        if (adventurer.actionPerc >= 0) 
        {
            textPercent.enabled = true;
            textPercent.text = adventurer.actionPerc.ToString("0") + "%";
        }
        else textPercent.enabled = false;
    }
}
