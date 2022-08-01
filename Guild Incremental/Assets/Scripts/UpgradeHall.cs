using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHall : GuildHall
{
    public GuildUpgradePanel upgradePanel;
    public GameObject upgradePanelPrefab;
    public UpgradeData firstUpgrade;   

    private Dictionary<string, UpgradeData> allUpgrades;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
        allUpgrades = new Dictionary<string, UpgradeData>();

        Object[] resources = Resources.LoadAll("Upgrades", typeof(UpgradeData));
        foreach (Object resource in resources)
        {
            UpgradeData upgradeData = (UpgradeData)resource;
            allUpgrades.Add(upgradeData.upgradeID, upgradeData);
        }
    }

    private void Start()
    {
        LoadUpgrades();
        upgradePanel.SetActiveJobs(0, guild.MaxActiveUpgrades);
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        //Safe loop in case job completes and removes mid-loop
        for (int i = guild.ActiveUpgrades.Count - 1; i >= 0; i--)
        {
            if (guild.ActiveUpgrades[i].IsActive()) count++;
            guild.ActiveUpgrades[i].Update();            
        }
        upgradePanel.SetActiveJobs(count, guild.MaxActiveUpgrades);
    }

    public override void Load()
    {
        foreach (string upgradeID in guild.CompletedUpgrades)
            allUpgrades.Remove(upgradeID);

        upgradePanel.RemoveAllJobs();
        foreach (Upgrade upgrade in guild.ActiveUpgrades)
        {            
            AddUpgradeJob(upgrade);
            upgrade.OnLoad();
        }
    }

    public override void ResetGame()
    {
        LoadResources();
        upgradePanel.RemoveAllJobs();

        LoadUpgrades();
        upgradePanel.SetActiveJobs(0, guild.MaxActiveUpgrades);
    }

    public void LoadUpgrades()
    {
        if (guild.CompletedUpgrades.Count == 0 && guild.ActiveUpgrades.Count == 0)
        {
            AddUpgradeJob(firstUpgrade);
            return;
        }

        foreach (KeyValuePair<string, UpgradeData> kvp in allUpgrades)
        {
            bool skip = false;
            foreach (Upgrade upgrade in guild.ActiveUpgrades)
            {
                if (upgrade.data.upgradeID == kvp.Key)
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;

            if (guild.CompletedUpgrades.Contains(kvp.Value.requiresUpgrade))
            {
                AddUpgradeJob(kvp.Value);
            }
        }
    }

    public UpgradeData FindUpgradeData(string id)
    {
        foreach (KeyValuePair<string, UpgradeData> kvp in allUpgrades)
        {
            if (kvp.Value.upgradeID == id) return kvp.Value;
        }
        return null;
    }

    public void AddUpgradeJob(UpgradeData data)
    {
        GameObject upgradeObj = Instantiate(upgradePanelPrefab);
        UpgradePanel upgradeUI = upgradeObj.GetComponent<UpgradePanel>();
        Upgrade upgrade = new Upgrade(guild, data, upgradeUI);
        upgradeUI.upgrade = upgrade;
        upgradePanel.AddUpgrade(upgradeObj);
        guild.ActiveUpgrades.Add(upgrade);
    }

    public void AddUpgradeJob(Upgrade upgrade)
    {
        upgrade.guild = guild;
        if (upgrade.upgradeID.Length > 0 && upgrade.data == null)
            upgrade.data = FindUpgradeData(upgrade.upgradeID);        

        GameObject upgradeObj = Instantiate(upgradePanelPrefab);
        UpgradePanel upgradeUI = upgradeObj.GetComponent<UpgradePanel>();
        upgrade.upgradePanel = upgradeUI;
        upgradeUI.upgrade = upgrade;
        upgradePanel.AddUpgrade(upgradeObj);
    }

    public override void CompleteUpgrade(string upgradeID)
    {
        allUpgrades.Remove(upgradeID);
        foreach (Upgrade upgrade in guild.ActiveUpgrades)
        {
            if (upgrade.data.upgradeID == upgradeID)
            {
                guild.ActiveUpgrades.Remove(upgrade);                
                break;
            }
        }
        LoadUpgrades();
    }
}
