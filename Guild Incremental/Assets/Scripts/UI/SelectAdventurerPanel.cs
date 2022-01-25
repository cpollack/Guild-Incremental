using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AdventurerFilter
{
    HasAssignedQuest,
}

public struct AdvFilterSet
{
    public AdventurerFilter filter;
    public int value;
}

public class SelectAdventurerPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public GameObject content;
    public GameObject adventurerRowPrefab;

    public List<AdventurerRow> rows;
    private List<AdvFilterSet> filters = new List<AdvFilterSet>();
    public AdventurerRow focusedRow;

    public delegate void OnSelect(Adventurer adventurer);
    public OnSelect onSelectDelegate;

    public Guild guild;

    // Start is called before the first frame update
    void Awake()
    {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameLib.HideIfClickedOutside(gameObject))
            Select(null);
    }

    private void OnEnable()
    {
        canvasGroup.blocksRaycasts = true;

        foreach (var row in rows)
            Destroy(row.gameObject);
        rows.Clear();

        foreach (Adventurer adventurer in guild.Adventurers)
        {
            foreach (var filterSet in filters)
            {
                switch(filterSet.filter)
                {
                    case AdventurerFilter.HasAssignedQuest:
                        if (adventurer.assignedQuest != null) continue;
                        break;
                }
            }

            GameObject advObj = Instantiate(adventurerRowPrefab, content.transform, false);
            AdventurerRow row = advObj.GetComponent<AdventurerRow>();
            row.selectAdventurerPanel = this;
            row.adventurer = adventurer;
            rows.Add(row);
        }
    }

    private void OnDisable()
    {
        foreach (var row in rows)
            Destroy(row.gameObject);
        rows.Clear();

        canvasGroup.blocksRaycasts = false;
        onSelectDelegate = null;
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    public void SetFocused(AdventurerRow row)
    {
        focusedRow = row;
    }

    public void LoseFocus(AdventurerRow row)
    {
        if (focusedRow == row) row = null;
    }

    public void Select(AdventurerRow row)
    {
        if (onSelectDelegate != null) 
            onSelectDelegate(row.adventurer);
        gameObject.SetActive(false);
    }

    public void ClearFilters()
    {
        filters?.Clear();
    }

    public void AddFilter(AdventurerFilter filter, int value = 0)
    {
        var filterSet = new AdvFilterSet();
        filterSet.filter = filter;
        filterSet.value = value;
        filters.Add(filterSet);
    }
}
