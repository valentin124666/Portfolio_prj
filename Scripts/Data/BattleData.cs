using System;
using Data.Characteristics;
using Managers;
using Settings;
using Tools;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/BattleData")]
    public class BattleData : ScriptableObject
    {
        public float MarkerMovementTime;
        public int RepetitionFighterStatesMax;
        public float CooldownAttack;
        public float CooldownProtection;
        public float CooldownDodge;

        public int MaxNumberOpponent => _npcFighterData.Length;
        [SerializeField] private NPCFighterData[] _npcFighterData;

        public NPCFighterData GetNPCFighterData(int id)
        {
            var i = id > _npcFighterData.Length - 1 ? UnityEngine.Random.Range(_npcFighterData.Length - 6, _npcFighterData.Length) : id;
            return _npcFighterData[i];
        }

        public void UpdateEnemyDatabase()
        {
            var npcFighterDataUtility = InternalTools.GetTypeObject<NPCFighterDataUtility>(
                GameDataManager.LoadJsonOrNull(typeof(NPCFighterDataUtility)));

            _npcFighterData = new NPCFighterData[npcFighterDataUtility.Enemies.Length];

            for (var i = 0; i < npcFighterDataUtility.Enemies.Length; i++)
            {
                _npcFighterData[i] = GetNPCFighterData(npcFighterDataUtility.Enemies[i]);
            }
        }

        public NPCFighterData GetNPCFighterData(NPCFighterDataParameterUtility parameterUtility)
        {
            return new NPCFighterData()
            {
                Name = parameterUtility.name,
                Health = parameterUtility.healthPoints,
                Armor = parameterUtility.defense,
                Attack = parameterUtility.attack,
                AttackDuration = parameterUtility.attackDuration,
                ProtectionDuration = parameterUtility.protectionDuration,
                IdleDuration = parameterUtility.idleDuration,
                Money = parameterUtility.money,
                Exp = parameterUtility.exp,
                WeaponModel = parameterUtility.weaponModel,
                ShieldModel = parameterUtility.shieldModel,
                HelmetModel = parameterUtility.helmetModel,
                Archetype = Enumerators.NPCArchetype.Normal,
                IdColor = parameterUtility.IdColor,
                IdAvatar = parameterUtility.IdAvatar,
                IsBoss = parameterUtility.IsBoss
            };
        }
    }

    [DataName("EnemyData")]
    public class NPCFighterDataUtility
    {
        public NPCFighterDataParameterUtility[] Enemies;
    }

    [Serializable]
    public class NPCFighterDataParameterUtility
    {
        public int id;
        public string name;
        public float healthPoints;
        public float attack;
        public float defense;
        public float speed;
        public float attackDuration;
        public float protectionDuration;
        public float idleDuration;
        public int money;
        public int exp;
        public int weaponModel;
        public int shieldModel;
        public int helmetModel;
        public int IdColor;
        public int IdAvatar;
        public bool IsBoss;
    }
}