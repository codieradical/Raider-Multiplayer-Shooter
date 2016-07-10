using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainmenuGUIHandler : GUISlidesHandler {

    public List<GUISlide> slides;

    public struct GUISlide
    {
        public GUISlide(int _width, int _height, GameObject _slide)
        {
            width = _width;
            height = _height;
            slide = _slide;
        }

        public string name
            { get { return slide.name; } private set { slide.name = value; } }
        public int width
            { get { return width; } private set { width = value; } }
        public int height
            { get { return height; } private set { height = value; } }
        public bool active
            { get { return slide.activeSelf; } set { slide.SetActive(value); } }
        public GameObject slide
            { get { return slide; } private set { slide = value; } }
    }

    public void GetGUISlidesFromGameScene()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            slides.Add(new GUISlide(0, 0, child));
        }
    }

    //If the user changes the resolution of the game, the menu needs to react to that.
    public void RecalculateSlideWidthHeight()
    {
    
    }

    public void MoveSlideOffScreen()
    {

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
