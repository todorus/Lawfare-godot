using Godot;

namespace Lawfare.scripts.ui.character.portrait;

public partial class PortraitDisplay : Control
{
    [Export]
    public CharacterObserver CharacterObserver { get; private set; }
}