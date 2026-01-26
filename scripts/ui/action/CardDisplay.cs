using Godot;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.action;

public partial class CardDisplay : Control
{
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    [Signal]
    public delegate void OnClickedEventHandler(Card card);
    
    private Card _card;
    public Card Card
    {
        get => _card;
        set
        {
            _card = value;
            EmitSignalLabelChanged(value?.Label);
        }
    }
    
    public void OnClick()
    {
        EmitSignalOnClicked(Card);
    }
}