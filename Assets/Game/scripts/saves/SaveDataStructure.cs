using UnityEngine;
using System.Collections.Generic;
using System;

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
        public Color color { get { return new Color(r, g, b); } set { r = value.r; g = value.g; b = value.b; } }
        float r;
        float g;
        float b;
    }

    public string username;
    public string password;

    public List<Character> characters = new List<Character>();

    [System.Serializable]
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
        }

        public Character(string _guild, Color _armourPrimaryColor, Color _armourSecondaryColor, Color _armourTertiaryColor, Color _emblemLayer2Color, Color _emblemLayer1Color, Color _emblemLayer0Color, int _emblemLayer0, int _emblemLayer1, bool _emblemLayer2, int _level, int _exp, Race _race)
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

        public Race race;
        public string guild;
        public SerializableColor armourPrimaryColor;
        public SerializableColor serializableArmourPrimaryColor;
        public SerializableColor armourSecondaryColor;
        public SerializableColor armourTertiaryColor;
        public SerializableColor emblemLayer2Color;
        public SerializableColor emblemLayer1Color;
        public SerializableColor emblemLayer0Color;
        public AvailableArmours shoulderArmour;
        public AvailableArmours helmetArmour;
        public AvailableArmours chestArmour;
        public int emblemLayer0;
        public int emblemLayer1;
        public bool emblemLayer2;
        public int level;
        public int exp;
    }
}
