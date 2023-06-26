using System;
using System.Collections.Generic;
using System.Linq;
using Data.Characteristics;
using Settings;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/ProductCharacteristicsData")]
    public class ProductCharacteristicsData : ScriptableObject
    {
        [SerializeField] private List<ProductCharacteristics> Sword;
        [SerializeField] private List<ProductCharacteristics> Shield;

        public ProductCharacteristics GetProductByLevel(int level, Enumerators.ReceptacleObjectType productType)
        {
            ProductCharacteristics[] products;
            switch (productType)
            {
                case Enumerators.ReceptacleObjectType.Sword:
                    products = Sword.ToArray();
                    break;
                case Enumerators.ReceptacleObjectType.Shield:
                    products = Shield.ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(productType), productType, null);
            }

            foreach (var data in products)
            {
                if (data.level != level) continue;
                return data;
            }

            for (var i = 1; i < products.Length; i++)
            {
                if (level < products[i].level && level > products[i - 1].level)
                    return products[i - 1];
            }

            return products[^1];
        }

        public ProductCharacteristics GetProductById(int id, Enumerators.ReceptacleObjectType productType)
        {
            ProductCharacteristics[] products;
            switch (productType)
            {
                case Enumerators.ReceptacleObjectType.Sword:
                    products = Sword.ToArray();
                    break;
                case Enumerators.ReceptacleObjectType.Shield:
                    products = Shield.ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(productType), productType, null);
            }

            return products.First(item => item.idMod == id);
        }
    }
}