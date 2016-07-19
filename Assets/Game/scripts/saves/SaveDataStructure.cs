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
        public Character() { }

        public Character(Color _primaryColor, Color _secondaryColor, Color _tertiaryColor, int _emblemLayer1, int _emblemLayer2, int _emblemLayer3, int _level, int _exp)
        {
            primaryColor = _primaryColor;
            secondaryColor = _secondaryColor;
            tertiaryColor = _tertiaryColor;
            emblemLayer1 = _emblemLayer1;
            emblemLayer2 = _emblemLayer2;
            emblemLayer3 = _emblemLayer3;
            level = _level;
            EXP = _exp;
        }

        public Color primaryColor;
        public Color secondaryColor;
        public Color tertiaryColor;
        public int emblemLayer1;
        public int emblemLayer2;
        public int emblemLayer3;
        public int level;
        public int EXP;
    }
}
