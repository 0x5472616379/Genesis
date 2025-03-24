namespace Genesis.Definitions;

public class ItemAnimations
{
    public int Id { get; set; }
    public Animations Animations { get; set; }
}

public class Animations
{
    public int BlockAnim { get; set; }
    public int StandAnim { get; set; }
    public int WalkAnim { get; set; }
    public int RunAnim { get; set; }
    public int StandTurnAnim { get; set; }
    public int Turn180Anim { get; set; }
    public int Turn90CWAnim { get; set; }
    public int Turn90CCWAnim { get; set; }
}