using Godot;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.board.factions;

public static class FactionExt
{
    public static bool CanControl(this Faction faction, GodotObject obj)
    {
        if (obj is not ICharacter pawn) return false;
        return faction.CanControl(pawn);
    }

    public static bool CanControl(this Faction faction, ISubject pawn)
    {
        return pawn.Allegiances?.Contains(faction) ?? false;
    }
}