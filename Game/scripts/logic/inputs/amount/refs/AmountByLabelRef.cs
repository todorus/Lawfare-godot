using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount.refs;

[GlobalClass]
public partial class AmountByLabelRef : AmountRef
{
    [Export] 
    private InputLabel _label;
    
    protected override int GetAmountValue(GameEvent gameEvent)
    {
        var input = gameEvent.Inputs[_label] as AmountInput;
        if (input == null)
        {
            GD.PrintErr($"No AmountInput found for label {_label}");
            return 0;
        }
        
        return input.GetValue(gameEvent) as int? ?? 0;
    }
}