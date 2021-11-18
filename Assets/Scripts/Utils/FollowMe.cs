using UnityEngine;
using UnityEngine.UI;

public class FollowMe : MonoBehaviour
{
    public GameObject tofollow;

    /// <summary>
    /// reference to the camera for the floor screen
    /// </summary>
    public Camera floorcamera;

    /// <summary>
    /// reference to the canvas on which the gameobject is placed on
    /// </summary>
    private GameObject Canvas;

    public float offset = 150;

    /// <summary>
    /// transform of the canvas
    /// </summary>
    private RectTransform CanvasRect;

    // Use this for initialization
    private void Start()
    {
        Canvas = transform.parent.parent.gameObject;
        if (Canvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
        {
            floorcamera = Canvas.GetComponent<Canvas>().worldCamera;
        }
        CanvasRect = Canvas.GetComponent<RectTransform>();
        if (tofollow == null)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// percentage of completement of the loading bar
    /// </summary>
    private float amount = 0;

    // Update is called once per frame
    private void Update()
    {
        Vector2 ViewportPosition = floorcamera.WorldToViewportPoint(tofollow.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        gameObject.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition + Vector2.up * offset;
        if (amount >= 0.99f || amount <= 0)
        {
            gameObject.GetComponent<Image>().fillAmount = 0;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            gameObject.GetComponent<Image>().fillAmount = amount;
        }
    }

    /// <summary>
    /// set the percentage of fillup
    /// </summary>
    /// <param name="percentage"></param>
    public void insidethecard(float percentage)
    {
        amount = percentage;
    }

    /// <summary>
    /// empty the loading bar
    /// </summary>
    public void exitthecard()
    {
        amount = 0;
    }
}