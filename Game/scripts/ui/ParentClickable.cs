using Godot;

namespace Lawfare.scripts.ui;

public partial class ParentClickable : Node
{
    [Signal]
    public delegate void ClickedMouseButtonEventHandler(MouseButton button);
    
    [Signal]
    public delegate void ClickedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        if(GetParent() is Control parentControl)
        {
            parentControl.GuiInput += _GuiInput;
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if(GetParent() is Control parentControl)
        {
            parentControl.GuiInput -= _GuiInput;
        }
    }

    private void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignalClickedMouseButton(mouseEvent.ButtonIndex);
            EmitSignalClicked();
        }
    }
}