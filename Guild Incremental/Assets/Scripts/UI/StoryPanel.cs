using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    private Guild guild;
    public CanvasGroup canvasGroup;
    public GameObject contentPanel;
    public Text storyText;
    public string defaultText;

    public List<StoryEntryPanel> entryPanels = new List<StoryEntryPanel>();
    public GameObject storyEntryPanelPrefab;    
    public StoryEntryPanel activeEntry;

    private void OnEnable()
    {
        foreach (StoryEntryPanel storyEntryPanel in entryPanels)
            Destroy(storyEntryPanel.gameObject);
        entryPanels.Clear();
        activeEntry = null;
        storyText.text = defaultText;

        canvasGroup.blocksRaycasts = true;
        foreach (StoryEntry storyEntry in guild.StoryEntries)
        {
            GameObject gameObject = Instantiate(storyEntryPanelPrefab, contentPanel.transform, false);
            StoryEntryPanel storyEntryPanel = gameObject.GetComponent<StoryEntryPanel>();
            storyEntryPanel.storyPanel = this;
            storyEntryPanel.storyEntry = storyEntry;

            if (storyEntry.read) storyEntryPanel.SetInactive();
            else storyEntryPanel.SetUnread();
            entryPanels.Add(storyEntryPanel);
        }

        if (entryPanels.Count > 0) OnEntryClick(entryPanels[0]);
    }

    private void OnDisable()
    {
        canvasGroup.blocksRaycasts = false;
        guild.storyButton.UpdateButtonState();
    }

    // Start is called before the first frame update
    void Awake()
    {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
    }

    // Update is called once per frame
    void Update()
    {
        GameLib.HideIfClickedOutside(gameObject);
    }

    public void OnCloseClick()
    {        
        gameObject.SetActive(false);
    }

    public void OnEntryClick(StoryEntryPanel clickEntry)
    {
        if (activeEntry == clickEntry) return;
        if (activeEntry != null) activeEntry.SetInactive();
        activeEntry = clickEntry;
        activeEntry.SetActive();
        storyText.text = activeEntry.storyEntry.text;
        guild.storyButton.UpdateButtonState();
    }
}
