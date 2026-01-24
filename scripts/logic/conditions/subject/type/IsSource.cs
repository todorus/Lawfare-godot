using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.type;

[GlobalClass]
public partial class IsSource : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        return gameEventData.Source == subject;
    }
}