using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberNibbler.Scripts
{
    public class Startup : Node
    {
        public override void _Ready()
        {
            base._Ready();

            OS.MinWindowSize = new Vector2(600, 400);
        }
    }
}
