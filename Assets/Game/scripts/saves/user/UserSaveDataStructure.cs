using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;

namespace Raider.Game.Saves.User
{
    /// <summary>
    /// User data synced with the online database.
    /// </summary>
    [Serializable]
    public class UserSaveDataStructure
    {
        public string username;

        public List<Character> characters = new List<Character>();
        public UserSettings userSettings = new UserSettings();

        [Serializable]
        public class UserSettings
        {
            public UserSettings()
            {
                LobbyDisplay = LobbyDisplays.Scroll;
                perspective = CameraModeController.CameraModes.FirstPerson;
            }

            public enum LobbyDisplays
            {
                Scroll,
                Split
            }

            public string lobbyDisplayString;
            public string perspectiveString;

            public LobbyDisplays LobbyDisplay
            {
                get { return (LobbyDisplays)Enum.Parse(typeof(LobbyDisplays), lobbyDisplayString); }
                set { lobbyDisplayString = value.ToString(); }
            }

            public CameraModeController.CameraModes perspective
            {
                get { return (CameraModeController.CameraModes)Enum.Parse(typeof(CameraModeController.CameraModes), perspectiveString); }
                set { perspectiveString = value.ToString(); }
            }
        }

        [Serializable]
        public class Emblem
        {
            public Emblem()
            {
                layer2Color = new CommonSaveDataStructure.SerializableColor(Color.white);
                layer1Color = new CommonSaveDataStructure.SerializableColor(Color.gray);
                layer0Color = new CommonSaveDataStructure.SerializableColor(Color.black);
                layer0 = 0;
                layer1 = 0;
                layer2 = true;
            }

            public Emblem(Color _layer2Color, Color _layer1Color, Color _layer0Color, int _layer0, int _layer1, bool _layer2)
            {
                layer0Color = new CommonSaveDataStructure.SerializableColor(_layer0Color);
                layer1Color = new CommonSaveDataStructure.SerializableColor(_layer1Color);
                layer2Color = new CommonSaveDataStructure.SerializableColor(_layer2Color);
                layer0 = _layer0;
                layer1 = _layer1;
                layer2 = _layer2;
            }

            public CommonSaveDataStructure.SerializableColor layer2Color;
            public CommonSaveDataStructure.SerializableColor layer1Color;
            public CommonSaveDataStructure.SerializableColor layer0Color;
            public int layer0;
            public int layer1;
            public bool layer2;
        }

        [Serializable] // Todo: Write custom serializer for networking.
        public class Character
        {
            /// <summary>
            /// Instanciates and assigns some default colors.
            /// </summary>
            public Character()
            {
                emblem = new Emblem();
                armourPrimaryColor = new CommonSaveDataStructure.SerializableColor(Color.cyan);
                armourSecondaryColor = new CommonSaveDataStructure.SerializableColor(Color.black);
                armourTertiaryColor = new CommonSaveDataStructure.SerializableColor(Color.cyan);
                Race = Races.X;
                HelmetArmour = Armours.X;
                ShoulderArmour = Armours.X;
                ChestArmour = Armours.X;
                guild = "";
                //currentMission = "New Campaign";
            }

            public Character(Emblem _emblem, string _guild, Color _armourPrimaryColor, Color _armourSecondaryColor, Color _armourTertiaryColor, int _level, int _exp, Races _race)
            {
                emblem = _emblem;
                guild = _guild;
                armourPrimaryColor = new CommonSaveDataStructure.SerializableColor(_armourPrimaryColor);
                armourSecondaryColor = new CommonSaveDataStructure.SerializableColor(_armourSecondaryColor);
                armourTertiaryColor = new CommonSaveDataStructure.SerializableColor(_armourTertiaryColor);
                level = _level;
                exp = _exp;
                Race = _race;
                //currentMission = _currentMission;
            }

            public enum Armours
            {
                X,
                Y
            }

            public enum Races
            {
                X = 0,
                Y = 1,
                Spartan = 2
            }


            //Unity's JsonUtility doesn't understand enums, so im storing them as strings.
            //The strings are stored in the database and synced on networks.
            //However, the game reads the enum properties, which parse the strings.
            //The alternative would be parsing the number representations of the enums on the API,
            //However, that would be higher maintainence.
            //Additionally, if I find a fix, this'll be easier to update.

            [SyncVar] public Emblem emblem;
            [SyncVar] public string raceString; //Race Enum
            [SyncVar] public string guild;
            [SyncVar] public CommonSaveDataStructure.SerializableColor armourPrimaryColor;
            [SyncVar] public CommonSaveDataStructure.SerializableColor armourSecondaryColor;
            [SyncVar] public CommonSaveDataStructure.SerializableColor armourTertiaryColor;
            [SyncVar] public string shoulderArmourString; //Armours Enum
            [SyncVar] public string helmetArmourString; //Armours Enum
            [SyncVar] public string chestArmourString; //Armours Enum
            [SyncVar] public int level;
            [SyncVar] public int exp;

            public Races Race
            {
                get { return (Races)Enum.Parse(typeof(Races), raceString); }
                set { raceString = value.ToString(); }
            }
            public Armours ShoulderArmour
            {
                get { return (Armours)Enum.Parse(typeof(Armours), shoulderArmourString); }
                set { shoulderArmourString = value.ToString(); }
            }
            public Armours HelmetArmour
            {
                get { return (Armours)Enum.Parse(typeof(Armours), helmetArmourString); }
                set { helmetArmourString = value.ToString(); }
            }
            public Armours ChestArmour
            {
                get { return (Armours)Enum.Parse(typeof(Armours), chestArmourString); }
                set { chestArmourString = value.ToString(); }
            }
        }
    }
}