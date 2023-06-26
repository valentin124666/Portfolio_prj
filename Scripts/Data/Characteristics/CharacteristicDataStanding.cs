using UnityEngine;

namespace Data.Characteristics
{
    [CreateAssetMenu(menuName = "Custom menu/Data/CharacteristicDataStanding")]
    public class CharacteristicDataStanding : ScriptableObject
    {
        [SerializeField] private PlayerDataScriptable playerData;
        [SerializeField] private NPCDataScriptable npcData;
        [SerializeField] private LevelDataScriptable leveDataScriptable;
        [SerializeField] private TutorialDataScriptable tutorialDataScriptable;

        public SimpleData[] GetData()
        {
            return new SimpleData[]
            {
                playerData,
                npcData,
                leveDataScriptable,
                tutorialDataScriptable
            };
        }
    }
}