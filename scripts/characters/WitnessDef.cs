using Godot;

namespace Lawfare.scripts.characters;

[GlobalClass]
public partial class WitnessDef : Resource
{
    [Export]
    public string Label { get; private set; }

    [Export]
    public Texture2D Image { get; private set; }
}