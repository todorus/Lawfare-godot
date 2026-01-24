using System.Linq;
using Agents.scripts.ui.action;
using Godot;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.effects.property;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.@event;

public partial class ChangeDisplaySpawner : Node3D
{
    [Export] private PackedScene _changeDisplay;

    public void OnResolution(Resolution resolution)
    {
        DisplayChanges(resolution.Changes);
    }

    public void DisplayChanges(IChange[] changes)
    {
        var propertyChanges = changes.OfType<PropertyAddEffect.PropertyAddChange>();

        var grouped = propertyChanges
            .GroupBy(pc => new { pc.Subject, pc.Property })
            .Select(g => new
            {
                g.Key.Subject,
                g.Key.Property,
                Amount = g.Sum(pc => pc.Amount)
            });

        foreach (var group in grouped)
        {
            var displayInstance = _changeDisplay.Instantiate<ChangeDisplay>();
            displayInstance.Subject = group.Subject;
            displayInstance.ChangeQuantity = new Quantity
            {
                Property = group.Property,
                Amount = group.Amount
            };

            if (group.Subject is Node3D node3D)
                node3D.AddChild(displayInstance);
            else
                AddChild(displayInstance);
        }
    }
}