using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.inputs.subject;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using SubjectRef = Lawfare.scripts.logic.inputs.subject.refs.SubjectRef;

namespace Lawfare.scripts.logic.inputs.amount;

[GlobalClass]
public partial class PropertyAmountInput : AmountInput
{
    [Export] 
    private SubjectRef _subjectRef;
    
    [Export]
    private Property _property;
    
    protected override int GetAmountValue(GameEvent gameEvent)
    {
        var subjectValue = _subjectRef.GetValue(gameEvent);
        if (subjectValue is not ISubject subject) return 0;

        var amount = subject.Quantities.GetValue(_property);
        return amount;
    }
}