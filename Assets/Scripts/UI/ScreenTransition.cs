using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    private RectTransform rectTransform;

    public bool screenOpen = false;
    public bool screenClose = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        if (screenOpen)
        {
            rectTransform.sizeDelta += new Vector2(4, 4);
            if (rectTransform.rect.width >= 100)
            {
                screenOpen = false;
            }
        }
        else if (screenClose)
        {
            rectTransform.sizeDelta -= new Vector2(4, 4);
            if (rectTransform.rect.width <= 0)
            {
                screenClose = false;
            }
        }


    }


    public void OpenUpScreen()
    {
        GameManager.Instance.Invoke("IHateScreenTransitioning", 0.1f);
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 0);
        screenOpen = true;
    }

    public void CloseScreen()
    {
        rectTransform.sizeDelta = new Vector2(100, 100);
        screenClose = true;
    }

}
