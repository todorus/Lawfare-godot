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
    
    [Export]
    private JudgeDef[] _debugJudges;
    
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
    
    public override void _Ready()
    {
        base._Ready();
        Judges = _debugJudges.Select(def => new Judge(def)).ToArray();
    }
}