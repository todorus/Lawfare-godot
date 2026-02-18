using Godot;
using Lawfare.scripts.logic.effects.direct.property;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.inputs;
using Lawfare.scripts.logic.inputs.amount;
using Lawfare.scripts.logic.inputs.subject.refs;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.transfer;

[GlobalClass]
public partial class PropertyTransferEffect : Effect
{
    [ExportGroup("From")]
    [Export]
    private SubjectRef _from;
    
    [Export]
    private Property _fromProperty;
    
    [Export]
    private InputLabel _fromAmount;
    
    [ExportGroup("To")]
    [Export]
    private SubjectRef _to;
    
    [Export]
    private Property _toProperty;
    
    [Export]
    private InputLabel _toAmount;
    
    public override ChangeGroup[] Stage(GameEvent gameEvent)
    {
        var fromDiff = Stage(gameEvent, _from, _fromProperty, _fromAmount);
        var toDiff = Stage(gameEvent, _to, _toProperty, _toAmount);
        var changeGroup = new ChangeGroup([fromDiff, toDiff]);
        return [changeGroup];
    }
    
    private IDiff Stage(GameEvent gameEvent, SubjectRef subjectRef, Property property, InputLabel input)
    {
        var subject = subjectRef.GetValue(gameEvent) as ISubject;
        var amountInput = gameEvent.Inputs[input] as AmountInput;
        var amount = amountInput?.GetValue(gameEvent) as int? ?? 0;
        
        var oldAmount = subject.Quantities.GetValue(property);
        var newAmount = subject.Quantities.StageAdd(property, amount);
        return new PropertyDiff(
            subject,
            new Quantity
            {
                Property = property,
                Amount = oldAmount
            },
            new Quantity
            {
                Property = property,
                Amount = newAmount
            }
        );
    }
}