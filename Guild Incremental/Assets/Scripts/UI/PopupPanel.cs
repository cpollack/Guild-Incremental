using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text textTime;
    public Text textContent;

    public int displayForMS;
    public int fadeOutMS;

    private float elapsed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > (displayForMS + fadeOutMS) / 1000)
        {
            gameObject.SetActive(false);
            return;
        }
        if (elapsed > displayForMS / 1000)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, ((elapsed * 1000) - displayForMS) / fadeOutMS);
        }
    }

    public void Popup(string time, string content)
    {
        textTime.text = time;
        textContent.text = content;
        elapsed = 0;
        canvasGroup.alpha = 1;
        gameObject.SetActive(true);
    }
}
