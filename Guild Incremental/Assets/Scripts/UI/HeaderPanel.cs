using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderPanel : MonoBehaviour
{
    public Guild guild;

    public Text textRenown;
    public Text textGold;
    public Text textDate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textRenown.text = "Renown: " + guild.renown.ToString();
        textGold.text = "Gold: " + guild.gold.ToString();
        textDate.text = guild.timeString + " (" + guild.timeOfDay.ToString() + ")";
    }
}
