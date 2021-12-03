using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallPanel : MonoBehaviour
{
    public GameObject panelContent;
    public GameObject adventurerPanelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        GameObject gameObject = Instantiate(adventurerPanelPrefab);
        gameObject.transform.SetParent(panelContent.transform, false);
        gameObject.GetComponent<AdventurerPanel>().adventurer = adventurer;
    }
}
