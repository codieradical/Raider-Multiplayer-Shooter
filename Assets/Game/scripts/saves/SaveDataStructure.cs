using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using Raider.Game.Cameras;

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
		public Settings settings;

		[System.Serializable]
		public class Settings
		{
			public Settings()
			{
				lobbyDisplay = LobbyDisplay.Scroll;
				perspective = CameraModeController.CameraModes;
			}

			public enum LobbyDisplay
			{
				Scroll,
				Split
			}

			public LobbyDisplay lobbyDisplay;
			public CameraModeController.CameraModes perspective;
		}

        [System.Serializable]
        public class Emblem
        {
            public Emblem()
            {
                layer2Color = new SerializableColor(Color.white);
                layer1Color = new SerializableColor(Color.gray);
                layer0Color = new SerializableColor(Color.black);
                layer0 = 0;
                layer1 = 0;
                layer2 = true;
            }

            public Emblem(Color _layer2Color, Color _layer1Color, Color _layer0Color, int _layer0, int _layer1, bool _layer2)
            {
                layer0Color = new SerializableColor(_layer0Color);
                layer1Color = new SerializableColor(_layer1Color);
                layer2Color = new SerializableColor(_layer2Color);
                layer0 = _layer0;
                layer1 = _layer1;
                layer2 = _layer2;
            }

            public SerializableColor layer2Color;
            public SerializableColor layer1Color;
            public SerializableColor layer0Color;
            public int layer0;
            public int layer1;
            public bool layer2;
        }

        [System.Serializable] // Todo: Write custom serializer for networking.
        public class Character
        {
            /// <summary>
            /// Instanciates and assigns some default colors.
            /// </summary>
            public Character()
            {
                emblem = new Emblem();
                armourPrimaryColor = new SerializableColor(Color.cyan);
                armourSecondaryColor = new SerializableColor(Color.black);
                armourTertiaryColor = new SerializableColor(Color.cyan);
                race = Race.X;
                guild = "";
                //currentMission = "New Campaign";
            }

            public Character(Emblem _emblem, string _guild, Color _armourPrimaryColor, Color _armourSecondaryColor, Color _armourTertiaryColor, int _level, int _exp, Race _race)
            {
                emblem = _emblem;
                guild = _guild;
                armourPrimaryColor = new SerializableColor(_armourPrimaryColor);
                armourSecondaryColor = new SerializableColor(_armourSecondaryColor);
                armourTertiaryColor = new SerializableColor(_armourTertiaryColor);
                level = _level;
                exp = _exp;
                race = _race;
                //currentMission = _currentMission;
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

            //[SyncVar]
            //public string currentMission;
            [SyncVar]
            public Emblem emblem;
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
            public AvailableArmours shoulderArmour;
            [SyncVar]
            public AvailableArmours helmetArmour;
            [SyncVar]
            public AvailableArmours chestArmour;
            [SyncVar]
            public int level;
            [SyncVar]
            public int exp;
        }
    }
}