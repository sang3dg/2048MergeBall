using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

public class CreateFont : Editor
{
    [MenuItem("CreateFont/CreateNewFont")]
    public static void CreateNewFont()
    {
        FontWizard fontWizard = CreateInstance<FontWizard>();
        fontWizard.position = new Rect(0, 0, 500, 500);
        fontWizard.Show();
    }
}
public class FontWizard : ScriptableWizard
{
    public Font newFont;
    [Header("[All Text , Every fileName need be text content.]")]
    public List<Sprite> fontSprites;
    public Material fontMaterial;
    private void OnWizardCreate()
    {
        fontMaterial.shader = Shader.Find("GUI/Text Shader");
        fontMaterial.SetTexture("_MainTex", fontSprites[0].texture);
        fontMaterial.SetColor("_MainColor", Color.white);
        Texture spriteTex = fontSprites[0].texture;
        float totalWidth = spriteTex.width;
        float totalHeight = spriteTex.height;
        float maxHeight = 0;
        float maxWidth = 0;
        int textCount = fontSprites.Count;
        CharacterInfo[] characters = new CharacterInfo[textCount];
        for(int i = 0; i < textCount; i++)
        {
            CharacterInfo character = new CharacterInfo();
            Sprite textSp = fontSprites[i];
            string name = textSp.name;
            int asciiIndex = Encoding.ASCII.GetBytes(name)[0];
            float posX = textSp.rect.x;
            float posY = textSp.rect.y;
            float width = textSp.rect.width;
            float height = textSp.rect.height;
            if (width > maxWidth)
                maxWidth = width;
            if (height > maxHeight)
                maxHeight = height;

            character.index = asciiIndex;
            character.uv = new Rect(posX / totalWidth, posY / totalHeight, width / totalWidth, height / totalHeight);
            character.vert = new Rect(0, 0, width, -height);
            character.advance = (int)width;
            characters[i] = character;
        }
        newFont.characterInfo = characters;
        Debug.Log("maxHeight = " + maxHeight + " , maxWidth = " + maxWidth);
    }
}
