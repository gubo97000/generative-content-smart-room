using UnityEngine;
using UnityEngine.UI;

public class PlayerPersonalPanel : MonoBehaviour
{
    public Image avatar;
    public Image BackgroundImage;
    public Text Name;
    public GameObject Pointprefab;
    public GameObject Pointholder;
    public GameObject pointtext;

    [Range(0, 100)]
    public float pointdistance = 5;

    public bool hasBeenCalled = false;
    private int threshold = -1;

    private bool activeplayer;
    private Color backgroundColor;
    private int score;
    private bool compactHolder = false;

    private float pointgridsize;

    public void SetUp(Player p)
    {
        avatar.sprite = Base64ToTexture(p.avatar);
        Name.text = p.name + " " + p.surname;
        score = 0;
        if (Pointprefab == null)
        {
            pointtext.GetComponent<Text>().text = "0";
        }

        ScoringElement.ScoreHandlerEvent += addPoint;
    }

    private void Update()
    {
        if (activeplayer)
        {
            if (transform.GetChild(0).localScale.x < 1.09f)
            {
                transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, Vector3.one * 1.1f, Time.deltaTime);
            }
        }
        else
        {
            if (transform.GetChild(0).localScale.x > 0.82f)
            {
                transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, Vector3.one * 0.8f, Time.deltaTime);
            }
        }

        BackgroundImage.color = Color.Lerp(BackgroundImage.color, backgroundColor, Time.deltaTime);

        if (Input.GetKeyUp(KeyCode.Return))
        {
            addPoint("");
        }
    }

    public void setAsActivePlayer(bool isActive, Color activeColor)
    {
        activeplayer = isActive;
        backgroundColor = activeColor;
        if (isActive)
        {
            hasBeenCalled = true;
        }
    }

    public void addPoint(object parameters)
    {
        if (activeplayer)
        {
            score++;
            if (Pointprefab == null)
            {
                pointtext.GetComponent<Text>().text = score.ToString();
            }
            else
            {
                if (threshold == -1)
                {
                    GridLayoutGroup grid = Pointholder.GetComponent<GridLayoutGroup>();
                    pointgridsize = Pointholder.GetComponent<RectTransform>().rect.height;
                    threshold = Mathf.FloorToInt(Pointholder.GetComponent<RectTransform>().rect.width / (pointgridsize * (1 + pointdistance / 100)));
                    grid.cellSize = Vector2.one * pointgridsize;
                    grid.spacing = Vector2.right * pointgridsize * pointdistance / 100;
                }
                if (Pointholder.transform.childCount < threshold && !compactHolder)
                {
                    GameObject g = GameObject.Instantiate(Pointprefab, Pointholder.transform);
                    if (g.GetComponent<PointShower>() != null)
                    {
                        g.GetComponent<PointShower>().setUp(parameters);
                    }
                }
                else
                {
                    if (!compactHolder)
                    {
                        compactHolder = true;
                        int lenght = Pointholder.transform.childCount;
                        for (int i = 1; i < lenght; i++)
                        {
                            GameObject.DestroyImmediate(Pointholder.transform.GetChild(1).gameObject);
                        }
                        Pointholder.GetComponent<GridLayoutGroup>().padding.left = (int)(pointgridsize * 1.8f);
                        pointtext.GetComponent<Text>().text = score.ToString() + "X";
                        compactHolder = true;
                    }
                    else
                    {
                        pointtext.GetComponent<Text>().text = score.ToString() + "X";
                    }
                }
            }
        }
    }

    private Sprite Base64ToTexture(string base64)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64);
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        return sprite;
    }
}