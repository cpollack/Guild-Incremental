using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfoPanel : MonoBehaviour
{
    public Canvas canvas;
    public Text hoverText;
    public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 mousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        mousePos.y -= 20;
        rectTransform.position = canvas.transform.TransformPoint(mousePos);
    }

    public void SetAndDisplay(string text)
    {
        if (text.Length == 0) return;

        hoverText.text = text;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());

        UpdatePosition();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
