using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<MainMenuItem> menuItems = new List<MainMenuItem>();
    public MainMenuItem currentItem;
    public Toggle defaultToggle;

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
        foreach (MainMenuItem menuItem in menuItems) {
            menuItem.CompleteUpgrade(upgradeID);
        }
    }

    public void OnChange(MainMenuItem menuItem)
    {
        if (currentItem == menuItem) return;
        if (menuItem == null) return;
        if (!menuItem.GetComponent<Toggle>().isOn) return;

        currentItem.OnLeaveTab();
        currentItem = menuItem;
        currentItem.OnEnterTab();
    }

    public void Reset(List<string> completedUpgrades, bool gotoDefTab = false)
    {
        if (gotoDefTab)
        {
            defaultToggle.isOn = true;
        }

        foreach (MainMenuItem menuItem in menuItems)
        {
            if (menuItem.requireUpgradeID.Length == 0) menuItem.gameObject.SetActive(true);
            else if (completedUpgrades.Contains(menuItem.requireUpgradeID)) menuItem.gameObject.SetActive(true);
            else menuItem.gameObject.SetActive(false);
        }
    }
}
