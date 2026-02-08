using Godot;

namespace Lawfare.scripts.ui;

public partial class Clickable : Control
{
    [Signal]
    public delegate void ClickedMouseButtonEventHandler(MouseButton button);
    
    [Signal]
    public delegate void ClickedEventHandler();

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignalClickedMouseButton(mouseEvent.ButtonIndex);
            EmitSignalClicked();
        }
    }
    
}