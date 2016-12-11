using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

namespace Raider.Game.Saves
{

    [Serializable]
    public class SaveDataStructure
    {
        [Serializable]
        public struct SerializableColor
        {
            public SerializableColor(Color color)
            {
                r = color.r;
                g = color.g;
                b = color.b;
            }
            public Color Color { get { return new Color(r, g, b); } set { r = value.r; g = value.g; b = value.b; } }
            [SyncVar]
            float r;
            [SyncVar]
            float g;
            [SyncVar]
            float b;
        }

        public string username;
        public string password;

        public List<Character> characters = new List<Character>();

        [System.Serializable] // Todo: Write custom serializer for networking.
        public class Character
        {
            /// <summary>
            /// Instanciates and assigns some default colors.
            /// </summary>
            public Character()
            {
                armourPrimaryColor = new SerializableColor(Color.cyan);
                armourSecondaryColor = new SerializableColor(Color.black);
                armourTertiaryColor = new SerializableColor(Color.cyan);
                emblemLayer2Color = new SerializableColor(Color.white);
                emblemLayer1Color = new SerializableColor(Color.gray);
                emblemLayer0Color = new SerializableColor(Color.black);
                emblemLayer2 = true;
                race = Race.X;
                guild = "";
                currentMission = "New Campaign";
            }

            public Character(string _guild, Color _armourPrimaryColor, Color _armourSecondaryColor, Color _armourTertiaryColor, Color _emblemLayer2Color, Color _emblemLayer1Color, Color _emblemLayer0Color, int _emblemLayer0, int _emblemLayer1, bool _emblemLayer2, int _level, int _exp, Race _race, string _currentMission)
            {
                guild = _guild;
                armourPrimaryColor = new SerializableColor(_armourPrimaryColor);
                armourSecondaryColor = new SerializableColor(_armourSecondaryColor);
                armourTertiaryColor = new SerializableColor(_armourTertiaryColor);
                emblemLayer0Color = new SerializableColor(_emblemLayer0Color);
                emblemLayer1Color = new SerializableColor(_emblemLayer1Color);
                emblemLayer2Color = new SerializableColor(_emblemLayer2Color);
                emblemLayer0 = _emblemLayer0;
                emblemLayer1 = _emblemLayer1;
                emblemLayer2 = _emblemLayer2;
                level = _level;
                exp = _exp;
                race = _race;
                currentMission = _currentMission;
            }

            public enum AvailableArmours
            {
                X,
                Y
            }

            public enum Race
            {
                X = 0,
                Y = 1
            }

            [SyncVar]
            public string currentMission;
            [SyncVar]
            public Race race;
            [SyncVar]
            public string guild;
            [SyncVar]
            public SerializableColor armourPrimaryColor;
            [SyncVar]
            public SerializableColor armourSecondaryColor;
            [SyncVar]
            public SerializableColor armourTertiaryColor;
            [SyncVar]
            public SerializableColor emblemLayer2Color;
            [SyncVar]
            public SerializableColor emblemLayer1Color;
            [SyncVar]
            public SerializableColor emblemLayer0Color;
            [SyncVar]
            public AvailableArmours shoulderArmour;
            [SyncVar]
            public AvailableArmours helmetArmour;
            [SyncVar]
            public AvailableArmours chestArmour;
            [SyncVar]
            public int emblemLayer0;
            [SyncVar]
            public int emblemLayer1;
            [SyncVar]
            public bool emblemLayer2;
            [SyncVar]
            public int level;
            [SyncVar]
            public int exp;
        }
    }
}