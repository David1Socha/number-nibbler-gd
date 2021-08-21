using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class HealthTextureRect : TextureRect
    {
        public void OnFrogHealthChanged(int health)
        {
            RectMinSize = new Vector2(health * Texture.GetWidth(), RectMinSize.y);
        }
    }
}