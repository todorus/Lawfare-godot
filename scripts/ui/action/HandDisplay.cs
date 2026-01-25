using System.Linq;
using Godot;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.action;

public partial class HandDisplay : Container
{
    [Export]
    private PackedScene _cardScene;

    [Export] 
    private Action[] _debugActions;
    
    public Card[] Cards 
    {
        set
        {
            this.ClearChildren();
            foreach (var card in value)
            {
                var cardDisplay = _cardScene.Instantiate<CardDisplay>();
                cardDisplay.Card = card;
                AddChild(cardDisplay);
            }
        }
    }
    
    public override void _Ready()
    {
        // Debug
        Cards = _debugActions
            .Select(action => new Card(action))
            .ToArray();
    }

}