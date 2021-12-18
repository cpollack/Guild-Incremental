using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardPanel : MonoBehaviour
{
    public QuestBoard questBoard;        
    public GameObject contentPanel;

    [Header("Main Quests")]
    public GameObject contentPanelMain;
    public GameObject contentPanelMainQuests;
    public GameObject prefabQuestMainPanel;

    [Header("Guild Quests")]
    public GameObject contentPanelGuildQuests;  
    public Text textGuildQuest;
    public GameObject prefabQuestGuildPanel;
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
            GameLib.RebuildLayout(this, gameObject);
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
        foreach(Quest quest in questBoard.GetQuests())
        {
            AddQuest(quest);
        }
        UpdateGuildQuestText();
        reloadLayout = true;
    }

    public void AddQuest(Quest quest)
    {
        if (QuestExists(quest)) return;

        GameObject destPanel;
        GameObject prefab;
        switch (quest.category)
        {
            case QuestCategory.Guild:
                destPanel = contentPanelGuildQuests;
                prefab = prefabQuestGuildPanel;
                break;
            case QuestCategory.Main:
                destPanel = contentPanelMainQuests;
                prefab = prefabQuestMainPanel;
                break;
            default:
                destPanel = null;
                prefab = null;
                Debug.LogError("QuestBoardPanel::AddQuest unhandled quest category [" + quest.category + "]");
                break;
        }

        GameObject objPanel = Instantiate(prefab, destPanel.transform, false);
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

    public void RemoveAllQuests()
    {
        foreach (QuestPanel panel in questPanels)
            Destroy(panel.gameObject);
        questPanels.Clear();

        UpdateGuildQuestText();
        UpdateCategoryPanels();
        reloadLayout = true;
    }

    public void RemoveQuest(Quest quest)
    {
        foreach (QuestPanel panel in questPanels)
        {
            if (panel.quest == quest)
            {
                Destroy(panel.gameObject);
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
        textGuildQuest.text = "Guild Quests (" + QuestCountByCategory(QuestCategory.Guild).ToString() + "/" + questBoard.GetMaxGuildQuests().ToString() + ")";
    }

    public void IssueGuildQuest()
    {
        questBoard.IssueQuest();
    }

    public int QuestCountByCategory(QuestCategory category)
    {
        int count = 0;
        foreach (Quest quest in questBoard.GetQuests())
            if (quest.category == category) count++;

        return count;
    }
}
