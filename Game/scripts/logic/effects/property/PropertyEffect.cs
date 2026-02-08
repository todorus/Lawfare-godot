using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.property;

[GlobalClass]
public abstract partial class PropertyEffect : Effect
{
    [Export] public Property Property;
}