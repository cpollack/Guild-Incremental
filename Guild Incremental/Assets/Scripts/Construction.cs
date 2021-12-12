using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : GuildHall
{
    public ConstructionPanel constructionPanel;
    public GameObject buildingPanelPrefab;
    public BuildingData firstBuild;
    public int maxJobs = 1;
    public List<Building> currentJobs = new List<Building>();    

    private Dictionary<string, BuildingData> allJobs;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
        allJobs = new Dictionary<string, BuildingData>();

        Object[] resources = Resources.LoadAll("Buildings", typeof(BuildingData));
        foreach (Object resource in resources)
        {
            BuildingData buildingData = (BuildingData)resource;
            allJobs.Add(buildingData.buildingID, buildingData);
        }
    }

    private void Start()
    {
        LoadConstructionJobs();
        constructionPanel.SetActiveJobs(0, maxJobs);
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        //Safe loop in case job completes and removes mid-loop
        for (int i = currentJobs.Count - 1; i >= 0; i--)
        {
            if (currentJobs[i].IsActive()) count++;
            currentJobs[i].Update();            
        }
        constructionPanel.SetActiveJobs(count, maxJobs);
    }

    public void Load(List<Building> currentBuildProjects, int maxConstructionJobs)
    {
        foreach (string buildID in guild.CompletedBuildings)
            allJobs.Remove(buildID);

        foreach (Building building in currentBuildProjects)
        {
            AddConstructionJob(building);
            building.OnLoad();
        }

        maxJobs = maxConstructionJobs;
    }

    public override void ResetGame()
    {
        LoadResources();
        guild.CompletedBuildings.Clear();
        constructionPanel.RemoveAllJobs();
        currentJobs.Clear();

        LoadConstructionJobs();
        constructionPanel.SetActiveJobs(0, maxJobs);
    }

    public void LoadConstructionJobs()
    {
        if (guild.CompletedBuildings.Count == 0 && currentJobs.Count == 0)
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

            if (guild.CompletedBuildings.Contains(kvp.Value.requiresBuilding))
            {
                AddConstructionJob(kvp.Value);
            }
        }
    }

    public BuildingData FindBuildingData(string id)
    {
        foreach (KeyValuePair<string, BuildingData> kvp in allJobs)
        {
            if (kvp.Value.buildingID == id) return kvp.Value;
        }
        return null;
    }

    public void AddConstructionJob(BuildingData data)
    {
        GameObject buildingObj = Instantiate(buildingPanelPrefab);
        BuildingPanel buildingUI = buildingObj.GetComponent<BuildingPanel>();
        Building building = new Building(guild, data, buildingUI);
        buildingUI.building = building;
        constructionPanel.AddBuilding(buildingObj);
        currentJobs.Add(building);
    }

    public void AddConstructionJob(Building building)
    {
        building.guild = guild;
        if (building.buildingID.Length > 0 && building.data == null)
            building.data = FindBuildingData(building.buildingID);        

        GameObject buildingObj = Instantiate(buildingPanelPrefab);
        BuildingPanel buildingUI = buildingObj.GetComponent<BuildingPanel>();
        building.buildingPanel = buildingUI;
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
