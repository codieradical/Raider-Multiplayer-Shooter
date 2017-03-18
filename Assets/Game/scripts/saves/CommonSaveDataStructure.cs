using System;
using UnityEngine;

namespace Raider.Game.Saves
{
    [Serializable]
    public class CommonSaveDataStructure
    {
        //Should probably add Alpha.
        [Serializable]
        public struct SerializableColor
        {
            //Struct constructors must assign variables values, 
            //but using properties is not sufficient for compile.
            //So first, the bytes are assigned 0.
            public SerializableColor(Color color)
            {
                r = g = b = 0;
                Rfloat = color.r;
                Gfloat = color.g;
                Bfloat = color.b;
            }

            //Convert this struct to a UnityEngine color, or vica versa.
            public Color Color {
                get { return new Color(Rfloat, Gfloat, Bfloat); }
                set { Rfloat = value.r; Gfloat = value.g; Bfloat = value.b; } }

            //Access the byte values, converted to percentage floats. 255 being 1, or 100%.
            private float Rfloat {
                get { return r / 255f; }
                set { r = (byte)(value * 255); } }
            private float Gfloat {
                get { return g / 255f; }
                set { g = (byte)(value * 255); } }
            private float Bfloat {
                get { return b / 255f; }
                set { b = (byte)(value * 255); } }

            public byte r;
            public byte g;
            public byte b;
        }
    }
}
