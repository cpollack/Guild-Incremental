using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{
    public Text questText;
    public Quest quest;
    public Adventurer adventurer;

    // Start is called before the first frame update
    void Start()
    {
        questText.text = quest.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    void Update()
    {
        questText.text = quest.ToString();
    }
}
