using ArcticRS.Actions;
using Genesis.Client;
using Genesis.Configuration;
using Genesis.Container;
using Genesis.Environment;
using Genesis.Interactions;
using Genesis.Managers;
using Genesis.Model;
using Genesis.Movement;
using Genesis.Shops;
using Genesis.Skills;
using Genesis.Skills.Combat;

namespace Genesis.Entities;

public class Player : Entity
{
    public bool IsBot { get; set; }

    public int ArriveDelayTicks { get; set; }
    public int NormalDelayTicks { get; set; }
    public bool MovedLastTick { get; set; }
    public bool MovedThisTick { get; set; }
    public bool IsDelayed => ArriveDelayTicks > 0 || NormalDelayTicks > 0;

    public CombatHelper CombatHelper { get; set; }

    public NetworkSession Session { get; set; }
    public PlayerUpdateFlags Flags { get; set; }

    public Player[] LocalPlayers { get; set; } = new Player[255];

    public bool PerformedTeleport { get; set; }
    public ActionHandler ActionHandler { get; set; }

    public PlayerAttributes Attributes { get; set; }
    // public EquipmentManager EquipmentManager { get; set; }
    public ColorManager ColorManager { get; set; }
    public AnimationManager AnimationManager { get; set; }
    public SkillManager SkillManager { get; set; }
    public RSInteraction CurrentInteraction { get; set; }
    public DialogueManager DialogueManager { get; set; }
    public BonusManager BonusManager { get; set; }
    // public Player Following { get; set; }

    public EquipmentContainer Equipment { get; set; } = new EquipmentContainer(11);
    public RSContainer BankContainer { get; set; } = new BankContainer(ServerConfig.BANK_SIZE);
    public InventoryContainer Inventory { get; } = new InventoryContainer(ServerConfig.INVENTORY_SIZE);
    // public RSContainer WindowInventory { get; } = new InventoryContainer(ServerConfig.INVENTORY_SIZE);

    public FightMode FightMode { get; set; } = FightMode.ACCURATE;

    public bool EquippedItem { get; set; }
    
    public Player()
    {
        Session = new NetworkSession(this);
        LoadDefaults();
    }

    private void LoadDefaults()
    {
        Flags = PlayerUpdateFlags.None;
        PlayerMovementHandler = new PlayerMovementHandler(this);
        // EquipmentManager = new EquipmentManager(this);
        CombatHelper = new CombatHelper(this);

        // InventoryManager = new InventoryManager(this);
        // BankManager = new BankManager(this);

        SkillManager = new SkillManager(this);
        DialogueManager = new DialogueManager(this);
        ActionHandler = new ActionHandler(this);
        BonusManager = new BonusManager(this);

        ColorManager = new ColorManager();
        AnimationManager = new AnimationManager();
        Attributes = new PlayerAttributes();
    }

    public void PreProcessTick()
    {
        // Decrement delays at the START of the tick
        if (ArriveDelayTicks > 0) ArriveDelayTicks--;
        if (NormalDelayTicks > 0) NormalDelayTicks--;

        // Update movement history
        MovedLastTick = MovedThisTick;
        MovedThisTick = false;
    }

    public void ProcessMovement()
    {
        // if (CurrentInteraction != null && InteractingEntity != null)
        // {
        //     // if (InteractingEntity is Player player)
        //     // {
        //     //     PlayerMovementHandler.Reset();
        //     //     RSPathfinder.FindPath(this, player.Location.X, player.Location.Y, true, 1, 1);
        //     //     PlayerMovementHandler.Finish();
        //     // }
        // }
        // else
        // {
        //     PlayerMovementHandler.Process();
        // }
        
        PlayerMovementHandler.Process();
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

    public bool canMove(int stepsX, int stepsY)
    {
        return Region.canMove(Location.X, Location.Y, Location.X + stepsX, Location.Y + stepsY, Location.Z, 1, 1);
    }

    public int AnimationDelay { get; set; }

    public override void SetCurrentAnimation(int animationId, int delay = 0)
    {
        CurrentAnimation = animationId;
        AnimationDelay = delay;
        Flags |= PlayerUpdateFlags.Animation;
    }

    public Damage RecentDamage { get; set; }
    public Damage RecentDamage1 { get; set; }

    public void SetDamage(Damage damage)
    {
        RecentDamage = damage;
        SetCurrentAnimation(424); /* Block animation */

        // SetCurrentGfx(damage.Gfx);
        if (CurrentHealth - damage.Amount <= 0)
        {
            CurrentHealth = 0;
            ActionHandler.AddAction(new RespawnAction(this));
        }
        else
        {
            CurrentHealth -= damage.Amount;
        }

        SkillManager.RefreshSkill(SkillType.HITPOINTS);
        Flags |= PlayerUpdateFlags.SingleHit;
    }

    public void SetDoubleDamage(Damage damage1)
    {
        var totalDamage = damage1.Amount;
        
        RecentDamage1 = damage1;
        SetCurrentAnimation(424); /* Block animation */
        if (CurrentHealth - totalDamage <= 0)
        {
            CurrentHealth = 0;
            ActionHandler.AddAction(new RespawnAction(this));
        }
        else
        {
            CurrentHealth -= totalDamage;
        }

        SkillManager.RefreshSkill(SkillType.HITPOINTS);
        Flags |= PlayerUpdateFlags.DoubleHit;
    }

    public override void SetCurrentGfx(Gfx gfx)
    {
        if (gfx == null) return;
        
        CurrentGfx = gfx;
        Flags |= PlayerUpdateFlags.Graphics;
    }

    public Entity InteractingEntity { get; set; }
    public Shop OpenShop { get; set; }

    public void SetFacingEntity(Entity entity)
    {
        InteractingEntity = entity;
        Flags |= PlayerUpdateFlags.InteractingEntity;
    }

    public override void SetFaceX(int x)
    {
        CurrentFaceX = x;
        Flags |= PlayerUpdateFlags.FaceDirection;
    }

    public override void SetFaceY(int y)
    {
        CurrentFaceY = y;
        Flags |= PlayerUpdateFlags.FaceDirection;
    }
    
    
    public void ClearInteraction() => CurrentInteraction = null;

    public void Reset()
    {
        PerformedTeleport = false;
        Flags = PlayerUpdateFlags.None;

        PlayerMovementHandler.PrimaryDirection = -1;
        PlayerMovementHandler.SecondaryDirection = -1;
        PlayerMovementHandler.IsRunning = false;
        PlayerMovementHandler.IsWalking = false;
        PlayerMovementHandler.DiscardMovementQueue = false;

        CurrentAnimation = -1;
        CurrentGfx = null;
    }
}