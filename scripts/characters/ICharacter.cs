using Godot;
using Lawfare.scripts.info;

namespace Lawfare.scripts.characters;

public interface ICharacter : IInfo
{
    public Texture2D Image { get; }
}