using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderPanel : MonoBehaviour
{
    public Guild guild;

    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textRenown;
    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textDate;

    // Start is called before the first frame update
    void Start()
    {
        textTitle.text = "Guild Incremental - v" + Application.version + " (Alpha)";
    }

    // Update is called once per frame
    void Update()
    {
        textRenown.text = guild.Renown.ToString();
        textGold.text = guild.Gold.ToString();
        //textDate.text = guild.CurrentTime.GetFormattedTime() + " (" + guild.timeOfDay.ToString() + ")";
        textDate.text = guild.CurrentTime.GetFormattedTime();
    }
}
