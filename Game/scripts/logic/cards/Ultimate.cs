using Godot;

namespace Lawfare.scripts.logic.cards;

[GlobalClass]
public partial class Ultimate : Resource
{
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public string Description { get; private set; }
}