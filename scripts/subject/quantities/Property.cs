using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class Property : Resource
{
    [Export] public string Label { get; set; }

    [Export] public Texture2D Icon { get; set; }

    [Export] public virtual Category Category { get; set; }

    [Export] public int Minimum { get; set; }

    [Export] public int Maximum { get; set; } = int.MaxValue;
}