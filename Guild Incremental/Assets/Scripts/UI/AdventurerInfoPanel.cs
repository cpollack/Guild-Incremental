using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventurerInfoPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textRank;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textMerit;
    public TextMeshProUGUI textStrength;
    public TextMeshProUGUI textAgility;
    public TextMeshProUGUI textIntellect;

    public ProgressBar ExperienceBar;
    public ProgressBar HealthBar;
    public ProgressBar MagicBar;

    public TextMeshProUGUI textWeapon;
    public TextMeshProUGUI textShield;
    public TextMeshProUGUI textRing;
    public TextMeshProUGUI textRune;
    public TextMeshProUGUI textHead;
    public TextMeshProUGUI textBody;
    public TextMeshProUGUI textHand;
    public TextMeshProUGUI textFeet;

    private Adventurer adventurer = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameLib.HideIfClickedOutside(gameObject))
            return;

        UpdateAdventurerData();
    }

    private void OnEnable()
    {
        canvasGroup.blocksRaycasts = true;
        UpdateAdventurerData();
    }

    private void OnDisable()
    {
        canvasGroup.blocksRaycasts = false;
        adventurer = null;
    }

    void UpdateAdventurerData()
    {
        if (adventurer == null)
            return;

        textTitle.text = adventurer.Name + " - " + adventurer.race.ToString() + " " + adventurer.heroClass;
        textRank.text = "Rank " + adventurer.rank.ToString();
        textLevel.text = "Level " + adventurer.level.ToString();
        textMerit.text = "Merit: " + adventurer.merit.ToString();

        textStrength.text = "Strength: " + adventurer.strength.ToString() + " (2d2)";
        textAgility.text = "Agility: " + adventurer.agility.ToString() + " (2d2)";
        textIntellect.text = "Intellect: " + adventurer.intellect.ToString() + " (2d2)";

        UpdateProgressBars();
        UpdateEquipment();
    }

    private void UpdateProgressBars()
    {
        ExperienceBar.SetPercent((float)adventurer.experience / adventurer.NextLevel());
        ExperienceBar.SetText(adventurer.experience.ToString() + " / " + adventurer.NextLevel().ToString());

        HealthBar.SetPercent(adventurer.currentLife / adventurer.life);
        HealthBar.SetText(adventurer.currentLife.ToString() + " / " + adventurer.life.ToString());

        MagicBar.SetPercent(adventurer.magic == 0 ? 0 : adventurer.currentMagic / adventurer.magic);
        MagicBar.SetText(adventurer.currentMagic.ToString() + " / " + adventurer.magic.ToString());
    }

    private void UpdateEquipment()
    {
        SetEquipSlot(textWeapon, EquipmentSlot.Weapon);
        SetEquipSlot(textShield, EquipmentSlot.Shield);
        SetEquipSlot(textHead, EquipmentSlot.Head);
        SetEquipSlot(textBody, EquipmentSlot.Body);
        SetEquipSlot(textHand, EquipmentSlot.Hands);
        SetEquipSlot(textFeet, EquipmentSlot.Feet);
        SetEquipSlot(textRing, EquipmentSlot.Ring);
        SetEquipSlot(textRune, EquipmentSlot.Rune);
    }

    private void SetEquipSlot(TextMeshProUGUI textSlot, EquipmentSlot equipment)
    {
        Item equippedItem = adventurer.GetEquipedItem(equipment);
        textSlot.text = equippedItem != null ? equippedItem.name : "None";
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    public void SetAndShow(Adventurer adventurer)
    {
        this.adventurer = adventurer;
        gameObject.SetActive(true);
    }
}
