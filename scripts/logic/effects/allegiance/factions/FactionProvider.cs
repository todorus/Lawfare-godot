using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.effects.allegiance.factions;

[GlobalClass]
public abstract partial class FactionProvider : Resource
{
    public abstract Faction GetFaction(GameEvent gameEvent);
}