using Godot;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.ui.relations;

public partial class RelationsObserver : Container
{
    [Export]
    private PackedScene _relationScene;

    public void SetRelations(Relations relations) => Relations = relations;
    
    private Relations _relations;
    public Relations Relations
    {
        set
        {
            if (_relations != null)
            {
                _relations.OnChange -= UpdateRelationsDisplay;
            }
            
            _relations = value;
            if (value != null)
            {
                value.OnChange += UpdateRelationsDisplay;
            }
            UpdateRelationsDisplay(value);
        }
    }

    private void UpdateRelationsDisplay(Relations value)
    {
        this.ClearChildren();
        if (value == null) return;

        foreach (var relation in value.All)
        {
            var relationDisplay = _relationScene.Instantiate<RelationObserver>();
            relationDisplay.Relation = relation;
            AddChild(relationDisplay);
        }
    }
}