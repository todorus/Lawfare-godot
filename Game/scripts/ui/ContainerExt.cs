using Godot;

namespace Lawfare.scripts.ui;

public static class ContainerExt
{
    public static void ClearChildren(this Godot.Container container)
    {
        foreach (var child in container.GetChildren())
        {
            container.RemoveChild(child);
            child.QueueFree();
        }
    }

    public static void ClearChildren(this Node container)
    {
        foreach (var child in container.GetChildren())
        {
            child.QueueFree();
        }
    }
}