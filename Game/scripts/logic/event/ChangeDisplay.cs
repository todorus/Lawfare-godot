using System;
using Godot;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Agents.scripts.ui.action;

public partial class ChangeDisplay : Node3D
{
    [Export]
    private Sprite3D Icon;
    
    [Export]
    private Label3D AmountLabel;
    
    [ExportGroup("Height Animation")]
    [Export]
    private Curve HeightCurve;
    [Export]
    private float HeightMultiplier = 1.0f;
    [Export]
    private float Duration = .4f;
    [Export]
    private float RemoveDelay = 3f;
    
    public Vector3 StartPosition { get; set; }
    
    private double _elapsedTime;
    
    public Quantity ChangeQuantity
    {
        set
        {
            Icon.Texture = value.Property.Icon;
            AmountLabel.Text = value.Amount.ToString();
        }
    }

    public ISubject Subject { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        try
        {
            if (Subject is Node node && node.IsQueuedForDeletion())
            {
                QueueFree();
                return;
            }
        } catch (ObjectDisposedException)
        {
            QueueFree();
            return;
        }

        _elapsedTime += delta;
        if (_elapsedTime >= Duration + RemoveDelay)
        {
            QueueFree();
            return;
        }
        var t = (float) Math.Clamp(_elapsedTime / Duration, 0, 1);
        var heightOffset = new Vector3(0, HeightCurve.Sample(t) * HeightMultiplier, 0);
        
        Position = Subject.DamagePosition + heightOffset;
        Rotation = Vector3.Zero;
    }
}