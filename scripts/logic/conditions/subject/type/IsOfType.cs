using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.type;

[GlobalClass]
public partial class IsOfType : SubjectCondition
{
    [Export] private SubjectType _type;

    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        switch (_type)
        {
            case SubjectType.BoardSpace:
                return subject is ISubject;
            case SubjectType.Pawn:
                return subject is ICharacter;
            default:
                return false;
        }
    }
}