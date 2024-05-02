using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreadScripts.TextureUtility
{
    [System.Serializable]
    public class TextureAutoPackerData : TAPDreadData<TextureAutoPackerData>
    {
        public bool active;
        public List<TextureAutoPackerModule> activeModules = new List<TextureAutoPackerModule>();
        private static readonly string SavePath = "Assets/DreadScripts/Saved Data/Texture Utility/TextureAutoPackerData.asset";
        public static TextureAutoPackerData GetInstance()
        {
            return GetInstance(SavePath);
        }
    }
}
