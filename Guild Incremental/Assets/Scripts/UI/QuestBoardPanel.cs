using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardPanel : MonoBehaviour
{
    public QuestBoard questBoard;
    public GameObject prefabQuestPanel;
    public GameObject contentPanel;

    [Header("Main Quests")]
    public GameObject contentPanelMain;
    public GameObject contentPanelMainQuests;

    [Header("Guild Quests")]
    public GameObject contentPanelGuildQuests;
    public Text textGuildQuest;
    public List<QuestPanel> questPanels;
    

    bool reloadLayout = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadQuests();
    }

    // Update is called once per frame
    void Update()
    {
        if (reloadLayout)
        {
            RebuildLayout(gameObject);
            reloadLayout = false;
        }
    }

    private void OnEnable()
    {
        reloadLayout = true;
    }

    public void SetLockState(bool unlocked)
    {
        if (unlocked)
        {
            contentPanel.SetActive(true);
        }
        else
        {
            contentPanel.SetActive(false);
        }
    }

    public void LoadQuests()
    {
        foreach(Quest quest in questBoard.quests)
        {
            AddQuest(quest);
        }
        reloadLayout = true;
    }

    public void AddQuest(Quest quest)
    {
        if (QuestExists(quest)) return;

        GameObject destPanel;
        switch (quest.category)
        {
            case QuestCategory.Guild:
                destPanel = contentPanelGuildQuests;
                break;
            case QuestCategory.Main:
                destPanel = contentPanelMainQuests;
                break;
            default:
                destPanel = contentPanel;
                break;
        }

        GameObject objPanel = Instantiate(prefabQuestPanel, destPanel.transform, false);
        QuestPanel questPanel = objPanel.GetComponent<QuestPanel>();
        questPanel.quest = quest;
        questPanel.adventurer = quest.adventurer;
        
        foreach (Resource reward in quest.rewards)
            questPanel.AddReward(reward);

        questPanels.Add(questPanel);

        if (quest.category == QuestCategory.Guild) UpdateGuildQuestText();
        UpdateCategoryPanels();
        reloadLayout = true;
    }

    public void RemoveQuest(Quest quest)
    {
        foreach (QuestPanel panel in questPanels)
        {
            if (panel.quest == quest)
            {
                Object.Destroy(panel.gameObject);
                questPanels.Remove(panel);
                break;
            }
        }
        
        if (quest.category == QuestCategory.Guild) UpdateGuildQuestText();
        UpdateCategoryPanels();
        reloadLayout = true;
    }

    public bool QuestExists(Quest quest)
    {
        foreach (QuestPanel panel in questPanels)
        {
            if (panel.quest == quest) return true;
        }
        return false;
    }

    public void UpdateCategoryPanels()
    {
        if (QuestCountByCategory(QuestCategory.Main) > 0) contentPanelMain.SetActive(true);
        else contentPanelMain.SetActive(false);
    }

    public void UpdateGuildQuestText()
    {
        textGuildQuest.text = "Guild Quests (" + QuestCountByCategory(QuestCategory.Guild).ToString() + "/" + questBoard.maxGuildQuests.ToString() + ")";
    }

    public void IssueGuildQuest()
    {
        questBoard.IssueQuest();
    }

    public void RebuildLayout(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            RebuildLayout(child.gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(obj.GetComponent<RectTransform>());
    }

    public int QuestCountByCategory(QuestCategory category)
    {
        int count = 0;
        foreach (Quest quest in questBoard.quests)
            if (quest.category == category) count++;

        return count;
    }
}
