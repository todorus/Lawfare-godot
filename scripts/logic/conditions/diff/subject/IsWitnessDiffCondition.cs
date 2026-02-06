using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.conditions.subject.type;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.diff.subject;

[GlobalClass]
public partial class IsWitnessDiffCondition : SubjectDiffCondition
{
    private SubjectCondition _witnessCondition = new IsWitness();
    
    protected override bool Evaluate(GameEvent gameEvent, ISubject subject)
    {
        return _witnessCondition.Evaluate(gameEvent, subject);
    }
}