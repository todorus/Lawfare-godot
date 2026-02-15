using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.logic.inputs.amount;
using Lawfare.scripts.logic.inputs.subject.refs;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.direct;

[GlobalClass]
public abstract partial class DirectEffect : Effect
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
        var changes = Stage(target, amount);
        var changeGroup = changes.ToChangeGroup();
        return [changeGroup];
    }
    
    public abstract IDiff[] Stage(ISubject subject, int value);
}