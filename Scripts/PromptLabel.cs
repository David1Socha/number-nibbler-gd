using Godot;
using System;
namespace NumberNibbler.Scripts
{
    public class PromptLabel : Label
    {
        public void OnPromptChanged(string prompt)
        {
            Text = prompt;
        }
    }
}