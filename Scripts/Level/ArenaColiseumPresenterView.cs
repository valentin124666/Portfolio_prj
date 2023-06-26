using System.Collections.Generic;
using System.Linq;
using Core;
using General;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Level
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.ArenaColiseum)]
    public class ArenaColiseumPresenterView : SimplePresenterView<ArenaColiseumPresenter, ArenaColiseumPresenterView>
    {
        [SerializeField] private Transform _cameraPos;
        [SerializeField] private Transform _posPlayerSpawn;
        [SerializeField] private Transform _posNPCSpawn;
        
        public override void Init()
        {
          
        }

        public Transform GetCameraAnchor() => _cameraPos;
        public Transform GetPosPlayerSpawn() => _posPlayerSpawn;
        public Transform GetPointSpawnFighter() => _posNPCSpawn;


    }
}