using Godot;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.ui.relations;

public partial class RelationObserver : Control
{
    [Signal]
    public delegate void FactionChangedEventHandler(Texture2D faction);
    [Signal]
    public delegate void IconChangedEventHandler(Texture2D icon);
    [Signal]
    public delegate void AmountChangedEventHandler(string amount);
    
    private IRelation _relation;

    public IRelation Relation
    {
        get => _relation;
        set
        {
            _relation = value;
            UpdateDisplay(value);
        }
    }

    private void UpdateDisplay(IRelation relation)
    {
        EmitSignalFactionChanged(relation?.Faction?.Icon);
        EmitSignalIconChanged(relation?.Property.Icon);
        var amount = relation?.Amount ?? 0;
        EmitSignalAmountChanged(amount.ToString());
    }
}