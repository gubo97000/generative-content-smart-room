using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWallGesture : MonoBehaviour
{
    public Animator animator;
    public bool isPlayer1;
    public bool isDragTutorial;

    void OnMouseDown()
    {
        if (!isDragTutorial)
        {
            animator.SetBool(isPlayer1 ? "check1" : "check2", true);
            Debug.Log(isPlayer1 ? "check1" : "check2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("check1") == true && animator.GetBool("check2") == true)
        {
            animator.SetTrigger("close");
            StartCoroutine(GetNextTutorial());
        }
    }

    IEnumerator GetNextTutorial()
    {
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent("NextTutorial", gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Apple" && isDragTutorial)
        {
            // Assumption: player 1 drags ball from the left side into the circle, and player 2 does vice versa
            // But even if the opposite happens, I made sure the tutorial does not loop forever, with this if-elseif-else
            if(animator.GetBool("check1") == true)
                animator.SetBool("check2", true);
            else if (animator.GetBool("check2") == true)
                animator.SetBool("check1", true);
            else
                animator.SetBool(other.transform.position.x > transform.position.x ? "check1" : "check2", true);

            Destroy(other.gameObject);
        }
    }
}
