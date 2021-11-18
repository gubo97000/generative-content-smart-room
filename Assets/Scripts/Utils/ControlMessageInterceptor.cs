using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class ControlMessageInterceptor : TabletHandlerManager
{
    public static Action Pause;
    public static Action Skip;
    public static Action Next;
    public static Action Back;
    public static Action Repeat;

    public static Action<int> ReceiveNewPlayer;

    protected override void HandlerButton(CommandMessages command)
    {
        switch (command)
        {
            case CommandMessages.pause:
                Time.timeScale = 0;
                break;

            case CommandMessages.play:
                Time.timeScale = 1;
                break;

            case CommandMessages.next:
                Next?.Invoke();
                break;

            case CommandMessages.back:
                Back?.Invoke();
                break;

            case CommandMessages.skip:
                Skip?.Invoke();
                break;

            case CommandMessages.repeat:
                Repeat?.Invoke();
                break;

            case CommandMessages.close:
                Application.Quit();
                break;
        }
    }

    protected override void ManageCustomCommand(JObject command)
    {
        print(command);
    }

    protected override void HandlerTurn(int playerName)
    {
        print(playerName);
        ReceiveNewPlayer?.Invoke(playerName);
    }
}