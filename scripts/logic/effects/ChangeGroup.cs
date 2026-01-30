namespace Lawfare.scripts.logic.effects;

public struct ChangeGroup(IDiff[] diffs)
{
    public readonly IDiff[] Diffs = diffs ?? [];
    public bool Cancelled = false;
}

public static class ChangeGroupExtensions
{
    public static ChangeGroup ToChangeGroup(this IDiff change)
    {
        return new ChangeGroup([change]);
    }

    public static ChangeGroup ToChangeGroup(this IDiff[] changes)
    {
        return new ChangeGroup(changes);
    }
}