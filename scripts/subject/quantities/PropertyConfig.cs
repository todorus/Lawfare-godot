using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class PropertyConfig : Resource
{
    [Export] public Property CorruptionLevel;

    [Export] public Property Urge;
}