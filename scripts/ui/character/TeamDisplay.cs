using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.character;

public partial class TeamDisplay : Container
{
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Signal]
    public delegate void HandChangedEventHandler(Card[] cards);
    
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private LawyerDef[] _debugTeam;
    
    [Export]
    private bool _mirror = false;
    
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
                characterObserver.Mirror = _mirror;
                characterObserver.CharacterClicked += OnCharacterClicked;
                characterObserver.HandChanged += OnHandChanged;
                AddChild(characterObserver);
            }
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        Team = new Team
        {
            Members = _debugTeam.Select(def => new Lawyer(def)).ToArray()
        };
    }
    
    public void OnCharacterClicked(GodotObject character)
    {
        EmitSignalCharacterClicked(character);
    }
    
    public void OnHandChanged(Card[] cards)
    {
        EmitSignalHandChanged(cards);
    }
}