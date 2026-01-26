using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.role;

[GlobalClass]
public partial class IsTarget : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        return gameEventData.Target == subject;
    }
}