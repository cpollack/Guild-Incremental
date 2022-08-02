using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventurerPanel : MonoBehaviour
{
    public Adventurer adventurer;

    [Header("Info")]
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textRace;
    public TextMeshProUGUI textClass;
    public TextMeshProUGUI textLevel;        

    [Header("Action")]
    public Slider progressBar;
    public GameObject rowAction;
    public TextMeshProUGUI textAction;
    public TextMeshProUGUI textPercent; //Will most likely be removed

    [Header("Battle")]
    public GameObject rowBattle;
    public ProgressBar adventurerLife;
    public ProgressBar monsterLife;

    // Start is called before the first frame update
    void Start()
    {
        textName.text = adventurer.Name;
        textRace.text = adventurer.race.ToString();
        textClass.text = adventurer.heroClass.ToString();
        textLevel.text = "[" + adventurer.level.ToString() + "]";
        progressBar.value = 0;
        rowAction.SetActive(true);
        rowBattle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (adventurer == null) return;
        textLevel.text = "[" + adventurer.level.ToString() + "]";
        textAction.text = adventurer.actionString;        

        if (adventurer.battle != null)
        {
            rowAction.SetActive(false);
            rowBattle.SetActive(true);
            progressBar.value = adventurer.actionPerc / 100.0f;

            adventurerLife.SetPercent(adventurer.currentLife / adventurer.life);
            adventurerLife.SetText(adventurer.currentLife.ToString() + " / " + adventurer.life.ToString());

            monsterLife.SetPercent(adventurer.battle.GetMonsterHealthPerc(0));
            monsterLife.SetText(adventurer.battle.GetMonsterName(0));
            return;
        }

        rowAction.SetActive(true);
        rowBattle.SetActive(false);
        //Negative percent values are used to hide the text
        if (adventurer.actionPerc >= 0)
        {
            progressBar.value = adventurer.actionPerc / 100.0f;
        }
        else progressBar.value = 0;
    }

    public void OnClick()
    {
        GameLib.Guild().adventurerInfoPanel.SetAndShow(adventurer);
    }
}
