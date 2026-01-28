using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.relation.faction;

[GlobalClass]
public partial class OwnTeamFactionProvider : FactionProvider
{
    public override Faction GetFaction(GameEvent gameEvent, ISubject subject)
    {
        var ownFaction = gameEvent.Context.GetTeam(subject)?.Faction;
        return gameEvent.Context
            .Factions.FirstOrDefault(faction => faction == ownFaction);
    }
}