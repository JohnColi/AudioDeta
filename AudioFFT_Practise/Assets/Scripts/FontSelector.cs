using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A simple UI to display a selection of OS fonts and allow changing the UI font to any of them.
public class FontSelector : MonoBehaviour
{
    public List<Text> Texts;

    Vector2 scrollPos;
    string[] fonts;

    void Start()
    {
        fonts = Font.GetOSInstalledFontNames();
    }

    private void ChangeFont(Font font)
    {
        foreach (Text text in Texts)
        {
            text.font = font;
        }
    }

    void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var font in fonts)
        {
            if (GUILayout.Button(font))
            {
                Font f = Font.CreateDynamicFontFromOSFont(font, 12);
                GUI.skin.font = f;
                ChangeFont(f);
            }
        }
        GUILayout.EndScrollView();
    }
}