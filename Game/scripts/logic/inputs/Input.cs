using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs;

public abstract partial class Input : Resource
{
    [Export]
    public InputLabel Label { get; private set; }
    
    public abstract object GetValue(Context context, GameEvent gameEvent);
}