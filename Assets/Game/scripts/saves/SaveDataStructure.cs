using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveDataStructure
{
    public string username;
    public string password;

    public List<Character> characters;


    [System.Serializable]
    public class Character
    {
        /// <summary>
        /// Instanciates and assigns some default colors.
        /// </summary>
        public Character()
        {
            armourPrimaryColor = Color.cyan;
            armourSecondaryColor = Color.black;
            armourTertiaryColor = Color.cyan;
            emblemLayer2Color = Color.white;
            emblemLayer1Color = Color.gray;
            emblemLayer0Color = Color.black;
        }

        public Character(string _guild, Color _armourPrimaryColor, Color _armourSecondaryColor, Color _armourTertiaryColor, Color _emblemLayer2Color, Color _emblemLayer1Color, Color _emblemLayer0Color, int _emblemLayer0, int _emblemLayer1, bool _emblemLayer2, int _level, int _exp)
        {
            guild = _guild;
            armourPrimaryColor = _armourPrimaryColor;
            armourSecondaryColor = _armourSecondaryColor;
            armourTertiaryColor = _armourTertiaryColor;
            emblemLayer0Color = _emblemLayer0Color;
            emblemLayer1Color = _emblemLayer1Color;
            emblemLayer2Color = _emblemLayer2Color;
            emblemLayer0 = _emblemLayer0;
            emblemLayer1 = _emblemLayer1;
            emblemLayer2 = _emblemLayer2;
            level = _level;
            EXP = _exp;
        }

        public enum AvailableArmours
        {
            X,
            Y
        }

        public string guild;
        public Color armourPrimaryColor;
        public Color armourSecondaryColor;
        public Color armourTertiaryColor;
        public Color emblemLayer2Color;
        public Color emblemLayer1Color;
        public Color emblemLayer0Color;
        public AvailableArmours shoulderArmour;
        public AvailableArmours helmetArmour;
        public AvailableArmours chestArmour;
        public int emblemLayer0;
        public int emblemLayer1;
        public bool emblemLayer2;
        public int level;
        public int EXP;
    }
}
