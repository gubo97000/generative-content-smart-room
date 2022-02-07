using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


public class DemoInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool handRaise;

    [Header("Movement Settings")]
    public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
        EventManager.TriggerEvent("OnJumpStart", gameObject);
    }

    public void OnCrouch(InputValue value)
    {
        CrouchInput(value.isPressed);
        EventManager.TriggerEvent("OnCrouch", gameObject); // Deprecated, Is called on key press and on key release
        EventManager.TriggerEvent(value.isPressed ? "OnCrouchStart" : "OnCrouchEnd", gameObject);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnHandRaise(InputValue value)
    {
        // HandRaiseInput(value.isPressed);
        EventManager.TriggerEvent("OnHandRaise", gameObject); // Deprecated, Is called on key press and on key release
        EventManager.TriggerEvent(value.isPressed ? "OnHandRaiseStart" : "OnHandRaiseEnd", gameObject);
    }

    public void OnHandsForward(InputValue value)
    {
        // HandsForwardInput(value.isPressed);
        EventManager.TriggerEvent("OnHandsForward", gameObject); // Deprecated, Is called on key press and on key release
        EventManager.TriggerEvent(value.isPressed ? "OnHandsForwardStart" : "OnHandsForwardEnd", gameObject);
    }

    public void OnPanicButton(InputValue value)
    {
        Debug.Log("Reload Scene Pressed");
        EventManager.TriggerEvent("ResetInventory");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
#else
    // old input sys if we do decide to have it (most likely wont)...
#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }
    public void CrouchInput(bool newCrouchState)
    {
        crouch = newCrouchState;
        print("Crouch");
    }
    // public void HandRaiseInput(bool newCrouchState)
    // {
    //     crouch = newCrouchState;
    //     print("HandRaise");
    // }


    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

#if !UNITY_IOS || !UNITY_ANDROID

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

#endif

}

