using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHall : GuildHall
{
    public MainHallPanel mainHallPanel;
    public GameObject adventurersObject;
    public GameObject adventurerPrefab;
    public List<Adventurer> adventurers = new List<Adventurer>();

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnFirstAdventurer()
    {
        GameObject gameObject = Instantiate(adventurerPrefab);
        Adventurer adventurer = gameObject.GetComponent<Adventurer>();
        gameObject.transform.SetParent(adventurersObject.transform, false);
        adventurers.Add(adventurer);
        mainHallPanel.AddAdventurer(adventurer);
    }
}
