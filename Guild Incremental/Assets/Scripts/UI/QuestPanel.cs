using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public Text questText;
    public Image imageClaimed;
    public GameObject rewardsPanel;
    public GameObject rewardPrefab;
    public Text TextRequisition;

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
        if (quest.claimed) imageClaimed.gameObject.SetActive(true);
        else imageClaimed.gameObject.SetActive(false);
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
            case ResourceType.Bank:
                hover = "Coffers";
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
        if (quest.adventurer == null) TextRequisition.text = "Requisition";
        else TextRequisition.text = "Rescind";
    }

    public void ButtonRequisitionClick()
    {
        SelectAdventurerPanel panel = GameObject.Find("Guild").GetComponent<Guild>().selectAdventurerPanel;
        panel.onSelectDelegate = OnAdventurerSelect;
        panel.gameObject.SetActive(true);
    }

    public void OnAdventurerSelect(Adventurer adv)
    {
        //quest.Claim(adv);
        //SetMainQuestState();
    }
}
