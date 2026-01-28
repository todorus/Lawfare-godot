using System.Linq;
using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.character;

public partial class WitnessDisplay : Container
{
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private bool _mirror = false;
    
    public void SetWitnesses(Witness[] witnesses)
    {
        Witnesses = witnesses;
    }
    
    public Witness[] Witnesses
    {
        set
        {
            this.ClearChildren();
            if (value == null) return;
            
            foreach (var witness in value)
            {
                var characterObserver = _characterScene.Instantiate<CharacterObserver>();
                characterObserver.Character = witness;
                characterObserver.Mirror = _mirror;
                characterObserver.CharacterClicked += EmitSignalCharacterClicked;
                AddChild(characterObserver);
            }
        }
    }
}