using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/UIData")]
    public class UIData : ScriptableObject
    {
        [SerializeField] private ColiseumPopupData _coliseumPopupData;
        public ColiseumPopupData coliseumPopupData => _coliseumPopupData;
        [SerializeField] private GymPopupData _gymPopupData;
        public GymPopupData gymPopupData => _gymPopupData;
    }

    [Serializable]
    public struct ColiseumPopupData
    {
        public string Massage;
    }
    [Serializable]
    public struct GymPopupData
    {
        public string MassageBarbell;
        public string MassageMakiwara;
        public string MassagePunchingBag;
    }

    [Serializable]
    public struct Tutorial
    {
        public string _appearance;
        
    }
}