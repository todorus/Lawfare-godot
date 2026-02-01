using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.characters.lawyers;

[GlobalClass]
public partial class ElicitStatement : Resource
{
    [Export]
    public SubjectCondition[] WitnessConditions;
    
    [Export]
    public SubjectCondition[] LawyerConditions;

    public bool Evaluate(GameEvent gameEvent, Lawyer lawyer, Witness witness)
    {
        return WitnessConditions.All(condition => condition.Evaluate(gameEvent, witness))
            && LawyerConditions.All(condition => condition.Evaluate(gameEvent, lawyer));
    }
}