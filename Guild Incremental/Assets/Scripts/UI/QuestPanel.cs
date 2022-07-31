using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public TextMeshProUGUI questText;
    public Image imageIcon;
    public GameObject objectivePanel;
    public GameObject rewardsPanel;
    public GameObject resourcePrefab;

    public Button ButtonAssign;
    public TextMeshProUGUI TextAssign;

    public Quest quest;
    public Adventurer adventurer;

    // Start is called before the first frame update
    void Start()
    {
        questText.text = quest.ToString();
        SetMainQuestState();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update()
    {
        questText.text = quest.ToString();
        if (quest.category == QuestCategory.Project)
        {
            UpdateObjectives();
            return;
        }

        if (quest.claimed)
        {
            if (imageIcon != null) imageIcon.gameObject.SetActive(true);
            if (ButtonAssign == null) return;
            if (quest.category == QuestCategory.Main && quest.adventurer.currentLocation != null) ButtonAssign.enabled = false;
            else ButtonAssign.enabled = true;
        }
        else
        {
            if (imageIcon != null) imageIcon.gameObject.SetActive(false);
            if (ButtonAssign == null) return;
            if (quest.category == QuestCategory.Main) ButtonAssign.enabled = true;
        }
    }

    private void UpdateObjectives()
    {
        if (objectivePanel == null) return;
        if (quest.objectives == null) return;

        //objectivePanel.transform.childCount
        int itr = 0;
        foreach (Transform child in objectivePanel.transform)
        {
            if (quest.objectives.Count < itr) break;
            QuestObjective questObjective = quest.objectives[itr];
            ResourcePanel resourcePanel = child.GetComponent<ResourcePanel>();
            resourcePanel.resourceImage.sprite = quest.guild.GetResourceImage(questObjective.type);
            resourcePanel.resourceText.text = questObjective.name + " " + questObjective.current.ToString() + "/" + questObjective.count.ToString();
            itr++;
        }

        while (itr < quest.objectives.Count)
        {
            AddObjective(quest.objectives[itr]);
            itr++;
        }
    }

    public void AddObjective(QuestObjective questObjective)
    {
        GameObject obj = Instantiate(resourcePrefab, objectivePanel.transform, false);
        ResourcePanel resourcePanel = obj.GetComponent<ResourcePanel>();

        resourcePanel.resourceImage.sprite = quest.guild.GetResourceImage(questObjective.type);
        resourcePanel.resourceText.text = questObjective.name + " " + questObjective.current.ToString() + "/" + questObjective.count.ToString();
    }

    public void AddReward(Resource reward)
    {
        GameObject obj = Instantiate(resourcePrefab, rewardsPanel.transform, false);
        ResourcePanel resourcePanel = obj.GetComponent<ResourcePanel>();

        resourcePanel.resourceImage.sprite = quest.guild.GetResourceImage(reward.type);
        resourcePanel.resourceText.text = reward.value.ToString();

        string hover = "";
        switch (reward.type)
        {
            case ResourceType.Renown:
                hover = "Renown";
                break;
            case ResourceType.Gold:
                hover = "Gold";
                break;
            case ResourceType.Merit:
                hover = "Merit";
                break;
        }
        resourcePanel.hoverInfo.info = hover;
    }

    private void SetMainQuestState()
    {
        if (quest.category != QuestCategory.Main) return;
        if (quest.adventurer == null) TextAssign.text = "Assign";
        else TextAssign.text = "Rescind";
    }

    public void ButtonAssignClick()
    {
        SelectAdventurerPanel panel = GameObject.Find("Guild").GetComponent<Guild>().selectAdventurerPanel;
        panel.onSelectDelegate = OnAdventurerSelect;
        panel.ClearFilters();
        panel.AddFilter(AdventurerFilter.HasAssignedQuest);
        panel.gameObject.SetActive(true);
    }

    public void OnAdventurerSelect(Adventurer adv)
    {
        quest.Claim(adv);
        SetMainQuestState();
    }
}
