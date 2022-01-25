using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpSystem : MonoBehaviour
{
    public GameObject popUpBox;
    private Animator animator;
    public TMP_Text popUpText;

    void Start()
    {
        animator = GetComponent<Animator>();

        //EventManager.StartListening("Check1", Check1);
        //EventManager.StartListening("Check2", Check2);
        PopUp();
    }

    void OnDestroy()
    {
        //EventManager.StopListening("Check1", Check1);
        //EventManager.StopListening("Check2", Check2);
    }

    public void PopUp(/*string text*/)
    {
        //popUpBox.SetActive(true);
        //popUpText.text = text;
        animator.SetTrigger("pop");
    }

    /*public void Check1(EventDict dict)
    {
        animator.SetTrigger("check1");
    }

    public void Check2(EventDict dict)
    {
        animator.SetTrigger("check2");
    }*/
}
