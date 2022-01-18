using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionPanel : MonoBehaviour
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

    public void AddBuilding(GameObject buildObj)
    {
        buildObj.transform.SetParent(contentPanel.transform, false);
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
