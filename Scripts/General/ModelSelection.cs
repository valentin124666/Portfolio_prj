using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace General
{
    public class ModelSelection : MonoBehaviour
    {
        [SerializeField] private Model[] _model;
        private Model _current;

        public void SetModel(int id)
        {
            _current?.SetActive(false);

            var mod = _model.FirstOrDefault(item => item.Id == id) ?? _model[0];

            _current = mod;
            _current.SetActive(true);
        }

        public void DisableEverything()
        {
            foreach (var item in _model)
            {
                item.SetActive(false);
            }
        }

        public int GetCurrentId() => _current.Id;
    }

    [Serializable]
    public class Model
    {
        [SerializeField] private int id;
        public int Id => id;
        
        [SerializeField] private GameObject model;

        public void SetActive(bool isActive)
        {
            model.SetActive(isActive);
        }
    }
}