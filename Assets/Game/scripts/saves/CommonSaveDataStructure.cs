using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raider.Game.Saves
{
    [Serializable]
    public class CommonSaveDataStructure
    {
        [Serializable]
        public struct SerializableColor
        {
            public SerializableColor(Color color)
            {
                r = g = b = 0;
                rfloat = color.r;
                gfloat = color.g;
                bfloat = color.b;
            }
            public Color Color { get { return new Color(r, g, b); } set { rfloat = value.r; gfloat = value.g; bfloat = value.b; } }
            //[SyncVar]
            private float rfloat { get { return r / 255f; } set { r = (byte)(value * 255); } }
            //[SyncVar]
            private float gfloat { get { return g / 255f; } set { g = (byte)(value * 255); } }
            //[SyncVar]
            private float bfloat { get { return b / 255f; } set { b = (byte)(value * 255); } }

            public byte r;
            public byte g;
            public byte b;
        }
    }
}
