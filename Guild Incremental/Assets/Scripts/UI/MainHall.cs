using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHall : GuildHall
{
    public MainHallPanel mainHallPanel;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Load()
    {
        foreach(Adventurer adventurer in guild.Adventurers)
        {
            mainHallPanel.AddAdventurer(adventurer);
        }
    }

    public override void ResetGame()
    {
        mainHallPanel.RemoveAllAdventurers();
    }

    public void SpawnFirstAdventurer()
    {
        Adventurer adventurer = new Adventurer(guild);
        adventurer.Name = "Adventurer A";
        guild.Adventurers.Add(adventurer);
        mainHallPanel.AddAdventurer(adventurer);
    }
}
