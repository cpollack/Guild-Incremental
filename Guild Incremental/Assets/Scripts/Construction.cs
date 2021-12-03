using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : GuildHall
{
    public ConstructionPanel constructionPanel;
    public GameObject buildingPanelPrefab;
    public BuildingData firstBuild;
    public List<Building> currentJobs = new List<Building>();    

    private Dictionary<string, BuildingData> allJobs = new Dictionary<string, BuildingData>();

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        Object[] resources = Resources.LoadAll("Buildings", typeof(BuildingData));
        foreach (Object resource in resources)
        {
            BuildingData buildingData = (BuildingData)resource;
            allJobs.Add(buildingData.buildingID, buildingData);
        }

        LoadConstructionJobs();
    }

    // Update is called once per frame
    void Update()
    {
        //Safe loop in case job completes and removes mid-loop
        for (int i = currentJobs.Count - 1; i >= 0; i--)
        {
            currentJobs[i].Update();
        }
    }

    public void LoadConstructionJobs()
    {
        if (guild.completedBuildings.Count == 0)
        {
            AddConstructionJob(firstBuild);
            return;
        }

        foreach (KeyValuePair<string, BuildingData> kvp in allJobs)
        {
            bool skip = false;
            foreach (Building building in currentJobs)
            {
                if (building.data.buildingID == kvp.Key)
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;

            if (guild.completedBuildings.Contains(kvp.Value.requiresBuilding))
            {
                AddConstructionJob(kvp.Value);
            }
        }
    }

    public void AddConstructionJob(BuildingData data)
    {
        GameObject buildingObj = Instantiate(buildingPanelPrefab);
        BuildingUI buildingUI = buildingObj.GetComponent<BuildingUI>();
        Building building = new Building(guild, data, buildingUI);
        buildingUI.building = building;
        constructionPanel.AddBuilding(buildingObj);
        currentJobs.Add(building);
    }

    public override void CompleteBuild(string buildID)
    {
        allJobs.Remove(buildID);
        foreach (Building building in currentJobs)
        {
            if (building.data.buildingID == buildID)
            {
                currentJobs.Remove(building);                
                break;
            }
        }
        LoadConstructionJobs();
    }
}
