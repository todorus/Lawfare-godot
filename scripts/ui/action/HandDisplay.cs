using Godot;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.action;

public partial class HandDisplay : Container
{
    [Export]
    private PackedScene _cardScene;
    
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

    public void SetCards(Card[] cards)
    {
        Cards = cards;
    }

}