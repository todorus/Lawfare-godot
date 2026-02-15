using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount.refs;

[GlobalClass]
public abstract partial class AmountRef : ValueRef
{ 
    public override object GetValue(GameEvent gameEvent) => GetAmountValue(gameEvent);
    
    protected abstract int GetAmountValue(GameEvent gameEvent);
}