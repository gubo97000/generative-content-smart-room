using UnityEngine;

public class TurnDealer : TabletHandlerManager
{
    private PlayerPanelManager panel;

    public override void Start()
    {
        base.Start();
        panel = GetComponent<PlayerPanelManager>();
    }

    protected override void HandlerTurn(int playerName)
    {
        Player p = GameSetting.instance.players.Find(x => x.id == playerName);
        panel.setActivePlayer(p.id);

        string sentence = PoolOfSentencies[Random.Range(0, PoolOfSentencies.Length)];
        sentence = sentence.Replace("{0}", p.phonema);
        MagicRoomManager.instance.MagicRoomTextToSpeachManager.GenerateAudioFromText(sentence);
    }
}