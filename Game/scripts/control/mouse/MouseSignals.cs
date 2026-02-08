using System;
using Godot;

namespace Agents.scripts.control.mouse;

public partial class MouseSignals : Node
{
    [Export]
    public Camera3D Camera { get; set; }
    
    [Signal]
    public delegate void LeftMouseEventHandler(GodotObject obj, bool isDown);
    [Signal]
    public delegate void RightMouseEventHandler(GodotObject obj, bool isDown);
    [Signal]
    public delegate void MiddleMouseEventHandler(GodotObject obj, bool isDown);
    
    [Signal]
    public delegate void MoveEventHandler(GodotObject obj, Vector2 delta);
    
    [Signal]
    public delegate void PanEventHandler(GodotObject obj, Vector2 delta);

    public override void _UnhandledInput(InputEvent @event)
    {
        base._Input(@event);
        var obj = GetObjectUnderMouse();
        
        if (@event is InputEventMouseMotion mouseMotion)
        {
            EmitSignalMove(obj, mouseMotion.Relative);
        } else if (@event is InputEventMouseButton mouseButton)
        {
            HandleButton(mouseButton.ButtonIndex, mouseButton.Pressed, obj);
        } else if (@event is InputEventPanGesture panGesture) 
        {
            EmitSignalPan(obj, panGesture.Delta);
        }
    }
    
    private void HandleButton(MouseButton buttonIndex, bool pressed, GodotObject obj)
    {
        switch (buttonIndex)
        {
            case MouseButton.Left:
                EmitSignalLeftMouse(obj, pressed);
                break;
            case MouseButton.Right:
                EmitSignalRightMouse(obj, pressed);
                break;
            case MouseButton.Middle:
                EmitSignalMiddleMouse(obj, pressed);
                break;
        }
    }

    private GodotObject GetObjectUnderMouse()
    {
        var result = this.ShootRayFromCamera(Camera);
        if (result.Count == 0)
        {
            return null;
        }
        
        var collider = result["collider"];
        var obj = collider.Obj;
        
        if (obj is HitBox hitBox)
        {
            return hitBox.Target;
        }
        if (obj is GodotObject godotObject)
        {
            return godotObject;
        }

        return null;
    }
}