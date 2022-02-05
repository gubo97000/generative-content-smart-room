using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpSystem : MonoBehaviour
{
    public GameObject popUpBox;
    private Animator animator;
    public TMP_Text popUpText;

    private bool isHandRaiseEnabled;
    public bool isWinCanvas;

    private MagicRoomAppliancesManager _aM = null;
    
    // Zero if not timed
    public float timer = 0f;
    private float elapsed = 0;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (isWinCanvas && MagicRoomManager.instance?.MagicRoomAppliancesManager != null)
        {
            _aM = MagicRoomManager.instance.MagicRoomAppliancesManager;
        }

        PopUp();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (timer > 0f && elapsed > timer)
        {
            if (animator.GetBool("check1") == false) {
                animator.SetBool("check1", true);
                elapsed = 0;
            }
            else
                animator.SetBool("check2", true);
        }

        if (animator.GetBool("check1") == true && animator.GetBool("check2") == true)
        {
            animator.SetTrigger("close");
            StartCoroutine(GetNextTutorial());
        }

    }

    public void PopUp(/*string text*/)
    {
        //popUpBox.SetActive(true);
        //popUpText.text = text;
        animator.SetTrigger("pop");
        if (isWinCanvas)
        {
            _aM?.SendChangeCommand("Macchina delle Bolle", "ON", 3);
            Debug.Log("Evviva le bolle!!!");
        }
    }

    void OnHandRaise(EventDict dict)
    {
        if (isHandRaiseEnabled)
            animator.SetBool(((GameObject)dict["sender"]).tag == "Player1" ? "check1" : "check2", true);
    }

    IEnumerator GetNextTutorial()
    {
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent("NextTutorial", gameObject);
    }

}
