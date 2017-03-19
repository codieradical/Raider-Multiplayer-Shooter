using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using Raider.Game.Cameras;
using Raider.Game.Saves;
using Raider.Game.Weapons;

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
                Perspective = CameraModeController.CameraModes.FirstPerson;
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

            public CameraModeController.CameraModes Perspective
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
                PrimaryWeapon = Armory.DEFAULT_PRIMARY_WEAPON;
                SecondaryWeapon = Armory.DEFAULT_SECONDARY_WEAPON;
                TertiaryWeapon = Armory.DEFAULT_TERTIARY_WEAPON;
                foreach (Armory.Weapons weapon in Enum.GetValues(typeof(Armory.Weapons)))
                {
                    weaponCustomizations.Add(new Armory.WeaponTypeAndVariation(weapon, Armory.WeaponVariation.Mid));
                }
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
                PrimaryWeapon = Armory.DEFAULT_PRIMARY_WEAPON;
                SecondaryWeapon = Armory.DEFAULT_SECONDARY_WEAPON;
                TertiaryWeapon = Armory.DEFAULT_TERTIARY_WEAPON;
                foreach (Armory.Weapons weapon in Enum.GetValues(typeof(Armory.Weapons)))
                {
                    weaponCustomizations.Add(new Armory.WeaponTypeAndVariation(weapon, Armory.WeaponVariation.Mid));
                }
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
                Y = 1
            }


            //Unity's JsonUtility doesn't understand enums, so I'm storing them as strings.
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
            [SyncVar] public string primaryWeaponString;
            [SyncVar] public string secondaryWeaponString;
            [SyncVar] public string tertiaryWeaponString;
            [SyncVar] public List<Armory.WeaponTypeAndVariation> weaponCustomizations = new List<Armory.WeaponTypeAndVariation>();

            public Armory.WeaponVariation GetWeaponChosenVariation(Armory.Weapons weapon)
            {
                foreach(Armory.WeaponTypeAndVariation weaponCustomization in weaponCustomizations)
                {
                    if (weaponCustomization.Weapon == weapon)
                        return weaponCustomization.Variation;
                }
                return Armory.WeaponVariation.Mid;
            }

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
            public Armory.Weapons PrimaryWeapon
            {
                get { return (Armory.Weapons)Enum.Parse(typeof(Armory.Weapons), primaryWeaponString); }
                set { primaryWeaponString = value.ToString(); }
            }
            public Armory.Weapons SecondaryWeapon
            {
                get { return (Armory.Weapons)Enum.Parse(typeof(Armory.Weapons), secondaryWeaponString); }
                set { secondaryWeaponString = value.ToString(); }
            }
            public Armory.Weapons TertiaryWeapon
            {
                get { return (Armory.Weapons)Enum.Parse(typeof(Armory.Weapons), tertiaryWeaponString); }
                set { tertiaryWeaponString = value.ToString(); }
            }
        }
    }
}