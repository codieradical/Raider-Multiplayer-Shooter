using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raider.Game.Saves;

namespace Raider.Game.GUI.Components
{

    public class EmblemHandler : MonoBehaviour
    {

        [HideInInspector]
        public Sprite[] layer0sprites;
        [HideInInspector]
        public Sprite[] layer1sprites;
        [HideInInspector]
        public Sprite[] layer2sprites;

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

        public void UpdateEmblem(SaveDataStructure.Character _character)
        {
            layer0image.color = _character.emblemLayer0Color.Color;
            layer1image.color = _character.emblemLayer1Color.Color;
            layer2image.color = _character.emblemLayer2Color.Color;

            layer2image.gameObject.SetActive(_character.emblemLayer2);

            layer0image.sprite = layer0sprites[_character.emblemLayer0];
            layer1image.sprite = layer1sprites[_character.emblemLayer1];
            layer2image.sprite = layer2sprites[_character.emblemLayer1];
        }
    }
}