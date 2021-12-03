using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Char
{
    public class Parents
    {
        public int FatherShape { get; set; }
        public int MotherShape { get; set; }
        public int FatherSkin { get; set; }
        public int MotherSkin { get; set; }
        public double Similarity { get; set; }
        public double SkinSimilarity { get; set; }
    }

    public class Appearance
    {
        public int Value { get; set; }
        public double Opacity { get; set; }
    }

    public class Hairs
    {
        public int Hair { get; set; }
        public int Color { get; set; }
        public int HighlightColor { get; set; }
    }

    public class CustomizationModel
    {
        public Parents Parents { get; set; }
        public List<double> Features { get; set; }
        public List<Appearance> Appearance { get; set; }
        public Hairs Hair { get; set; }
        public int EyebrowColor { get; set; }
        public int BeardColor { get; set; }
        public int EyeColor { get; set; }
        public int BlushColor { get; set; }
        public int LipstickColor { get; set; }
        public int ChestHairColor { get; set; }

        /// <summary>
        /// Main Char
        /// </summary>
        public CustomizationModel()
        {
            Parents = new Parents()
            {
                FatherShape = 0,
                MotherShape = 34,
                FatherSkin = 0,
                MotherSkin = 2,
                Similarity = 0.74,
                SkinSimilarity = 0.71
            };

            Features = new List<double>()
            {
                -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01, -0.01,-0.01,-0.01
            };

            Hair = new Hairs()
            {
                Hair = 16,
                Color = 3,
                HighlightColor = 0
            };

            Appearance = new List<Appearance>()
            {
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 4, Opacity = 1 },
                new Appearance() { Value = 10, Opacity = 0.99 },
                new Appearance() { Value = 2, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 255, Opacity = 0.99 },
                new Appearance() { Value = 16, Opacity = 0.99 },
            };

            EyebrowColor = 0;
            BeardColor = 3;
            EyeColor = 2;
            BlushColor = 0;
            LipstickColor = 0;
            ChestHairColor = 52;
        }
    }
}
