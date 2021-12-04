using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoardPanel : MonoBehaviour
{
    public QuestBoard questBoard;
    public GameObject contentPanel;
    public GameObject contentPanelGuild;
    public Text textGuildQuest;
    public List<QuestPanel> questPanels;
    public GameObject prefabQuestPanel;

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
        if (quest.category == QuestCategory.Guild) destPanel = contentPanelGuild;
        else destPanel = contentPanel;

        GameObject objPanel = Instantiate(prefabQuestPanel, destPanel.transform, false);
        QuestPanel questPanel = objPanel.GetComponent<QuestPanel>();
        questPanel.quest = quest;
        questPanel.adventurer = quest.adventurer;
        questPanels.Add(questPanel);

        UpdateGuildQuestText();
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
        
        UpdateGuildQuestText();
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

    public void UpdateGuildQuestText()
    {
        int count = 0;
        foreach (QuestPanel panel in questPanels)
        {
            if (panel.quest.category == QuestCategory.Guild) count++;
        }
        textGuildQuest.text = "Guild Quests (" + count.ToString() + "/" + questBoard.maxGuildQuests.ToString() + ")";
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
}
