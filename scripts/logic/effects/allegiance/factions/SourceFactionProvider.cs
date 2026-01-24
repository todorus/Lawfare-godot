using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.effects.allegiance.factions;

[GlobalClass]
public partial class SourceFactionProvider : FactionProvider
{
    public override Faction GetFaction(GameEvent gameEvent)
    {
        return gameEvent.Faction;
    }
}