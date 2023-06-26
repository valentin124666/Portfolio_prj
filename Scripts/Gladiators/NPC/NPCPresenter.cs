using System;
using System.Collections.Generic;
using Core;
using Gladiators.AnimatorCustom;
using Level;
using Level.Interactive;
using Managers.Controller;
using Settings;
using Tools;
using UnityEngine;
using NPCAnimations = Settings.Enumerators.NPCAnimations;
using Random = UnityEngine.Random;

namespace Gladiators.NPC
{
    public class NPCPresenter : SimplePresenter<NPCPresenter, NPCPresenterView>
    {
        public event Action<NPCPresenter> FinishedState;

        private readonly LevelController _levelController;
        private readonly NPCMoveView _npcMoveView;
        private readonly PassantAnimator _npcAnimatorView;
        private readonly NPCController _npcController;

        private NPCInteractiveObject _rackAtWhich;
        private PointInterest _targetMove;

        private List<INPCModule> _NPCModule;

        private bool _traceEndOfRoad;

        public Enumerators.NPCStates CurrentState;
        public Enumerators.PlaceInterestType PlaceInterestType { get; private set; }

        public NPCPresenter(NPCPresenterView view, LevelController levelController, NPCController npcController) : base(view)
        {
            _npcController = npcController;
            _levelController = levelController;
            _NPCModule = new List<INPCModule>();
            _NPCModule.AddRange(View.GetComponents<INPCModule>());
            _npcMoveView = GetModule<NPCMoveView>();
            _npcAnimatorView = GetModule<PassantAnimator>();

            foreach (var item in _NPCModule)
            {
                item.Init();
            }

            MainApp.Instance.FixedUpdateEvent += FixedUpdate;
        }

        private void FixedUpdate()
        {
            if (_traceEndOfRoad)
            {
                if (_npcMoveView.EndOfRoad())
                {
                    if (PlaceInterestType == Enumerators.PlaceInterestType.RackPosPlace)
                        _npcMoveView.SetAlignOnTarget();

                    _traceEndOfRoad = false;

                    FinishedState?.Invoke(this);
                }
            }
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);

            if (active)
            {
                StateMachine(CurrentState, _targetMove);
                MainApp.Instance.FixedUpdateEvent += FixedUpdate;
            }
            else
            {
                MainApp.Instance.FixedUpdateEvent -= FixedUpdate;
            }
        }

        private void StateMachine(Enumerators.NPCStates state, params object[] args)
        {
            CurrentState = state;
            switch (CurrentState)
            {
                case Enumerators.NPCStates.Buy:
                    View.Buy(_rackAtWhich, () => { FinishedState?.Invoke(this); });
                    _npcAnimatorView.SetAnimation(NPCAnimations.Buy.ToString());

                    break;
                case Enumerators.NPCStates.Movement:

                    _targetMove?.ReturnToFreeList();
                    _targetMove = InternalTools.GetTypeObject<PointInterest>(args[0]);

                    if (_targetMove == null)
                        throw new NullReferenceException();

                    _npcMoveView.SetTargetAndMove(_targetMove.Points);
                    _traceEndOfRoad = true;
                    _npcAnimatorView.SetAnimation(NPCAnimations.Walking.ToString());

                    break;
                case Enumerators.NPCStates.DoNothing:
                    switch (PlaceInterestType)
                    {
                        case Enumerators.PlaceInterestType.Unknown:
                            _npcAnimatorView.SetAnimation(NPCAnimations.Idle.ToString());
                            InternalTools.ActionDelayed(() => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing);

                            break;
                        case Enumerators.PlaceInterestType.SpawnPosPlace:
                            _npcAnimatorView.SetAnimation(NPCAnimations.Idle.ToString());
                            InternalTools.ActionDelayed(
                                () => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing - Random.Range(0, _npcController.NPCTimeDoNothing / 2));

                            break;
                        case Enumerators.PlaceInterestType.AlleyPosPlace:
                            _npcAnimatorView.SetAnimation(NPCAnimations.IdleAlley.ToString());
                            InternalTools.ActionDelayed(
                                () => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing * Random.Range(1, 4f));


                            break;
                        case Enumerators.PlaceInterestType.AlleyFrontShopPosPlace:
                            _npcAnimatorView.SetAnimation(NPCAnimations.IdleAlley.ToString());
                            InternalTools.ActionDelayed(
                                () => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing * Random.Range(1, 4f));

                            break;
                        case Enumerators.PlaceInterestType.ShopPosPlace:
                            _npcAnimatorView.SetAnimation(NPCAnimations.IdleShop.ToString());
                            InternalTools.ActionDelayed(
                                () => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing - Random.Range(0, _npcController.NPCTimeDoNothing / 4));


                            break;
                        case Enumerators.PlaceInterestType.RackPosPlace:
                            View.ActivationEmotions(Enumerators.EmotionsType.Evil);
                            _npcAnimatorView.SetAnimation(NPCAnimations.DidNotBuy.ToString());
                            InternalTools.ActionDelayed(
                                () => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing);

                            break;
                        default:
                            _npcAnimatorView.SetAnimation(NPCAnimations.Idle.ToString());
                            InternalTools.ActionDelayed(() => { FinishedState?.Invoke(this); }, _npcController.NPCTimeDoNothing);
                            break;
                    }

                    break;
            }
        }

        public void Buy()
        {
            if (_rackAtWhich == null)
            {
                Debug.LogError("There is no rack");
                return;
            }

            StateMachine(Enumerators.NPCStates.Buy);
        }

        public void ToWait()
        {
            StateMachine(Enumerators.NPCStates.DoNothing);
        }

        public bool ThereAreGoodsOnRack()
        {
            if (_rackAtWhich == null)
                return false;

            return _rackAtWhich.ThereAreGoods();
        }

        public void MoveAwayFromRack()
        {
            _rackAtWhich = null;
        }

        public void TakePlaceAndGoRack(NPCInteractiveObject rack)
        {
            _rackAtWhich = rack;
            PlaceInterestType = Enumerators.PlaceInterestType.RackPosPlace;

            StateMachine(Enumerators.NPCStates.Movement, _rackAtWhich.GetFreePlaceOrNull());
        }

        public void HeadTowardsTarget(Enumerators.PlaceInterestType place)
        {
            if (_rackAtWhich != null)
            {
                Debug.LogError("Gotta move away from the bar");

                MoveAwayFromRack();
            }

            PlaceInterestType = place;


            StateMachine(Enumerators.NPCStates.Movement, _levelController.GetPlaceInterest(PlaceInterestType));
        }

        private T GetModule<T>() where T : INPCModule
        {
            return (T)_NPCModule.Find(module => module is T);
        }
    }
}