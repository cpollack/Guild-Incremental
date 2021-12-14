using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallPanel : MonoBehaviour
{
    public GameObject panelContent;
    public GameObject adventurerPanelPrefab;

    public List<AdventurerPanel> adventurerPanels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveAllAdventurers()
    {
        foreach (AdventurerPanel panel in adventurerPanels)
            Destroy(panel.gameObject);
        adventurerPanels.Clear();
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        GameObject gameObject = Instantiate(adventurerPanelPrefab);
        gameObject.transform.SetParent(panelContent.transform, false);
        AdventurerPanel adventurerPanel = gameObject.GetComponent<AdventurerPanel>();
        adventurerPanel.adventurer = adventurer;
        adventurerPanels.Add(adventurerPanel);
    }
}
