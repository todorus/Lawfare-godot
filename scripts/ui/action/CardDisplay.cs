using Godot;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.action;

public partial class CardDisplay : Control
{
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    public Card Card
    {
        set => EmitSignalLabelChanged(value?.Label);
    }
    
}