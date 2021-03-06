using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuItem : MonoBehaviour
{
    public List<GameObject> ownedPanels = new List<GameObject>();
    public string requireUpgradeID = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteUpgrade(string upgradeID)
    {
        if (gameObject.activeSelf) return;
        if (requireUpgradeID == upgradeID || requireUpgradeID.Length == 0)
        {
            gameObject.SetActive(true);
            if (GetComponent<Toggle>().isOn) OnEnterTab();
        }
    }

    public void OnLeaveTab()
    {
        foreach (GameObject panel in ownedPanels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    public void OnEnterTab()
    {
        foreach (GameObject panel in ownedPanels)
        {
            panel.gameObject.SetActive(true);
        }
    }
}
