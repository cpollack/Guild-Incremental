using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernMenuCategoryPanel : MonoBehaviour
{
    public TextMeshProUGUI textRank;
    public Image image;
    public GameObject content;
    public GameObject tavernMenuEntryPanelPrefab;
    private List<TavernMenuEntryPanel> menuEntryPanels = new List<TavernMenuEntryPanel>();

    public Rank rank;
    public Sprite collapsedSprite;
    public Sprite expandedSprite;
    private bool expanded = true;

    // Start is called before the first frame update
    void Start()
    {
        textRank.text = "Rank " + rank.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (expanded)
        {
            expanded = false;
            image.sprite = collapsedSprite;
            content.SetActive(false);
        }
        else
        {
            expanded = true;
            image.sprite = expandedSprite;
            content.SetActive(true);
        }
    }

    public void AddMenuItem(TavernMenuItem menuItem)
    {
        GameObject obj = Instantiate(tavernMenuEntryPanelPrefab, content.transform, false);
        TavernMenuEntryPanel menuEntry = obj.GetComponent<TavernMenuEntryPanel>();
        menuEntry.LoadMenuItem(menuItem);
        menuEntryPanels.Add(menuEntry);
    }
}
