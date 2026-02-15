using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs.amount;

public abstract partial class AmountInput : Input
{
    public override object GetValue(Context context, GameEvent gameEvent) => GetAmountValue(context, gameEvent);
    
    protected abstract int GetAmountValue(Context context, GameEvent gameEvent);
}