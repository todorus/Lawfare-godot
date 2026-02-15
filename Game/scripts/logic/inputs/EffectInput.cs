using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs;

[GlobalClass]
public abstract partial class EffectInput : Resource
{
    [Export]
    public InputLabel Label { get; private set; }
    
    public abstract object GetValue(GameEvent gameEvent);
}