using System.Collections;
using System.Collections.Generic;
using Data.Characteristics;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom menu/Data/EquipmentCharacteristicsData")]
public class EquipmentCharacteristicsData : ScriptableObject
{
    [SerializeField] private List<EquipmentCharacteristics> Sword;
    [SerializeField] private List<EquipmentCharacteristics> Shield;
        
    public EquipmentCharacteristics GetEquipmentSword(ushort id) => Sword.Find(item => item.idMod == id);
    public EquipmentCharacteristics GetEquipmentShield(ushort id) => Shield.Find(item => item.idMod == id);
}
