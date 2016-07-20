using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class CharacterEditorHandler : MonoBehaviour {

    [Header("Characters")]
    public GameObject charactersParent;

    [Header("Prefabs")]
    public Object selectedCharacterPrefab;

    private RenderTexture selectedCharacterView;
    private GameObject selectedCharacterDisplayModel;
    private Material selectedCharacterMaterial1;
    private Material selectedCharacterMaterial2;
    private Material selectedCharacterMaterial3;
    private Color primarycolor;
    private Color secondarycolor;
    private Color tertiarycolor;

    private GameObject modelTorso;
    private GameObject modelLimbs;
    private GameObject modelJoints;

    public void NewCharacter()
    {
        GameObject newCharacter = Instantiate(selectedCharacterPrefab) as GameObject;
        newCharacter.transform.SetParent(charactersParent.transform, false);
        newCharacter.name = "SelectedChar";

        selectedCharacterDisplayModel = newCharacter;

        selectedCharacterView = new RenderTexture(512, 515, 24, RenderTextureFormat.ARGB32);
        selectedCharacterView.Create();

        charactersParent.transform.FindChild("SelectedChar").FindChild("cam").GetComponent<Camera>().targetTexture = selectedCharacterView;
        transform.FindChild("Image").GetComponent<RawImage>().texture = selectedCharacterView;

        modelTorso = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighTorsoGeo").gameObject;
        modelLimbs = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighLimbsGeo").gameObject;
        modelJoints = selectedCharacterDisplayModel.transform.Find("Graphics").Find("Model").Find("BetaHighResMeshes").Find("Beta_HighJointsGeo").gameObject;

        selectedCharacterMaterial1 = new Material(modelTorso.GetComponent<Renderer>().material);
        selectedCharacterMaterial2 = new Material(modelJoints.GetComponent<Renderer>().material);
        selectedCharacterMaterial3 = new Material(modelLimbs.GetComponent<Renderer>().material);

        modelTorso.GetComponent<Renderer>().material = selectedCharacterMaterial1;
        modelJoints.GetComponent<Renderer>().material = selectedCharacterMaterial2;
        modelLimbs.GetComponent<Renderer>().material = selectedCharacterMaterial3;
    }

    public void UpdateColor1(Text colorText)
    {
        int hexColor;

        if (colorText.text.Length != 7 || !int.TryParse(colorText.text.Replace("#", ""), out hexColor))
        {
            return;
        }

        int r = int.Parse(hexColor.ToString().Remove(3, 2), NumberStyles.HexNumber);
        int g = int.Parse(hexColor.ToString().Remove(0, 2).Remove(3, 2), NumberStyles.HexNumber);
        int b = int.Parse(hexColor.ToString().Remove(0, 4), NumberStyles.HexNumber);

        primarycolor = new Color(r, g, b);
        selectedCharacterMaterial1.color = primarycolor;
        modelTorso.GetComponent<Renderer>().material = selectedCharacterMaterial1;
    }

    public void UpdateColor2(Text colorText)
    {
        int hexColor;

        if (colorText.text.Length != 7 || !int.TryParse(colorText.text.Replace("#", ""), out hexColor))
        {
            return;
        }

        int r = int.Parse(hexColor.ToString().Remove(3, 2), NumberStyles.HexNumber);
        int g = int.Parse(hexColor.ToString().Remove(0, 2).Remove(3, 2), NumberStyles.HexNumber);
        int b = int.Parse(hexColor.ToString().Remove(0, 4), NumberStyles.HexNumber);

        secondarycolor = new Color(r, g, b);
        selectedCharacterMaterial2.color = secondarycolor;
        modelJoints.GetComponent<Renderer>().material = selectedCharacterMaterial2;
    }

    public void UpdateColor3(Text colorText)
    {
        int hexColor;

        if (colorText.text.Length != 7 || !int.TryParse(colorText.text.Replace("#", ""), out hexColor))
        {
            return;
        }

        int r = int.Parse(hexColor.ToString().Remove(3, 2), NumberStyles.HexNumber);
        int g = int.Parse(hexColor.ToString().Remove(0, 2).Remove(3, 2), NumberStyles.HexNumber);
        int b = int.Parse(hexColor.ToString().Remove(0, 4), NumberStyles.HexNumber);

        tertiarycolor = new Color(r, g, b);
        selectedCharacterMaterial3.color = tertiarycolor;
        modelLimbs.GetComponent<Renderer>().material = selectedCharacterMaterial3;
    }
}
