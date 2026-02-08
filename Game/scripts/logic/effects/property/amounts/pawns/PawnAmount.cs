using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.effects.property.amounts.pawns;

[GlobalClass]
public partial class PawnAmount : AmountProvider
{
    [Export] public SubjectCondition[] SubjectConditions = [];

    protected override int Count(GameEvent gameEvent, ISubject subject)
    {
        return gameEvent.Subjects
            .Where(subject => subject is ICharacter)
            .Count(pawn => SubjectConditions.All(condition => condition.Evaluate(gameEvent, pawn)));
    }
}