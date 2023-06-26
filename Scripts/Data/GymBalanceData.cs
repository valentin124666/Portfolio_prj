using System;
using System.Collections.Generic;
using System.Linq;
using Data.Characteristics;
using Managers;
using Settings;
using Tools;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/GymBalanceData")]
    public class GymBalanceData : ScriptableObject
    {
        public GymBalance Attack;
        public GymBalance Defense;
        public GymBalance Health;

        public void UpdateGymDatabase()
        {
            var gymBalanceDataUtility = InternalTools.GetTypeObject<GymBalanceDataUtility>(
                GameDataManager.LoadJsonOrNull(typeof(GymBalanceDataUtility)));

            var balanceData = new List<GymBalanceDataParameterUtility>(gymBalanceDataUtility.Training);

            var attack = balanceData.FindAll(item => item.type == "attack");
            var defense = balanceData.FindAll(item => item.type == "defense");
            var health = balanceData.FindAll(item => item.type == "health");

            Attack.Balances = new Balance[attack.Count];

            for (var i = 0; i < attack.Count; i++)
            {
                Attack.Balances[i] = GetBalance(attack[i]);
            }
            
            Defense.Balances = new Balance[defense.Count];

            for (var i = 0; i < defense.Count; i++)
            {
                Defense.Balances[i] = GetBalance(defense[i]);
            }
            
            Health.Balances = new Balance[health.Count];

            for (var i = 0; i < health.Count; i++)
            {
                Health.Balances[i] = GetBalance(health[i]);
            }
        }

        private Balance GetBalance(GymBalanceDataParameterUtility parameterUtility)
        {
            return new Balance()
            {
                level = parameterUtility.level,
                cost = parameterUtility.price,
                limit = parameterUtility.bonus
            };
        }
    }

    [DataName("TrainingData")]
    public class GymBalanceDataUtility
    {
        public GymBalanceDataParameterUtility[] Training;
    }

    [Serializable]
    public class GymBalanceDataParameterUtility
    {
        public string type;
        public int level;
        public int price;
        public int bonus;
    }
}