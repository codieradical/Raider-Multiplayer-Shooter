using Raider.Game.Saves.User;
using UnityEngine;
using UnityEngine.UI;

namespace Raider.Game.GUI.Components
{

    public class EmblemHandler : MonoBehaviour
    {

        [HideInInspector]
        public static Sprite[] layer0sprites;
        [HideInInspector]
		public static Sprite[] layer1sprites;
        [HideInInspector]
		public static Sprite[] layer2sprites;

        [Header("Images")]
        public Image layer0image;
        public Image layer1image;
        public Image layer2image;

        public void Awake()
        {
            layer0sprites = Resources.LoadAll<Sprite>("gui/emblems/layer0");
            layer1sprites = Resources.LoadAll<Sprite>("gui/emblems/layer1");
            layer2sprites = Resources.LoadAll<Sprite>("gui/emblems/layer2");
        }

        public void UpdateEmblem(UserSaveDataStructure.Character _character)
        {
            layer0image.color = _character.emblem.layer0Color.Color;
            layer1image.color = _character.emblem.layer1Color.Color;
            layer2image.color = _character.emblem.layer2Color.Color;

            layer2image.gameObject.SetActive(_character.emblem.layer2);

            layer0image.sprite = layer0sprites[_character.emblem.layer0];
            layer1image.sprite = layer1sprites[_character.emblem.layer1];
            layer2image.sprite = layer2sprites[_character.emblem.layer1];
        }
    }
}