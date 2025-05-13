using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class EmojiData
    {
        public EmojiType type;
        public GameObject prefab;
    }

    [CreateAssetMenu(fileName = "EmojiPrefabData", menuName = "Data/EmojiPrefabData")]
    public class EmojiPrefabData : ScriptableObject
    {
        public List<EmojiData> emojiDatas;

        public GameObject GetEmojiPrefab(EmojiType type)
        {
            foreach (EmojiData item in emojiDatas)
                if (item.type == type)
                    return item.prefab;
            return null;
        }
    }
}
