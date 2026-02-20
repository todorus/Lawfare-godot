using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount;

[GlobalClass]
public partial class SumAmountInput : AmountInput
{
    
    [Export]
    public AmountInput[] Inputs { get; private set; } = [];
    
    [ExportGroup("Limits")]
    [Export] 
    public int Min { get; private set; } = 0;
    [Export] 
    public int Max { get; private set; } = int.MaxValue;
        
    protected override int GetAmountValue(GameEvent gameEvent)
    {
        var value = Inputs.Sum(input => (int) input.GetValue(gameEvent));
        return Mathf.Clamp(value, Min, Max);
    }
}