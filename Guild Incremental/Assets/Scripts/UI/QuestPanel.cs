using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public TextMeshProUGUI questText;
    public Image imageClaimed;
    public GameObject rewardsPanel;
    public GameObject rewardPrefab;

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
        if (quest.claimed)
        {
            imageClaimed.gameObject.SetActive(true);
            if (quest.category == QuestCategory.Main && adventurer.currentLocation != null) ButtonAssign.enabled = false;
            else ButtonAssign.enabled = true;
        }
        else
        {
            imageClaimed.gameObject.SetActive(false);
            if (quest.category == QuestCategory.Main) ButtonAssign.enabled = true;
        }
    }

    public void AddReward(Resource reward)
    {
        GameObject obj = Instantiate(rewardPrefab, rewardsPanel.transform, false);
        ResourcePanel rewardPanel = obj.GetComponent<ResourcePanel>();

        rewardPanel.resourceImage.sprite = quest.guild.GetResourceImage(reward.type);
        rewardPanel.resourceText.text = reward.value.ToString();

        string hover = "";
        switch (reward.type)
        {
            case ResourceType.Renown:
                hover = "Renown";
                break;
            case ResourceType.Gold:
                hover = "Gold";
                break;
        }
        rewardPanel.hoverInfo.info = hover;
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
