namespace ArcticRS.Actions;

public enum ActionPriority 
{ 
    Interruptible,   // "Weak"
    Standard,        // "Normal"
    Forceful,        // "Strong"
    Unstoppable      // "Soft"
}