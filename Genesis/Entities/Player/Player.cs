using Genesis.Client;
using Genesis.Configuration;
using Genesis.Managers;
using Genesis.Movement;

namespace Genesis.Entities;

public class Player : Entity
{
    public NetworkSession Session { get; set; }
    public PlayerUpdateFlags Flags { get; set; }
    
    public Player[] LocalPlayers { get; set; } = new Player[255];

    public bool PerformedTeleport { get; set; }
    public MovementHandler MovementHandler { get; set; }
    
    public PlayerAttributes Attributes { get; set; }
    public EquipmentManager EquipmentManager { get; set; }
    public ColorManager ColorManager { get; set; }
    public AnimationManager AnimationManager { get; set; }

    public Player()
    {
        Session = new NetworkSession(this);
        LoadDefaults();
    }

    private void LoadDefaults()
    {
        Flags = PlayerUpdateFlags.None;
        MovementHandler = new MovementHandler(this);
        EquipmentManager = new EquipmentManager(this);
        ColorManager = new ColorManager();
        AnimationManager = new AnimationManager();
        Attributes = new PlayerAttributes();
    }

    public void AddLocalPlayer(Entity player)
    {
        for (int i = 0; i < LocalPlayers.Length; i++)
        {
            if (LocalPlayers[i] == null)
            {
                LocalPlayers[i] = (Player)player;
                break;
            }
        }
    }
    
    public void RemoveLocalPlayer(Entity player)
    {
        for (int i = 0; i < LocalPlayers.Length; i++)
        {
            if (LocalPlayers[i] == player)
            {
                //override the slot of the player disconnecting and compress
                for (int j = i; j < LocalPlayers.Length - 1; j++)
                    LocalPlayers[j] = LocalPlayers[j + 1];

                break;
            }
        }
    }
    
    public void Reset()
    {
        PerformedTeleport = false;
        Flags = PlayerUpdateFlags.None;
        
        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        MovementHandler.IsRunning = false;
        MovementHandler.IsWalking = false;
        MovementHandler.DiscardMovementQueue = false;
    }
}