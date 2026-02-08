namespace Agents.scripts.story.player;

using Godot;

public partial class BlockMouseScroll : Control
{
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp ||
                mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                AcceptEvent();
            }
        }

        if (@event is InputEventPanGesture)
        {
            AcceptEvent();
        }
    }
}
