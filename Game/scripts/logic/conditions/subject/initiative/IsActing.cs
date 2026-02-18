using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.initiative;

[GlobalClass]
public partial class IsActing : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        var state = gameEventData.Context.InitiativeTrack;
        return Initiative.GetCurrent(state) == subject;
    }
}