using Godot;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.diff.subject;

[GlobalClass]
public abstract partial class SubjectDiffCondition : DiffCondition
{
    public override bool Evaluate(GameEvent gameEventData, IDiff diff)
    {
        return Evaluate(gameEventData, diff.Subject);
    }
    
    protected abstract bool Evaluate(GameEvent gameEvent, ISubject subject);
}