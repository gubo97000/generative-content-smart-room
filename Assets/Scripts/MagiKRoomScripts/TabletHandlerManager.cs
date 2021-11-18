using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class TabletHandlerManager : MonoBehaviour
{
    protected event UnityAction Handler;

    protected readonly string[] PoolOfSentencies = new string[]{
        "{0} è il tuo turno!",
        "{0} adesso tocca a te!",
        "Forza {0}, giochiamo!",
        "{0} facci vedere quanto sei bravo!",
        "Vediamo, adesso gioca {0}"
    };

    public virtual void Start()
    {
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.ExperienceManagerComunication.MessageHandlerEvent += TabletHandler;
        }
    }

    public virtual void Update()
    {
        Handler?.Invoke();
        Handler = null;
    }

    private void OnDestroy()
    {
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.ExperienceManagerComunication.MessageHandlerEvent -= TabletHandler;
        }
    }

    protected virtual void TabletHandler(string content)
    {
        try
        {
            JObject message = JObject.Parse(content);
            Debug.LogError(message.ToString());
            if (message.TryGetValue("request", out JToken value))
            {
                string type = value.Value<string>("type");
                switch (type)
                {
                    case "setConfiguration":
                        {
                            JObject payload = value.Value<JObject>("payload");
                            Handler += () =>
                            {
                                HandlerConfiguration(payload);
                            };
                            break;
                        }
                    default: break;
                }
            }
            else if (message.TryGetValue("message", out value))
            {
                JObject custom = value.Value<JObject>();
                Handler += () =>
                {
                    ManageCustomCommand(custom);
                };
            }
            else if (message.TryGetValue("event", out value))
            {
                string type = value.Value<string>("type");
                switch (type)
                {
                    case "activityCommand":
                        {
                            string command = value.Value<JObject>("payload").Value<string>("command");
                            CommandMessages commandEnum = (CommandMessages)Enum.Parse(typeof(CommandMessages), command);
                            Handler += () =>
                            {
                                HandlerButton(commandEnum);
                            };
                            break;
                        }
                    case "newPlayerSelected":
                        {
                            int id = value.Value<JObject>("payload").Value<int>("playerId");
                            Debug.Log(id);
                            Handler += () =>
                            {
                                HandlerTurn(id);
                            };
                            break;
                        }
                    default: break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    protected virtual void HandlerButton(CommandMessages command)
    {
    }

    protected virtual void HandlerTurn(int playerName)
    {
    }

    protected virtual void HandlerConfiguration(JObject configuration)
    {
    }

    protected virtual void ManageCustomCommand(JObject command)
    {
    }
}

public enum CommandMessages
{
    pause, play, next, back, skip, repeat, close
}