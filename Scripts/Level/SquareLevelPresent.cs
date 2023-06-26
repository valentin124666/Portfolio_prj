using System;
using System.Collections.Generic;
using Core;
using Level.Interactive;
using Settings;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Level
{
    public sealed class SquareLevelPresent : SimplePresenter<SquareLevelPresent, SquareLevelPresentView>, ILevel
    {
        public SquareLevelPresent(SquareLevelPresentView view) : base(view)
        {
            SetActive(false);
        }

        public PointInterest GetRandomPoint(Enumerators.PlaceInterestType typePoint)
        {
            var pointsInterest = View.GetPointsInterest(typePoint);
            if (pointsInterest == null)
            {
                Debug.LogError("[SquareLevelPresent][GetPoint] There is no such point type");
                throw new NullReferenceException();
            }

            return pointsInterest.GetFreePlace();
        }

        public Transform GetPoint(Enumerators.PlaceInterestType typePoint, int number)
        {
            var pointsInterest = View.GetPointsInterest(typePoint);
            if (pointsInterest == null)
            {
                Debug.LogError("[SquareLevelPresent][GetPoint] There is no such point type");
                throw new NullReferenceException();
            }

            return pointsInterest.Points[number];
        }

        public Transform GetPointTransform(Enumerators.PlaceInterestType typePoint)
        {
            var pointsInterest = View.GetPointsInterest(typePoint);
            if (pointsInterest == null)
            {
                Debug.LogError("[SquareLevelPresent][GetPoint] There is no such point type");
                throw new NullReferenceException();
            }

            return pointsInterest.GetPointsOrder();
        }

        public bool FreeSpaceRack(ref NPCInteractiveObject rack)
        {
            rack = View.GetFreeSpaceRack();
            return rack != null;
        }

        public Transform GetCameraAnchor() => View.GetCameraAnchor();
        public Transform GetPosPlayerSpawn() => View.GetPosPlayerSpawn();
        public void ActivationPurchase(Enumerators.InteractiveObjectName purchaseName, bool isActive) => View.ActivationPurchase(purchaseName, isActive);
        public WorkbenchPurchaseView GetPurchase(Enumerators.InteractiveObjectName purchaseName) => View.GetPurchase(purchaseName);

        public NPCInteractiveObject GetRack(Enumerators.InteractiveObjectName type) => View.GetRack(type);
        
        public Enumerators.InteractiveObjectName ActivationPurchase(WorkbenchPurchaseView purchaseView)
        {
            var purchase = View.GetPurchase(purchaseView);

            View.ActivationRack(purchase.objectPurchase.InteractiveName, true);
            purchase.objectPurchase.SetActive(true, true);
            purchase.workbenchPurchase.SetActive(false);
            return purchase.objectPurchase.InteractiveName;
        }

        public void LockLocation(Enumerators.LockLocation lockType, bool isLock) => View.LockLocation(lockType, isLock);
    }

    [Serializable]
    public class PointsInterest
    {
        public Enumerators.PlaceInterestType Type;

        [FormerlySerializedAs("Points")] [SerializeField]
        private Transform[] points;

        public Transform[] Points => points;

        private List<PointInterest> _freePlace;

        private int currentPoint;

        public int Count => _freePlace.Count;

        public void Init()
        {
            _freePlace = new List<PointInterest>();

            for (int i = 0; i < points.Length; i++)
            {
                _freePlace.Add(new PointInterest(points[i], this));
            }
        }

        public Transform GetPointsOrder()
        {
            var point = points[currentPoint];

            currentPoint++;

            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }

            return point;
        }

        public PointInterest GetFreePlace()
        {
            if (_freePlace.Count == 0)
            {
                return new PointInterest(points[Random.Range(0, points.Length)], this);
            }

            var item = _freePlace[Random.Range(0, _freePlace.Count)];
            _freePlace.Remove(item);

            return item;
        }

        public void ReturnPlace(PointInterest item)
        {
            if (item != null && !_freePlace.Contains(item))
                _freePlace.Add(item);
        }
    }

    [Serializable]
    public class PointInterest
    {
        public Transform Points;
        private PointsInterest _main;

        public PointInterest(Transform point, PointsInterest pointsInterest)
        {
            _main = pointsInterest;
            Points = point;
        }

        public void ReturnToFreeList()
        {
            _main.ReturnPlace(this);
        }
    }

    [Serializable]
    public struct Purchase
    {
        public InteractiveObject objectPurchase;
        public WorkbenchPurchaseView workbenchPurchase;
    }
}