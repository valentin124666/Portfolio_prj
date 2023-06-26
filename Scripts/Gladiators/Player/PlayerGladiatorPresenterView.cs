using System;
using Core;
using DG.Tweening;
using Level.Interactive;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements.Pages;
using UnityEngine;

namespace Gladiators.Player
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.PlayerGladiator)]
    public class PlayerGladiatorPresenterView : SimplePresenterView<PlayerGladiatorPresenter, PlayerGladiatorPresenterView>
    {
        private InteractiveObject _interactive;
        private FighterZone _fighterZone;
        [SerializeField] private GameObject _hummer;
        [SerializeField] private GameObject _barbell;
        [SerializeField] private GameObject _shield;

        private InGameSquarePagePresenter _inGameSquarePage;

        public override void Init()
        {
            MainApp.Instance.FixedUpdateEvent += FixedUpdateCustom;
            _inGameSquarePage = GameClient.Get<IUIManager>().GetPage<InGameSquarePagePresenter>();
            _hummer.SetActive(false);
            _barbell.SetActive(false);
            _shield.SetActive(false);
        }


        private void FixedUpdateCustom()
        {
            if (_interactive == null || !_interactive.FinishedWorking) return;
            
            Presenter.DisableInteractionState();
            EndInteraction();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactiveObject = other.GetComponent<InteractiveObject>();
            if (interactiveObject != null)
            {
                if (interactiveObject.AllowAccess)
                {
                    _interactive = interactiveObject;

                    _interactive.ZoneActivation()
                        .OnComplete(() =>
                        {
                            var interactive = _interactive;
                            Presenter.EnableInteractionState(_interactive, () => interactive.Interaction());
                            if (_interactive.GetType() == typeof(WorkbenchPurchaseView)) return;

                            switch (_interactive.InteractiveName)
                            {
                                case Enumerators.InteractiveObjectName.WorkbenchSword:
                                    _hummer.SetActive(true);
                                    break;
                                case Enumerators.InteractiveObjectName.WorkbenchShield:
                                    break;
                                case Enumerators.InteractiveObjectName.TrainingApparatusBarbell:
                                    _barbell.SetActive(true);
                                    break;
                                case Enumerators.InteractiveObjectName.TrainingApparatusMakiwara:
                                    _shield.SetActive(true);
                                    break;
                                case Enumerators.InteractiveObjectName.TrainingApparatusPunchingBag:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });
                }
            }

            _fighterZone = other.GetComponent<FighterZone>();
            if (_fighterZone != null)
            {
                _fighterZone.ZoneActivation().OnComplete(() =>
                {
                    GameClient.Instance.GetService<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGameArena);
                    TaskManager.ExecuteAfterDelay(0.5f, () => _fighterZone.EndInteraction());
                });
            }

            if (other.CompareTag("Gym"))
            {
                _inGameSquarePage.SetActiveCharacteristics(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactive != null
                && other.gameObject == _interactive.gameObject
                && Presenter.CurrentState != Enumerators.PlayerStates.Interaction)
            {
                EndInteraction();
            }

            if (_fighterZone != null && _fighterZone.gameObject == other.gameObject)
            {
                _fighterZone.EndInteraction();
                _fighterZone = null;
            }

            if (other.CompareTag("Gym"))
            {
                _inGameSquarePage.SetActiveCharacteristics(false);
            }
        }

        public void EndInteraction()
        {
            if (_interactive == null) return;

            _hummer.SetActive(false);
            _barbell.SetActive(false);
            _shield.SetActive(false);

            _interactive.EndInteraction();
            _interactive = null;
        }
    }
}