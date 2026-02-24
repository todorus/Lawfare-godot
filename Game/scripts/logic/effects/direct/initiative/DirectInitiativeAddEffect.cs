using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.logic.inputs.amount;
using Lawfare.scripts.logic.inputs.subject.refs;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.direct.initiative;

[GlobalClass]
public partial class DirectInitiativeAddEffect : Effect
{
    [Export]
    private SubjectRef _target;
    
    [Export]
    private InputLabel _amount;

    public override ChangeGroup[] Stage(GameEvent gameEvent)
    {
        var target = _target.GetValue(gameEvent) as ISubject;
        var amountInput = gameEvent.Inputs[_amount] as AmountInput;
        var amount = amountInput?.GetValue(gameEvent) as int? ?? 0;
        
        IDiff[] changes = Initiative.MoveEntity(gameEvent.Context, target as IHasInitiative, amount)
            .Cast<IDiff>().ToArray();
        var changeGroup = changes.ToChangeGroup();
        
        return [changeGroup];
    }
}