using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using TutorialStep = Settings.Enumerators.TutorialStep;

namespace Data.Characteristics
{
    [CreateAssetMenu(menuName = "Custom menu/Data/CharacteristicDataStarting")]
    public class CharacteristicDataStarting : ScriptableObject
    {
        [SerializeField] private PlayerDataJson playerData;
        [SerializeField] private NPCDataJson npcData;
        [SerializeField] private LevelDataJson levelDataJson;

        [SerializeField] private TutorialDataJson tutorialDataJson = new()
        {
            Steps = new StepTutorial[]
            {
                new() { name = TutorialStep.PurchaseOfAnAnvil, completed = false }, new() { name = TutorialStep.ShowStock, completed = false },
                new() { name = TutorialStep.DropMaterial, completed = false }, new() { name = TutorialStep.MachineWork, completed = false },
                new() { name = TutorialStep.TakeSwords, completed = false }, new() { name = TutorialStep.ShowRack, completed = false }, new() { name = TutorialStep.TakeMoney, completed = false },
                new() { name = TutorialStep.ShowColosseum, completed = false }, new() { name = TutorialStep.GreatLoss, completed = false }
            }
        };

        public IEnumerable<SimpleData> GetData()
        {
            return new SimpleData[]
            {
                playerData,
                npcData,
                levelDataJson,
                tutorialDataJson
            };
        }
    }
}