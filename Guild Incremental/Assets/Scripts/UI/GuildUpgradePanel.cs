using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildUpgradePanel : MonoBehaviour
{
    public TextMeshProUGUI activeText;
    public GameObject contentPanel;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameLib.RebuildLayout(this, contentPanel);
    }

    public void AddUpgrade(GameObject upgradeObj)
    {
        upgradeObj.transform.SetParent(contentPanel.transform, false);
    }

    public void SetActiveJobs(int current, int max)
    {
        activeText.text = "Active " + current.ToString() + "/" + max.ToString();
    }

    public void RemoveAllJobs()
    {
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
