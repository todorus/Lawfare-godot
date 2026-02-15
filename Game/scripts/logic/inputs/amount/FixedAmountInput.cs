using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount;

[GlobalClass]
public partial class FixedAmountInput : AmountInput
{
    
    [Export] 
    public int Amount { get; private set; }
    protected override int GetAmountValue(GameEvent gameEvent) => Amount;
}