using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class Category : Resource
{
    [Export] public string Label { get; set; }
}