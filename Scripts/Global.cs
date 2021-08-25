using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberNibbler.Scripts
{
    public static class Global
    {
        public static class Difficulties
        {
            public const string Easy = "Easy";
            public const string Hard = "Hard";
        }

        public static class Categories
        {
            public const string Addition = "Addition";
            public const string Subtraction = "Subtraction";
            public const string Division = "Division";
            public const string Multiplication = "Multiplication";
            public const string Multiples = "Multiples";

            // TODO could add other categories -- fractions, decimals, exponents, etc... ???

            /// <summary>
            /// Test only value, shouldn't be exposed in actual game
            /// </summary>
            public const string Basic = "Basic";
        }

        public static class Config
        {
            public const string FilePath = "user://player_data.nncfg";
            public const string MenuSettingsSection = "MenuSettings";
            public const string CategoryValue = "Category";
            public const string DifficultyValue = "Difficulty";
        }
    }
}
