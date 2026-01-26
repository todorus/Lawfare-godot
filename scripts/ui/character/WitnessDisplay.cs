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
    private WitnessDef[] _debugWitnesses;
    
    [Export]
    private bool _mirror = false;
    
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
    
    public override void _Ready()
    {
        base._Ready();
        Witnesses = _debugWitnesses.Select(def => new Witness(def)).ToArray();
    }
}