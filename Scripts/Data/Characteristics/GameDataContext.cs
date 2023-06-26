using System;
using System.Collections.Generic;
using System.Linq;
using Settings;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Characteristics
{
    public interface IData
    {
        string GetDataJson();
    }

    [Serializable]
    public class SimpleData
    {
    }

    #region Player

    [Serializable]
    [DataName("PlayerDataJson")]
    public class PlayerDataJson : SimpleData
    {
        public int PlayerBackpackSize;
        public int PlayerCurrentCoin;
        public int FameLevel;
        public int NumberNextFameLevel;

        public float Health;
        public int AddedHealth;
        public int Attack;
        public int Defense;

        public float AttackSpeed;
        public float PauseBeforeAttack;
        public float ProtectionTime;
        public float DodgeTime;
        public ushort IdModelShield;
        public ushort IdModelSword;
    }

    [Serializable]
    public class PlayerDataScriptable : SimpleData
    {
        public float PlayerMovementSpeed;
        public float SpeedMovementReceptacleObject;
        public float PauseBetweenEjectionReceptacleObject;
        public float FlightCurvatureReceptacleObject;
        public float AccelerationsProductionAClick;
        public float NPCStunTime;
        public int FactorHealthShield;
    }

    [Serializable]
    [DataInfo(typeof(PlayerDataJson), typeof(PlayerDataScriptable))]
    public class PlayerData : IData
    {
        public float PlayerMovementSpeed;
        public float SpeedMovementReceptacleObject;
        public float PauseBetweenEjectionReceptacleObject;
        public float FlightCurvatureReceptacleObject;
        public float AccelerationsProductionAClick;
        public float NPCStunTime;
        public int FactorHealthShield;

        public int PlayerBackpackSize;
        public int PlayerCurrentCoin;
        public int FameLevel;
        public int NumberNextFameLevel;

        public float Health;
        public int AddedHealth;
        public int Attack;
        public int Defense;

        public ushort IdModelShield;
        public ushort IdModelSword;
        public float AttackSpeed;
        public float PauseBeforeAttack;
        public float ProtectionTime;
        public float DodgeTime;

        public PlayerData(object playerDataJson, object playerDataScriptable)
        {
            var dataScriptable = InternalTools.GetTypeObject<PlayerDataScriptable>(playerDataScriptable);

            PlayerMovementSpeed = dataScriptable.PlayerMovementSpeed;
            SpeedMovementReceptacleObject = dataScriptable.SpeedMovementReceptacleObject;
            PauseBetweenEjectionReceptacleObject = dataScriptable.PauseBetweenEjectionReceptacleObject;
            FlightCurvatureReceptacleObject = dataScriptable.FlightCurvatureReceptacleObject;
            AccelerationsProductionAClick = dataScriptable.AccelerationsProductionAClick;
            NPCStunTime = dataScriptable.NPCStunTime;
            FactorHealthShield = dataScriptable.FactorHealthShield;
            
            var dataJson = InternalTools.GetTypeObject<PlayerDataJson>(playerDataJson);
            PlayerBackpackSize = dataJson.PlayerBackpackSize;
            PlayerCurrentCoin = dataJson.PlayerCurrentCoin;
            FameLevel = dataJson.FameLevel;
            NumberNextFameLevel = dataJson.NumberNextFameLevel;

            Health = dataJson.Health;
            Attack = dataJson.Attack;
            Defense = dataJson.Defense;
            AddedHealth = dataJson.AddedHealth;

            IdModelShield = dataJson.IdModelShield;
            IdModelSword = dataJson.IdModelSword;
            AttackSpeed = dataJson.AttackSpeed;
            PauseBeforeAttack = dataJson.PauseBeforeAttack;
            ProtectionTime = dataJson.ProtectionTime;
            DodgeTime = dataJson.DodgeTime;
        }

        public string GetDataJson()
        {
            var data = new PlayerDataJson();
            data.PlayerBackpackSize = PlayerBackpackSize;
            data.PlayerCurrentCoin = PlayerCurrentCoin;
            data.FameLevel = FameLevel;
            data.NumberNextFameLevel = NumberNextFameLevel;

            data.Health = Health;
            data.Attack = Attack;
            data.Defense = Defense;
            data.AddedHealth = AddedHealth;

            data.IdModelShield = IdModelShield;
            data.IdModelSword = IdModelSword;
            data.AttackSpeed = AttackSpeed;
            data.PauseBeforeAttack = PauseBeforeAttack;
            data.ProtectionTime = ProtectionTime;
            data.DodgeTime = DodgeTime;

            return JsonUtility.ToJson(data);
        }
    }

    #endregion

    #region NPC

    [Serializable]
    [DataName("NPCDataJson")]
    public class NPCDataJson : SimpleData
    {
        public int IdNPC;

        public string Name;

        public float Health;
        public float Armor;
        public float Attack;
        public float AttackDuration;
        public float ProtectionDuration;
        public float IdleDuration;
        public int Money;
        public int Exp;
        
        public int IdColor;
        public int IdAvatar;
        public bool IsBoss;

        public int WeaponModel;
        public int ShieldModel;
        public int HelmetModel;
        public int Archetype;
    }

    [Serializable]
    public class NPCDataScriptable : SimpleData
    {
        public float NPCTimeDoNothing;
        public float AttackSpeed;
        public int NPCPoolSize;
        public float BossSize;
        public int MinimumNumberNPCShop;
        public float ProductFlySpeed;
        public float CoinFlySpeed;
        public float PauseBetweenEjectionCoin;
    }

    [Serializable]
    [DataInfo(typeof(NPCDataJson), typeof(NPCDataScriptable))]
    public class NPCData : IData
    {
        public float NPCTimeDoNothing;
        public int NPCPoolSize;
        public int MinimumNumberNPCShop;
        public float ProductFlySpeed;
        public float CoinFlySpeed;
        public float PauseBetweenEjectionCoin;
        public float BossSize;

        public int IdNPC;
        public string Name;

        public float Health;
        public float Armor;
        public float Attack;
        public float AttackSpeed;
        public float AttackDuration;
        public float ProtectionDuration;
        public float IdleDuration;
        public int Money;
        public int Exp;
        
        public int IdColor;
        public int IdAvatar;
        public bool IsBoss;

        public int WeaponModel;
        public int ShieldModel;
        public int HelmetModel;
        public int Archetype;


        public NPCData(object npcDataJson, object npcDataScriptable)
        {
            var dataScriptable = InternalTools.GetTypeObject<NPCDataScriptable>(npcDataScriptable);

            NPCTimeDoNothing = dataScriptable.NPCTimeDoNothing;
            NPCPoolSize = dataScriptable.NPCPoolSize;
            MinimumNumberNPCShop = dataScriptable.MinimumNumberNPCShop;
            ProductFlySpeed = dataScriptable.ProductFlySpeed;
            CoinFlySpeed = dataScriptable.CoinFlySpeed;
            PauseBetweenEjectionCoin = dataScriptable.PauseBetweenEjectionCoin;
            BossSize = dataScriptable.BossSize;
            AttackSpeed = dataScriptable.AttackSpeed;

            var dataJson = InternalTools.GetTypeObject<NPCDataJson>(npcDataJson);

            IdNPC = dataJson.IdNPC;
            Name = dataJson.Name;

            Health = dataJson.Health;
            Armor = dataJson.Armor;
            Attack = dataJson.Attack;
            AttackDuration = dataJson.AttackDuration;
            ProtectionDuration = dataJson.ProtectionDuration;
            IdleDuration = dataJson.IdleDuration;
            Money = dataJson.Money;
            Exp = dataJson.Exp;
            
            IdColor = dataJson.IdColor;
            IdAvatar = dataJson.IdAvatar;
            IsBoss = dataJson.IsBoss;
            
            WeaponModel = dataJson.WeaponModel;
            ShieldModel = dataJson.ShieldModel;
            HelmetModel = dataJson.HelmetModel;
            Archetype = dataJson.Archetype;
        }

        public string GetDataJson()
        {
            var data = new NPCDataJson
            {
                IdNPC = IdNPC,
                Name = Name,
                Health = Health,
                Armor = Armor,
                Attack = Attack,
                AttackDuration = AttackDuration,
                ProtectionDuration = ProtectionDuration,
                IdleDuration = IdleDuration,
                Money = Money,
                Exp = Exp,
                IdColor = IdColor,
                IdAvatar = IdAvatar,
                IsBoss = IsBoss,
                WeaponModel = WeaponModel,
                ShieldModel = ShieldModel,
                HelmetModel = HelmetModel,
                Archetype = Archetype
            };

            return JsonUtility.ToJson(data);
        }
    }

    [Serializable]
    public class NPCFighterData
    {
        public string Name;

        public float Health;
        public float Armor;
        public float Attack;
        public float AttackDuration;
        public float ProtectionDuration;
        public float IdleDuration;
        public int Money;
        public int Exp;
        public int WeaponModel;
        public int ShieldModel;
        public int HelmetModel;
        public int IdColor;
        public int IdAvatar;
        public bool IsBoss;
        public Enumerators.NPCArchetype Archetype;
    }

    #endregion

    #region Level

    [Serializable]
    [DataName("LevelDataJson")]
    public class LevelDataJson : SimpleData
    {
        public BenchDataJson[] BenchData;
        public ReceptacleDataJson[] ReceptacleData;
    }

    [Serializable]
    public class LevelDataScriptable : SimpleData
    {
        public BenchDataScriptable[] BenchData;
    }

    [Serializable]
    [DataInfo(typeof(LevelDataJson), typeof(LevelDataScriptable))]
    public class LevelData : IData
    {
        public BenchData[] Benches;
        public List<ReceptacleDataJson> ReceptacleData;

        public LevelData(object leveDataJson, object leveDataScriptable)
        {
            var dataJson = InternalTools.GetTypeObject<LevelDataJson>(leveDataJson);
            var dataScriptable = InternalTools.GetTypeObject<LevelDataScriptable>(leveDataScriptable);
            Benches = new BenchData[dataScriptable.BenchData.Length];
            for (int i = 0; i < Benches.Length; i++)
            {
                var json = dataJson.BenchData.FirstOrDefault
                    (item => dataScriptable.BenchData[i].name == (Enumerators.InteractiveObjectName)item.idName);

                json.idName = (ushort)dataScriptable.BenchData[i].name;

                Benches[i] = new BenchData(json, dataScriptable.BenchData[i]);
            }

            if (dataJson.ReceptacleData == null)
                ReceptacleData = new List<ReceptacleDataJson>();
            else
                ReceptacleData = new List<ReceptacleDataJson>(dataJson.ReceptacleData);
        }

        public string GetDataJson()
        {
            var benchDataJson = new BenchDataJson[Benches.Length];

            for (int i = 0; i < benchDataJson.Length; i++)
            {
                benchDataJson[i] = new BenchDataJson
                {
                    idName = (ushort)Benches[i].name,
                    isActivation = Benches[i].isActivation,
                    idProduct = Benches[i].idProduct
                };
            }

            var data = new LevelDataJson
            {
                BenchData = benchDataJson,
                ReceptacleData = ReceptacleData.ToArray()
            };

            return JsonUtility.ToJson(data);
        }
    }

    [Serializable]
    public class ReceptacleDataJson
    {
        public string id;
        public int[] idMod;
    }

    [Serializable]
    public class BenchData
    {
        public Enumerators.InteractiveObjectName name;
        public bool isActivation;
        public ushort idProduct;
        public float timeZoneActivation;
        public ushort receptacleVolumeForMaterials;
        public ushort receptacleVolumeForProducts;


        public BenchData(BenchDataJson benchDataJson, BenchDataScriptable benchDataScriptable)
        {
            isActivation = benchDataJson.isActivation;
            idProduct = benchDataJson.idProduct;
            timeZoneActivation = benchDataScriptable.timeZoneActivation;
            receptacleVolumeForMaterials = benchDataScriptable.receptacleVolumeForMaterials;
            receptacleVolumeForProducts = benchDataScriptable.receptacleVolumeForProducts;
            name = benchDataScriptable.name;
        }
    }

    [Serializable]
    public class BenchDataJson
    {
        public ushort idName;
        public bool isActivation;
        public ushort idProduct;
    }

    [Serializable]
    public class BenchDataScriptable
    {
        public Enumerators.InteractiveObjectName name;
        public float timeZoneActivation;
        public ushort receptacleVolumeForMaterials;
        public ushort receptacleVolumeForProducts;
    }

    #endregion

    #region Tutorial

    [Serializable]
    [DataName("TutorialDataJson")]
    public class TutorialDataJson : SimpleData
    {
        public StepTutorial[] Steps;
    }

    [Serializable]
    public class TutorialDataScriptable : SimpleData
    {
    }

    [Serializable]
    [DataInfo(typeof(TutorialDataJson), typeof(TutorialDataScriptable))]
    public class TutorialData : IData
    {
        public List<StepTutorial> Steps;

        public TutorialData(object tutorialDataJson, object tutorialDataScriptable)
        {
            var dataJson = InternalTools.GetTypeObject<TutorialDataJson>(tutorialDataJson);
            Steps = new List<StepTutorial>();
            foreach (var item in dataJson.Steps)
            {
                Steps.Add(new StepTutorial()
                {
                    name = item.name,
                    completed = item.completed
                });
            }
        }


        public string GetDataJson()
        {
            return JsonUtility.ToJson(new TutorialDataJson() { Steps = Steps.ToArray() });
        }
    }

    [Serializable]
    public class StepTutorial
    {
        public Enumerators.TutorialStep name;
        public bool completed;
    }

    #endregion

    [Serializable]
    public class ProductCharacteristics
    {
        public string name;
        public ushort idMod;
        public Enumerators.NamePrefabAddressable prefab;
        public int price;
        public ushort level;
    }

    [Serializable]
    public class EquipmentCharacteristics
    {
        public string name;
        public ushort idMod;
        public float power;
        public ushort level;
    }

    [Serializable]
    public class GymBalance
    {
        public int step;
        public Balance[] Balances;

        public int GetCost(int currentBonus)
        {
            var data = Balances.FirstOrDefault(item => item.limit > currentBonus);

            return data?.cost ?? 1000000;
        }

        public int GetLimit(int level)
        {
            var data = Balances.FirstOrDefault(item => item.level == level);

            return data?.limit ?? 1000000;
        }
        public int GetComingChange(int limit)
        {
            var data = Balances.FirstOrDefault(item => item.limit > limit);

            return data?.level ?? 1000000;
        }
    }

    [Serializable]
    public class Balance
    {
        public int level;
        public int cost;
        public int limit;
    }
}