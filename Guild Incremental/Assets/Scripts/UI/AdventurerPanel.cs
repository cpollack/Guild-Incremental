using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventurerPanel : MonoBehaviour
{
    public Adventurer adventurer;
    public Text textName;
    public Text textLevel;
    public Text textAction;
    public Text textPercent;

    // Start is called before the first frame update
    void Start()
    {
        textName.text = adventurer.Name;
        textLevel.text = "Level " + adventurer.level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (adventurer == null) return;
        textLevel.text = "Level " + adventurer.level.ToString();
        textAction.text = adventurer.actionString;

        //Negative percent values are used to hide the text
        if (adventurer.actionPerc >= 0) 
        {
            textPercent.enabled = true;
            textPercent.text = adventurer.actionPerc.ToString("0") + "%";
        }
        else textPercent.enabled = false;
    }
}
