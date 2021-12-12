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

    public void CompleteBuilding(string buildID)
    {
        foreach (MainMenuItem menuItem in menuItems) {
            menuItem.CompleteBuilding(buildID);
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

    public void Reset(List<string> completedBuildings, bool gotoDefTab = false)
    {
        if (gotoDefTab)
        {
            defaultToggle.isOn = true;
        }

        foreach (MainMenuItem menuItem in menuItems)
        {
            if (menuItem.requireBuildID.Length == 0) menuItem.gameObject.SetActive(true);
            else if (completedBuildings.Contains(menuItem.requireBuildID)) menuItem.gameObject.SetActive(true);
            else menuItem.gameObject.SetActive(false);
        }
    }
}
