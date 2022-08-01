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

    public Image sun;
    public Image moon;

    // Start is called before the first frame update
    void Start()
    {
        textTitle.text = "Guild Incremental - " + guild.version;
    }

    // Update is called once per frame
    void Update()
    {
        textRenown.text = guild.Renown.ToString();
        textGold.text = guild.Gold.ToString();
        //textDate.text = guild.CurrentTime.GetFormattedTime() + " (" + guild.timeOfDay.ToString() + ")";
        textDate.text = guild.CurrentTime.GetFormattedTime();
        UpdateTimeIcons();
    }

    void UpdateTimeIcons()
    {
        float hour = guild.CurrentTime.hour;
        //5-17 is day
        //17-5 is night

        if (hour >= 5 && hour <= 17)
        {
            float perc = (hour - 5) / 12;
            float pos = (perc * 34) - 17;
            sun.rectTransform.anchoredPosition = new Vector2(pos, 0);
        }
        else sun.rectTransform.anchoredPosition = new Vector2(20, 0);

        if (hour <= 5 || hour >= 17)
        {
            if (hour <= 5) hour += 24;
            float perc = (hour - 17) / 12;
            float pos = (perc * 34) - 17;
            moon.rectTransform.anchoredPosition = new Vector2(pos, 0);
        }
        else moon.rectTransform.anchoredPosition = new Vector2(20, 0);
    }
}
