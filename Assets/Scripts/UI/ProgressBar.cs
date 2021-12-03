using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("GameObject/UI/Progress Bar")]
    private static void CreateObject()
    {
        GameObject parentObj = Selection.activeGameObject;
        GameObject newObj = (GameObject)Instantiate(Resources.Load("UI/ProgressBar"));
        newObj.transform.parent = parentObj.transform;
        newObj.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetPercent(float value)
    {
        slider.value = value;
    }
}
