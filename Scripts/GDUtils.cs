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
        public static List<Timer> ActiveTimers { get; set; } = new List<Timer>();

        public static SignalAwaiter Wait(Node self, float seconds)
        {
            var timer = new Timer();
            self.AddChild(timer);
            timer.Start(seconds);
            ActiveTimers.Add(timer);
            return self.ToSignal(timer, "timeout");
        }

        public static void CancelAllActiveTimers()
        {
            foreach (var timer in ActiveTimers)
            {
                timer?.Stop();
                timer?.QueueFree();
            }

            ActiveTimers = new List<Timer>();
        }

        public static T PickRandomElement<T>(List<T> list, RandomNumberGenerator random)
        {
            return list[random.RandiRange(0, list.Count - 1)];
        }

        public static T PickRandomElement<T>(T[] list, RandomNumberGenerator random)
        {
            return list[random.RandiRange(0, list.Length - 1)];
        }

        public static void Quit(this Node self)
        {
            self.GetTree().Quit();
        }
    }
}
