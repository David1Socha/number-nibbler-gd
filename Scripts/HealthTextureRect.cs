using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class HealthTextureRect : TextureRect
    {
        public void OnFrogHealthChanged(int health)
        {
            RectSize = new Vector2(health * Texture.GetWidth(), RectSize.y);
        }
    }
}