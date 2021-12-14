using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct StoryEntry
{
    public bool read;
    public GameTime time;
    public string header;
    public string text;
}

public class StoryButton : MonoBehaviour
{
    public Image image;
    public Color defaultColor;
    public Color newColor;

    private Guild guild;

    // Start is called before the first frame update
    void Start()
    {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtonState()
    {
        if (IsUnreadEntries())
            image.color = newColor;
        else
            image.color = defaultColor;
    }

    private bool IsUnreadEntries()
    {
        foreach (StoryEntry storyEntry in guild.StoryEntries)
            if (!storyEntry.read) return true;

        return false;
    }
}
