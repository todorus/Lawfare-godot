namespace Lawfare.scripts.logic.effects;

public struct ChangeGroup(IChange[] changes)
{
    public readonly IChange[] Changes = changes ?? [];
    public bool Cancelled = false;
}

public static class ChangeGroupExtensions
{
    public static ChangeGroup ToChangeGroup(this IChange change)
    {
        return new ChangeGroup([change]);
    }

    public static ChangeGroup ToChangeGroup(this IChange[] changes)
    {
        return new ChangeGroup(changes);
    }
}