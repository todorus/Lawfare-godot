using System.Linq;
using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.character;

public partial class JudgeDisplay : Container
{
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Export]
    PackedScene _characterScene;
    
    public void SetJudges(Judge[] judges)
    {
        Judges = judges;
    }
    
    [Export]
    private bool _mirror = false;
    
    public Judge[] Judges
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