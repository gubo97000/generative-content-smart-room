using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpSystem : MonoBehaviour
{
    public GameObject popUpBox;
    private Animator animator;
    public TMP_Text popUpText;

    public bool isHandRaiseEnabled;

    void Start()
    {
        animator = GetComponent<Animator>();

        EventManager.StartListening("OnHandRaise", OnHandRaise);
        PopUp();
    }

    void Update()
    {
        if (animator.GetBool("check1") == true && animator.GetBool("check2") == true)
        {
            animator.SetTrigger("close");
            StartCoroutine(GetNextTutorial());
        }
    }

    void OnDestroy()
    {
        EventManager.StartListening("OnHandRaise", OnHandRaise);
    }

    public void PopUp(/*string text*/)
    {
        //popUpBox.SetActive(true);
        //popUpText.text = text;
        animator.SetTrigger("pop");
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
