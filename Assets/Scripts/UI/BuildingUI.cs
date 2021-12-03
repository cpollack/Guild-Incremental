using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public Text textName;
    public Text textTime;
    public Text textFlavor;
    public Text textCost;
    public Button button;
    public Text textButton;
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
}
