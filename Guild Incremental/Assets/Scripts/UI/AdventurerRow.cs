using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdventurerRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    public TextMeshProUGUI text;
    public GameObject hoverImage;

    public SelectAdventurerPanel selectAdventurerPanel;
    public Adventurer adventurer;

    // Start is called before the first frame update
    void Start()
    {
        hoverImage.SetActive(false);
        SetAdventurerText();
    }

    // Update is called once per frame
    void Update()
    {
        SetAdventurerText();
    }

    private void SetAdventurerText()
    {
        text.text = adventurer.Name + " - " + adventurer.race + " " + adventurer.heroClass + " [" + adventurer.level.ToString() + "]";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectAdventurerPanel.SetFocused(this);
        hoverImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectAdventurerPanel.LoseFocus(this);
        hoverImage.SetActive(false);
    }

    public void OnClick()
    {
        selectAdventurerPanel.Select(this);
    }
}
