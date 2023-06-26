using System;
using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/ColorNPC")]
    public class ColorNPC : ScriptableObject
    {
        [SerializeField] private ColorType[] _colors;
        [SerializeField] private Material[] _materials;

        public Material GetRandomMaterialPresenter()
        {
            return _materials[UnityEngine.Random.Range(0,_materials.Length)];
        }
        
        public Color GetColorById(int id)
        {
            var color = _colors.FirstOrDefault(item => item.id == id)??_colors[0];

            return color.color;
        }
    
        [Serializable]
        private class ColorType
        {
            public int id;
            public Color color;
        }
    }
}
