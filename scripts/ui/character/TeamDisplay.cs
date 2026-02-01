using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.cards;
using Team = Lawfare.scripts.characters.lawyers.Team;

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
    private bool _mirror = false;
    
    public void SetTeam(Team team)
    {
        Team = team;
    }
    
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
    
    public void OnCharacterClicked(GodotObject character)
    {
        EmitSignalCharacterClicked(character);
    }
    
    public void OnHandChanged(Card[] cards)
    {
        EmitSignalHandChanged(cards);
    }
}