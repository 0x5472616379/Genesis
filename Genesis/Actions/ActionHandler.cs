namespace ArcticRS.Actions;

public class ActionHandler
{
    public List<RSAction> Actions { get; set; } = new();

    public ActionHandler()
    {
        
    }

    public void AddAction(RSAction action)
    {
        Actions.Add(action);
    }
    
    public void ProcessActions()
    {
        foreach (var rsAction in Actions.ToList())
        {
            var valid = rsAction.ExecuteAction();
            if (!valid) continue;
            
            Actions.Remove(rsAction);
        }   
    }
    
    public bool ContainsActionOfCategory(ActionCategory category)
    {
        return Actions.Any(x => x.Category == category);
    }
    
}