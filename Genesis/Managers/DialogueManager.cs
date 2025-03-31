using Genesis.Entities.Player;

namespace Genesis.Managers;

public class DialogueManager
{
    private readonly Player _player;
    public int NextDialogue { get; set; } = -1;
    public DialogueManager(Player player)
    {
        _player = player;
    }
}