using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class SimulationAnimatorDict : SerializableDictionary<KeyCode, string> { }

public class PlayerMovementSimultor : MonoBehaviour
{
    public Skeleton skeleton;

    [Range(1, 6)]
    public int id;

    [Range(1, 10f)]
    public float movementSpeed = 5f;

    [Range(1, 10)]
    public float rotationSpeed = 1f;

    private Animator anim;
    private Vector3 reversedstandardizedFloorSize;
    private Vector3 sensorDisallinment;

    public SimulationAnimatorDict animationkeys;

    private bool followmouse = false;
    private Transform arm, forearm, hand;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (MagicRoomManager.instance.systemConfiguration != null)
        {
            sensorDisallinment = new Vector3(MagicRoomManager.instance.systemConfiguration.floorOffsetX, MagicRoomManager.instance.systemConfiguration.floorOffsetY, 0);
            Debug.Log(MagicRoomManager.instance.systemConfiguration.floorSizeX);
            Debug.Log(MagicRoomManager.instance.systemConfiguration.floorSizeY);
            reversedstandardizedFloorSize = new Vector3(MagicRoomManager.instance.systemConfiguration.floorSizeX * 0.065934f, 1, -MagicRoomManager.instance.systemConfiguration.floorSizeY * 0.125f);
        }
        arm = transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0);//transform.Find("mixamorig:RightArm");
        forearm = transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0);//transform.Find("mixamorig:RightForeArm");
        hand = transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);//transform.Find("mixamorig:RightHand");
    }

    public void Setup(int id)
    {
        this.id = id;
        Color col = Color.clear;
        switch (id)
        {
            case 1:
                col = Color.red;
                break;

            case 2:
                col = Color.blue;
                break;

            case 3:
                col = Color.green;
                break;

            case 4:
                col = new Color(1, 1, 0);
                break;

            case 5:
                col = new Color(1, 0, 1);
                break;

            case 6:
                col = new Color(0, 1, 1);
                break;
        }
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = col;
        transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = col;
    }

    private void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                //your code here
                //Debug.Log("pressed " + vKey);
                if (animationkeys.Keys.Contains(vKey))
                {
                    anim.SetTrigger(animationkeys[vKey]);
                }
            }
        }

        updateSkeleton();
        if (Input.GetKey(id.ToString()))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                anim.SetBool("Walk", true);
                transform.position += transform.forward * movementSpeed / 100;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                anim.SetBool("Walk", true);
                transform.position += transform.forward * -movementSpeed / 100;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                anim.SetBool("Walk", true);
                transform.RotateAround(transform.position, Vector3.up, rotationSpeed);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                anim.SetBool("Walk", true);
                transform.RotateAround(transform.position, Vector3.up, -rotationSpeed);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                anim.SetBool("Walk", false);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                List<string> gestures = new List<string>();
                gestures.AddRange(skeleton.Gestures);
                gestures.Add("CLOSEHANDLEFT");
                skeleton.Gestures = gestures.ToArray();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                List<string> gestures = new List<string>();
                gestures.AddRange(skeleton.Gestures);
                if (gestures.Contains("CLOSEHANDLEFT"))
                {
                    gestures.Remove("CLOSEHANDLEFT");
                    skeleton.Gestures = gestures.ToArray();
                }
            }
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                List<string> gestures = new List<string>();
                gestures.AddRange(skeleton.Gestures);
                gestures.Add("CLOSEHANDRIGHT");
                skeleton.Gestures = gestures.ToArray();
            }
            if (Input.GetKeyUp(KeyCode.RightShift))
            {
                List<string> gestures = new List<string>();
                gestures.AddRange(skeleton.Gestures);
                if (gestures.Contains("CLOSEHANDRIGHT"))
                {
                    gestures.Remove("CLOSEHANDRIGHT");
                    skeleton.Gestures = gestures.ToArray();
                }
            }

            if (Input.GetKeyUp(KeyCode.CapsLock))
            {
                followmouse = !followmouse;
            }
            if (followmouse)
            {
                anim.enabled = false;
                Vector2 mouseposition = Input.mousePosition;
                Vector2 screensize = new Vector2(Screen.width, Screen.height);

                hand.localRotation = Quaternion.Lerp(hand.localRotation, Quaternion.Euler(88, -7.64f, -11.6f), Time.deltaTime * 3);
                float rotationvalue = 0;
                rotationvalue = Mathf.Atan2(mouseposition.y - (screensize.y / 2), mouseposition.x - (screensize.x / 2)) * Mathf.Rad2Deg;
                if (mouseposition.x < screensize.x / 2)
                {
                    if (mouseposition.y < screensize.y / 2)
                    {
                        //3 quad
                        arm.rotation = Quaternion.Lerp(arm.rotation, Quaternion.Euler(-36.82f, -100.9f, -64.5f), Time.deltaTime * 3);
                        forearm.localRotation = Quaternion.Lerp(forearm.localRotation, Quaternion.Euler(-16.8f, -15.5f, rotationvalue + 180), Time.deltaTime * 3);
                    }
                    else
                    {
                        //2 quad
                        arm.rotation = Quaternion.Lerp(arm.rotation, Quaternion.Euler(-82f, -130f, -41.9f), Time.deltaTime * 3);
                        forearm.localRotation = Quaternion.Lerp(forearm.localRotation, Quaternion.Euler(-16.8f, -15.5f, (rotationvalue + 270)), Time.deltaTime * 3);
                    }
                }
                else
                {
                    if (mouseposition.y < screensize.y / 2)
                    {
                        //4 quad
                        arm.rotation = Quaternion.Lerp(arm.rotation, Quaternion.Euler(1.95f, 24.2f, -60.2f), Time.deltaTime * 3);
                        forearm.localRotation = Quaternion.Lerp(forearm.localRotation, Quaternion.Euler(-16f, -69f, rotationvalue), Time.deltaTime * 3);
                    }
                    else
                    {
                        //1 quad
                        arm.rotation = Quaternion.Lerp(arm.rotation, Quaternion.Euler(-67.2f, 14.6f, -50.2f), Time.deltaTime * 3);
                        forearm.localRotation = Quaternion.Lerp(forearm.localRotation, Quaternion.Euler(-16f, -61.59f, -rotationvalue), Time.deltaTime * 3);
                    }
                }
            }
            else
            {
                anim.enabled = true;
            }
        }
    }

    private void updateSkeleton()
    {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            Vector3 pos = child.position;
            pos = Vector3.Scale(pos, reversedstandardizedFloorSize);
            pos += Vector3.forward * 1.4f;
            pos = pos + sensorDisallinment;

            skeleton.SetPropertyValue(child.gameObject.name, pos);
        }
    }
}