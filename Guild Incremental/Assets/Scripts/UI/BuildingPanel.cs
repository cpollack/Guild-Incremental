using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textFlavor;
    public GameObject costPanel;
    public GameObject resourcePrefab;
    public Button button;
    public TextMeshProUGUI textButton;
    public ProgressBar progressBar;
    public Building building;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        building.OnClick();
    }

    public void OnComplete()
    {
        Destroy(gameObject);
    }

    public void AddCost(BuildingResource resource)
    {
        GameObject obj = Instantiate(resourcePrefab, costPanel.transform, false);
        ResourcePanel rewardPanel = obj.GetComponent<ResourcePanel>();

        rewardPanel.resourceImage.sprite = building.guild.GetResourceImage(resource.resourceType);
        rewardPanel.resourceText.text = resource.value.ToString();

        string hover = "";
        switch (resource.resourceType)
        {
            case ResourceType.Renown:
                hover = "Renown";
                break;
            case ResourceType.Gold:
                hover = "Gold";
                break;
        }
        rewardPanel.hoverInfo.info = hover;
    }
}
