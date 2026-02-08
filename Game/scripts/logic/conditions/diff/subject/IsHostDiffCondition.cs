using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.conditions.subject.role;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.diff.subject;

[GlobalClass]
public partial class IsHostDiffCondition : SubjectDiffCondition
{
    private SubjectCondition _hostCondition = new IsHost();
    
    protected override bool Evaluate(GameEvent gameEvent, ISubject subject)
    {
        return _hostCondition.Evaluate(gameEvent, subject);
    }
}