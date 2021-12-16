using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryEntryPanel : MonoBehaviour
{
    public Text text;
    public Image border;

    private bool isActive;
    public Color activeColor;
    public Color inactiveColor;    
    public Color unreadColor;

    public StoryEntry storyEntry;
    public StoryPanel storyPanel;

    // Start is called before the first frame update
    void Start()
    {
        text.text = storyEntry.time.GetFormattedTime() + "\n" + storyEntry.header;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive()
    {
        border.color = activeColor;
        storyEntry.read = true;
        isActive = true;
    }

    public void SetInactive()
    {
        border.color = inactiveColor;
        isActive = false;
    }

    public void SetUnread()
    {
        border.color = unreadColor;
        isActive = false;
    }

    public void OnClick()
    {
        if (isActive) return;
        storyPanel.OnEntryClick(this);
    }
}
