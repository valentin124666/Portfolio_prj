using Core;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements.Pages;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Level
{
    public sealed class ArenaColiseumPresenter : SimplePresenter<ArenaColiseumPresenter, ArenaColiseumPresenterView>, ILevel
    {
        public ArenaColiseumPresenter(ArenaColiseumPresenterView view) : base(view)
        {
            SetActive(false);
        }

        public Transform GetCameraAnchor() => View.GetCameraAnchor();
        public Transform GetPosPlayerSpawn() => View.GetPosPlayerSpawn();
        public Transform GetPointSpawnFighter() => View.GetPointSpawnFighter();
    }
}