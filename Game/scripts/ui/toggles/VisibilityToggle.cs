using Godot;

namespace Lawfare.scripts.ui.toggles;

public partial class VisibilityToggle : Node
{
    private Node3D _node3dParent;
    private Control _controlParent;
    private Node2D _node2DParent;
    
    public void SetVisible(bool isVisible)
    {
        if (_controlParent != null)
        {
            _controlParent.Visible = isVisible;
            return;
        }
        if (_node3dParent != null)
        {
            _node3dParent.Visible = isVisible;
            return;
        }
        if (_node2DParent != null)
        {
            _node2DParent.Visible = isVisible;
            return;
        }
    }
    
    public override void _Ready()
    {
        var parent = GetParent();
        if (parent is Node3D node3D)
        {
            _node3dParent = node3D;
            return;
        }
        if (parent is Control control)
        {
            _controlParent = control;
            return;
        }

        if (parent is Node2D node2D)
        {
            _node2DParent = node2D;
            return;
        }
    }
    
}