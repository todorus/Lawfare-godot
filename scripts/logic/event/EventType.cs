namespace Lawfare.scripts.logic.@event;

public enum EventType
{
    Enter,
    Leave,
    BeforeAction,
    Action,
    AfterAction,
    StartTurn,
    EndTurn,
    Assignment
}

public static class EventTypeExtensions
{
    public static bool ShouldRunTriggers(this GameEvent gameEvent)
    {
        return gameEvent.Type switch
        {
            EventType.Action => false,
            _ => true
        };
    }

    public static bool ShouldRollDice(this GameEvent gameEvent)
    {
        return gameEvent.Type switch
        {
            EventType.Action => true,
            _ => false
        };
    }

    public static bool ShouldRunAction(this GameEvent gameEvent)
    {
        return gameEvent.Type switch
        {
            EventType.Action => true,
            _ => false
        };
    }
}