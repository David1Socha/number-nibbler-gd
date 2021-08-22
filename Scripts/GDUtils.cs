using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberNibbler.Scripts
{
    public static class GDUtils
    {
        public static SignalAwaiter Wait(Node self, float seconds)
        {
            return self.ToSignal(self.GetTree().CreateTimer(seconds, pauseModeProcess: false), "timeout");
        }

        public static T PickRandomElement<T>(List<T> list, RandomNumberGenerator random)
        {
            return list[random.RandiRange(0, list.Count - 1)];
        }

        public static T PickRandomElement<T>(T[] list, RandomNumberGenerator random)
        {
            return list[random.RandiRange(0, list.Length - 1)];
        }
    }
}
