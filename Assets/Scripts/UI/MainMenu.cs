using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<MainMenuItem> menuItems = new List<MainMenuItem>();
    public MainMenuItem currentItem;

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
}
