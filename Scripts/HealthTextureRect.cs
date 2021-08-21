using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class HealthTextureRect : TextureRect
    {
        public void OnFrogHealthChanged(int health)
        {
            GD.Print($"received health update in ui!! {health}");
            // TODO set width to tileSize * health
        }
    }
}