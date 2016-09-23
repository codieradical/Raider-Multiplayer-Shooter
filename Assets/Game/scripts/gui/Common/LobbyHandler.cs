using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Raider.Game.Saves;
using Raider.Game.GUI.Layout;
using Raider.Game.Networking;

namespace Raider.Game.GUI.Components
{

    public class LobbyHandler : MonoBehaviour
    {
        #region Singleton Setup

        private static List<LobbyHandler> instances;

        void Awake()
        {
            if (instances == null)
                instances = new List<LobbyHandler>();
            instances.Add(this);
        }

        void OnDestroy()
        {
            instances.Remove(this);
            //instances.RemoveAt(instancePositionInArray);
        }

        #endregion

        /// <summary>
        /// This prefab needs to be available on at least one lobby handler per scene.
        /// </summary>
        public UnityEngine.Object nameplatePrefab;
        public UnityEngine.Object loadingNameplatePrefab;

        private static List<PlayerNameplate> players = new List<PlayerNameplate>();

        public struct PlayerNameplate
        {
            public PlayerNameplate(string _username, bool _leader, bool _speaking, bool _canspeak, SaveDataStructure.Character _character)
            {
                username = _username;
                leader = _leader;
                speaking = _speaking;
                canspeak = _canspeak;
                character = _character;
            }

            public string username;
            public bool leader;
            public bool speaking;
            public bool canspeak;

            public SaveDataStructure.Character character;
        }

        public static void DestroyAllPlayers()
        {
            players = new List<PlayerNameplate>();

            foreach(LobbyHandler instance in instances)
            {
                foreach (Transform nameplate in instance.gameObject.transform.FindChild("Players"))
                {
                    Destroy(nameplate.gameObject);
                }
            }
        }

        public static void AddLoadingPlayer()
        {
            foreach(LobbyHandler instance in instances)
            {
                if (instance.loadingNameplatePrefab == null)
                {
                    Debug.Log("A lobby handler is missing a nameplate prefab.");
                    Debug.LogAssertion("Please add the prefab to any lobby in the scene.");
                    throw new MissingFieldException();
                }

                GameObject newPlayer = Instantiate(instance.loadingNameplatePrefab) as GameObject;
                newPlayer.transform.SetParent(instance.gameObject.transform.FindChild("Players"), false);
                UpdateSidebars();
            }
        }

        public static void AddPlayer(PlayerNameplate player)
        {
            players.Add(player);

            foreach (LobbyHandler instance in instances)
            {
                if (instance.nameplatePrefab == null)
                {
                    Debug.Log("A lobby handler is missing a nameplate prefab.");
                    Debug.LogAssertion("Please add the prefab to any lobby in the scene.");
                    throw new MissingFieldException();
                }

                GameObject newPlayer = Instantiate(instance.nameplatePrefab) as GameObject;

                newPlayer.GetComponent<PreferredSizeOverride>().providedGameObject = instance.gameObject;

                newPlayer.name = player.username;
                newPlayer.transform.FindChild("emblem").GetComponent<EmblemHandler>().UpdateEmblem(player.character);

                newPlayer.transform.SetParent(instance.gameObject.transform.FindChild("Players"), false);
                newPlayer.transform.FindChild("name").GetComponent<Text>().text = player.username;
                newPlayer.transform.FindChild("guild").GetComponent<Text>().text = player.character.guild;
                newPlayer.transform.FindChild("level").GetComponent<Text>().text = player.character.level.ToString();
                newPlayer.transform.FindChild("icons").FindChild("leader").gameObject.SetActive(player.leader);

                Color plateColor = player.character.armourPrimaryColor.color;

                float _h, _s, _v;
                Color.RGBToHSV(plateColor, out _h, out _s, out _v);

                plateColor = Color.HSVToRGB(_h, _s, 0.5f);
                plateColor.a = 200f / 255f;

                //Color plateColor = character.armourPrimaryColor.color;
                //plateColor.a = 0.5f;
                //newPlate.GetComponent<Image>().color = plateColor;

                newPlayer.GetComponent<Image>().color = plateColor;

                UpdateSidebars();
            }
        }

        static void UpdateSidebars()
        {
            foreach (LobbyHandler instance in instances)
            {
                instance.transform.FindChild("Sidebar").FindChild("Player Count").GetComponent<Text>().text = String.Format("{0}/{1}", players.Count.ToString(), NetworkManager.instance.maxPlayers);
            }
        }
    }
}