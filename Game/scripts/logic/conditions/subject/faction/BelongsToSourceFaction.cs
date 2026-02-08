using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.faction;

[GlobalClass]
public partial class BelongsToSourceFaction : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        return subject.CanHaveFaction && subject.Allegiances.Contains(gameEventData.Faction);
    }
}