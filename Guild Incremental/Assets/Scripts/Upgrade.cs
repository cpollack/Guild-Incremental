using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UpgradeResource
{
    public ResourceType resourceType;
    public int value;

    public UpgradeResource(ResourceType type, int val)
    {
        resourceType = type;
        value = val;
    }
}

[Serializable]
public class Upgrade
{
    public Upgrade()
    {
        //empty constructor for json utility
    }

    public Upgrade(Guild guild, UpgradeData upgradeData, UpgradePanel upgradeUI)
    {
        this.guild = guild;
        data = upgradeData;
        upgradeID = data.upgradeID;
        this.upgradePanel = upgradeUI;
        this.upgradePanel.upgrade = this;

        upgradeMode = UpgradeMode.None;
        Started = false;
        Paused = false;
        Completed = false;
        UpdateButton();
        SetUpgradeData();
    }

    public string upgradeID;
    [NonSerialized] public UpgradeData data;
    [NonSerialized] public Guild guild;
    [NonSerialized] public UpgradePanel upgradePanel;

    public bool Started;
    public bool Paused;
    public bool Completed;    

    public GameTime startTime = new GameTime();
    public GameTime pauseTime = new GameTime();

    public enum UpgradeMode
    {
        None,
        Upgrade,
        Pause,
        Continue,
    }
    public UpgradeMode upgradeMode;

    // Update is called once per frame
    public void Update()
    {
        if (Started && !Paused && !Completed)
        {
            GameTime elapsedTime = guild.GetElapsedTime(startTime);
            float elapsed = elapsedTime.GetHours();
            float required = (data.timeDays * 24) + data.timeHours;
            float perc = Mathf.Min(elapsed / required, 1f);
            upgradePanel.progressBar.SetPercent(perc);
            if (perc == 1)
            {
                OnComplete();
            }
            SetRemainingTime();
        }
    }

    public void OnLoad()
    {
        UpdateButton();
        SetUpgradeData();
        if (Started)
        {         
            GameTime elapsedTime;
            if (Paused) elapsedTime = pauseTime.GetDifference(startTime);
            else elapsedTime = guild.GetElapsedTime(startTime);
            float elapsed = elapsedTime.GetHours();
            float required = (data.timeDays * 24) + data.timeHours;
            float perc = Mathf.Min(elapsed / required, 1f);
            upgradePanel.progressBar.SetPercent(perc);

            SetRemainingTime();
        }
    }

    void SetRemainingTime()
    {
        GameTime elapsedTime;
        if (Paused) elapsedTime = pauseTime.GetDifference(startTime);
        else elapsedTime = guild.GetElapsedTime(startTime);
        GameTime remTime = new GameTime(data.timeDays, data.timeHours);
        remTime = remTime.GetDifference(elapsedTime);
        string strRem;
        if (remTime.day == 0 && remTime.hour < 1)
        {
            int mins = (int)(60 * remTime.hour);
            if (mins < 1) mins = 1;
            strRem = mins.ToString() + " Minute" + (mins == 1 ? "" : "s") + " Remaining";
        }
        else strRem = remTime.ToString() + " Remaining";
        upgradePanel.textTime.text = strRem;
    }

    public void OnClick()
    {
        switch (upgradeMode) {
            case UpgradeMode.Upgrade:
                StartUpgrade();
                break;
            case UpgradeMode.Pause:
                GameTime dif = new GameTime();
                dif.Set(pauseTime.GetDifference(startTime));
                startTime.Set(guild.CurrentTime);
                startTime.SubtractHours(dif.GetHours());
                pauseTime.Set(0, 0);

                Paused = false;
                UpdateButton();
                break;
            case UpgradeMode.Continue:
                Paused = true;
                pauseTime.Set(guild.CurrentTime);
                UpdateButton();
                break;
        }
    }

    private void StartUpgrade()
    {
        if (guild.Renown < data.requiredRenown) return;
        if (guild.Gold < GetCost(ResourceType.Gold)) return;

        guild.Gold -= GetCost(ResourceType.Gold);
        Started = true;
        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (!Started)
        {
            if (guild.Renown < data.requiredRenown)
                upgradeMode = UpgradeMode.None;
            else
                upgradeMode = UpgradeMode.Upgrade;
        }
        else
        {
            if (Paused)
                upgradeMode = UpgradeMode.Pause;
            else
                upgradeMode = UpgradeMode.Continue;
        }

        switch (upgradeMode) {
            case UpgradeMode.None:
                upgradePanel.textButton.text = data.requiredRenown.ToString("0") + " Renown";
                break;
            case UpgradeMode.Upgrade:
                upgradePanel.textButton.text = "Upgrade";
                break;
            case UpgradeMode.Pause:
                upgradePanel.textButton.text = "Resume";
                break;
            case UpgradeMode.Continue:
                upgradePanel.textButton.text = "Pause";
                break;
        }
    }

    private void SetUpgradeData()
    {
        upgradePanel.textName.text = data.title;
        upgradePanel.textFlavor.text = data.description;

        GameTime upgradeTime = new GameTime(data.timeDays, data.timeHours);
        string strRem;
        if (upgradeTime.day == 0 && upgradeTime.hour < 1)
        {
            int mins = (int)(60 * upgradeTime.hour);
            if (mins < 1) mins = 1;
            strRem = mins.ToString() + " Minute" + (mins == 1 ? "" : "s");
        }
        else strRem = upgradeTime.ToString();
        upgradePanel.textTime.text = strRem;        

        foreach (UpgradeResource resource in data.cost)
            upgradePanel.AddCost(resource);
    }

    private void OnComplete()
    {
        Completed = true;
        guild.CompleteUpgrade(data.upgradeID);
        upgradePanel.OnComplete();
        if (data.completeLog.Length > 0) guild.AddLogEntry(data.completeLog);
    }

    public bool IsActive()
    {
        return Started && !Paused;
    }

    private int GetCost(ResourceType resource)
    {
        foreach (UpgradeResource upgradeResource in data.cost)
            if (upgradeResource.resourceType == resource)
                return upgradeResource.value;
        return 0;
    }
}
