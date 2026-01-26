using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.type;

[GlobalClass]
public partial class IsLawyer : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        return subject is Lawyer;
    }
}