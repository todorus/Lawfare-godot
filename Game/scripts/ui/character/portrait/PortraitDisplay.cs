using Godot;
using Lawfare.scripts.characters.lawyers;

namespace Lawfare.scripts.ui.character.portrait;

public partial class PortraitDisplay : Control
{
    [Export]
    public CharacterObserver CharacterObserver { get; private set; }
    
    [Signal]
    public delegate void CharacterClickedEventHandler(Lawyer character);
    
    public void Clicked()
    {
        EmitSignalCharacterClicked(CharacterObserver.Character as Lawyer);
    }
}