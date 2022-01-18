using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)]
    public string info;
    private HoverInfoPanel hoverInfoPanel;
    private bool MouseIn = false;
    private bool Hovering = false;
    private float hoverWait = 0.3f;
    private float hoverTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        hoverInfoPanel = GameObject.Find("Guild").GetComponent<Guild>().hoverInfoPanel;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MouseIn) return;
        hoverTime += Time.deltaTime;
        if (!Hovering)
        {
            if (hoverTime >= hoverWait)
            {
                Hovering = true;
                hoverInfoPanel.SetAndDisplay(info);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseIn = true;
        hoverTime = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseIn = false;
        Hovering = false;
        hoverInfoPanel.Hide();
    }
}
