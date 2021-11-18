using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class HintPanelManagement : MonoBehaviour
{
    private Text hinttext;
    private Animator anim;

    public static HintPanelManagement instance;
    private bool endspeak;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector2(Screen.width * 2, Screen.height * 2);
        hinttext = GetComponentInChildren<Text>();
        anim = transform.parent.GetComponent<Animator>();
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.MagicRoomTextToSpeachManager.EndSpeak += EndSpeak;
        }
    }

    private void EndSpeak()
    {
        endspeak = true;
    }

    public void StartHint(string textOfHint, float durationOfHint, Direction enteringside, float distanceOnTheEnteringSide, Characters helper = Characters.Emi)
    {
        hinttext.text = "";
        StartCoroutine(ShowHintTimed(textOfHint, durationOfHint * 0.1f, durationOfHint * 0.8f, durationOfHint * 0.1f, distanceOnTheEnteringSide, enteringside, helper));
    }

    private IEnumerator ShowHintTimed(string textOfHint, float durationintro, float durationhint, float durationout, float distanceOnTheEnteringSide, Direction enteringside, Characters helper)
    {
        distanceOnTheEnteringSide = Mathf.Min(1, Mathf.Max(0, distanceOnTheEnteringSide));
        Axis direction = (enteringside == Direction.Down || enteringside == Direction.Up) ? Axis.X : Axis.Y;
        Vector2 blocksize = GetComponent<RectTransform>().sizeDelta * GetComponent<RectTransform>().localScale;
        if (enteringside == Direction.Up || enteringside == Direction.Down)
        {
            if (distanceOnTheEnteringSide < (20 + blocksize.x / 2) / Screen.width)
            {
                distanceOnTheEnteringSide = (20 + blocksize.x / 2) / Screen.width;
            }
            if (distanceOnTheEnteringSide > (Screen.width - 20 - (blocksize.x / 2)) / Screen.width)
            {
                distanceOnTheEnteringSide = (Screen.width - 20 - (blocksize.x / 2)) / Screen.width;
            }
        }
        if (enteringside == Direction.Left || enteringside == Direction.Right)
        {
            if (distanceOnTheEnteringSide < (blocksize.y / 2 + 20) / Screen.height)
            {
                distanceOnTheEnteringSide = (blocksize.y / 2 + 20) / Screen.height;
            }
            if (distanceOnTheEnteringSide > (Screen.height - 20 - (blocksize.y / 2)) / Screen.height)
            {
                distanceOnTheEnteringSide = (Screen.height - 20 - (blocksize.y / 2)) / Screen.height;
            }
        }
        yield return EnterField(distanceOnTheEnteringSide, direction, enteringside, durationintro);
        hinttext.text = textOfHint;
        anim.SetBool("Emi", true);//helper.ToString(), true);
        endspeak = false;
        float starttime = Time.time;
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.MagicRoomTextToSpeachManager.GenerateAudioFromText(textOfHint);
            yield return new WaitUntil(() => endspeak == true);
        }
        else
        {
            yield return new WaitForSeconds(5);
        }
        anim.SetBool("Emi", false); //anim.SetBool(helper.ToString(), false);
        if (Time.time - starttime < durationhint)
        {
            yield return new WaitForSeconds(durationhint - (Time.time - starttime));
        }
        yield return ExitField(distanceOnTheEnteringSide, direction, enteringside, durationout);
    }

    private void Resize(Vector2 scale)
    {
        GetComponent<RectTransform>().localScale = scale;
    }

    private IEnumerator EnterField(float distance, Axis direction, Direction motiondirection, float duration)
    {
        Vector2 blocksize = GetComponent<RectTransform>().sizeDelta * GetComponent<RectTransform>().localScale;

        float modifier = (motiondirection == Direction.Down || motiondirection == Direction.Right) ? 1.3f : -0.3f;
        float secondModifier = motiondirection == Direction.Down ? Screen.height - blocksize.y / 2 : (blocksize.y / 2) + 20;
        float thirdModifier = motiondirection == Direction.Right ? Screen.width - blocksize.x / 2 : blocksize.x / 2;
        Vector2 endpositon = direction == Axis.X ? new Vector2(distance * Screen.width, secondModifier) : new Vector2(thirdModifier, distance * Screen.height);
        transform.position = direction == Axis.X ? new Vector2(distance * Screen.width, Screen.height * modifier) : new Vector2(modifier * Screen.width, Screen.height * distance);
        Vector2 startpositon = transform.position;
        do
        {
            yield return new WaitForEndOfFrame();
            float deltatime = Time.deltaTime;
            float percentile = duration / deltatime;
            transform.position += (Vector3)(endpositon - startpositon) / percentile;
        } while (Vector3.Distance(transform.position, endpositon) > 10);
    }

    private IEnumerator ExitField(float distance, Axis direction, Direction motiondirection, float duration)
    {
        Vector2 blocksize = GetComponent<RectTransform>().sizeDelta * GetComponent<RectTransform>().localScale;

        float modifier = (motiondirection == Direction.Down || motiondirection == Direction.Left) ? 1.3f : -0.3f;
        float secondModifier = motiondirection == Direction.Down ? Screen.height + blocksize.y / 2 : -(blocksize.y / 2) - 20;
        float thirdModifier = motiondirection == Direction.Right ? Screen.width + blocksize.x / 2 : -blocksize.x / 2;
        Vector2 endpositon = direction == Axis.X ? new Vector2(distance * Screen.width, secondModifier) : new Vector2(thirdModifier, distance * Screen.height);
        Vector2 startpositon = transform.position;
        do
        {
            yield return new WaitForEndOfFrame();
            float deltatime = Time.deltaTime;
            float percentile = duration / deltatime;
            transform.position += (Vector3)(endpositon - startpositon) / percentile;
        } while (Vector3.Distance(transform.position, endpositon) > 10);
        transform.position = new Vector2(Screen.width * 2, Screen.height * 2);
    }
}

public enum Characters
{
    Emi, Vis, Gus, Olfo, Auri, Tati
}

public enum Direction
{
    Up, Down, Left, Right
}