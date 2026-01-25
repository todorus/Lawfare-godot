using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.character;

public partial class TeamDisplay : Container
{
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private Team _debugTeam;
    
    public Team Team
    {
        set
        {
            this.ClearChildren();
            if (value == null) return;
            
            foreach (var member in value.Members)
            {
                var characterObserver = _characterScene.Instantiate<CharacterObserver>();
                characterObserver.Character = member;
                AddChild(characterObserver);
            }
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        Team = _debugTeam;
    }
}