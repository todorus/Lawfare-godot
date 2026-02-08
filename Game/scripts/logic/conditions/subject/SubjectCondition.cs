using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject;

[GlobalClass]
public abstract partial class SubjectCondition : Resource
{
    public abstract bool Evaluate(GameEvent gameEventData, ISubject subject);
}