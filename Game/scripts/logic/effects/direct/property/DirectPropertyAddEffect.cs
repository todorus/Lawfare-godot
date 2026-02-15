using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using AmountProvider = Lawfare.scripts.logic.effects.property.amounts.AmountProvider;

namespace Lawfare.scripts.logic.effects.direct.property;

[GlobalClass]
public partial class DirectPropertyAddEffect : DirectEffect
{
    [Export] 
    private Property _property;
    
    public override IDiff[] Stage(ISubject subject, int amount)
    {
        if (subject?.Quantities == null)
        {
            GD.PushWarning("PropertyAddChange.Apply: Subject or Subject.Quantities is null");
            var zeroQuantity = new Quantity
            {
                Property = _property,
                Amount = 0
            };
            return [new PropertyDiff(subject, zeroQuantity, zeroQuantity)];
        }

        var original = new Quantity
        {
            Property = _property,
            Amount = subject.Quantities.GetValue(_property)
        };
            
        int stagedValue = subject.Quantities.StageAdd(_property, amount);
            
        var updated = new Quantity
        {
            Property = _property,
            Amount = stagedValue
        };
            
        return [new PropertyDiff(subject, original, updated)];
    }
}