using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Tools
{
    public static class InternalTools
    {
        public static Sequence ActionDelayed(TweenCallback action, float delay = 0f)
        {
            var sequence = DOTween.Sequence();
            sequence.InsertCallback(delay,action);
            return sequence;
        }
        public static bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var results = new List<RaycastResult>();
            
            if (EventSystem.current == null)
                return false;
            
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static T GetTypeObject<T>(object obj)
        {
            var item = (T)obj;

            if (item != null)
            {
                return item;
            }

            throw new System.NullReferenceException();
        }

        public static bool[] GetProbability(int successRate)
        {
            var probability = new bool[100];
            var rate = new int[successRate];

            while (successRate>0)
            {
                var i = Random.Range(0, probability.Length);
                if(rate.Contains(i)) continue;

                rate[^successRate] = i;
                successRate--;
                probability[i] = true;
            }

            return probability;
        }

    }
}
