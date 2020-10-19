using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager
{
    private static SpriteManager _instance = null;
    public static SpriteManager Instance
    {
        get
        {
            if (_instance is null)
                _instance = new SpriteManager();
            return _instance;
        }
    }
    readonly Dictionary<SpriteAtlas_Name, string> spriteAtlasPathDic = new Dictionary<SpriteAtlas_Name, string>()
    {
        { SpriteAtlas_Name.Main,"SpriteAtlas/Main" },
        { SpriteAtlas_Name.RewardNoCash,"SpriteAtlas/RewardNoCash"},
        { SpriteAtlas_Name.Menu,"SpriteAtlas/Menu"},
        { SpriteAtlas_Name.BuyProp,"SpriteAtlas/BuyProp"},
    };
    readonly Dictionary<SpriteAtlas_Name, SpriteAtlas> loadedSpriteAtlasDic = new Dictionary<SpriteAtlas_Name, SpriteAtlas>();
    readonly Dictionary<string, Sprite> loadedSpriteDic = new Dictionary<string, Sprite>();
    public Sprite GetSprite(SpriteAtlas_Name spriteAtlas_Name,string sprite_name)
    {
        string spritePath = spriteAtlas_Name + "/" + sprite_name;
        if(loadedSpriteDic.TryGetValue(spritePath,out Sprite loadedSprite))
        {
            if (loadedSprite is null)
            {
                Debug.LogError("获取精灵图片错误：已加载精灵图片字典中存在该键，但对应的精灵图片为空！精灵图片的路径为：" + spritePath);
                loadedSpriteDic.Remove(spritePath);
                return null;
            }
            else
                return loadedSprite;
        }
        else
        {
            if(loadedSpriteAtlasDic.TryGetValue(spriteAtlas_Name,out SpriteAtlas loadedSpriteAtlas))
            {
                if(loadedSpriteAtlas is null)
                {
                    Debug.LogError("获取精灵图片错误：已经加载的精灵图集字典中存在该图集的键，但对应的图集为空！图集名称为：" + spriteAtlas_Name);
                    loadedSpriteAtlasDic.Remove(spriteAtlas_Name);
                    return null;
                }
                else
                {
                    Sprite targetSprite = loadedSpriteAtlas.GetSprite(sprite_name);
                    if(targetSprite is null)
                    {
                        Debug.LogError("获取精灵图片错误：精灵图集中不存在该名称的精灵图片！精灵图片的名称为：" + sprite_name);
                        return null;
                    }
                    else
                    {
                        loadedSpriteDic.Add(spritePath, targetSprite);
                        return targetSprite;
                    }
                }
            }
            else
            {
                if(spriteAtlasPathDic.TryGetValue(spriteAtlas_Name,out string spriteAtlasPath))
                {
                    if (string.IsNullOrEmpty(spriteAtlasPath))
                    {
                        Debug.LogError("获取精灵图片错误：精灵图片所在图集的路径为空！图集名称为：" + spriteAtlas_Name);
                        return null;
                    }
                    else
                    {
                        SpriteAtlas targetSpriteAtlas = Resources.Load<SpriteAtlas>(spriteAtlasPath);
                        if(targetSpriteAtlas is null)
                        {
                            Debug.LogError("获取精灵图片错误：请确认图集的路径是正确的！图集路径：" + spriteAtlasPath);
                            return null;
                        }
                        else
                        {
                            loadedSpriteAtlasDic.Add(spriteAtlas_Name, targetSpriteAtlas);
                            Sprite targetSprite = targetSpriteAtlas.GetSprite(sprite_name);
                            if (targetSprite is null)
                            {
                                Debug.LogError("获取精灵图片错误：精灵图集中不存在该名称的精灵图片！精灵图片的名称为：" + sprite_name);
                                return null;
                            }
                            else
                            {
                                loadedSpriteDic.Add(spritePath, targetSprite);
                                return targetSprite;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("获取精灵图片错误：精灵图集字典中不存在该图集的路径！图集名称为：" + spriteAtlas_Name);
                    return null;
                }
            }
        }
    }
}
public enum SpriteAtlas_Name
{
    Main,
    RewardNoCash,
    Menu,
    BuyProp,
}
