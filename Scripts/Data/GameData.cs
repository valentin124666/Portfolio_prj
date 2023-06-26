using System;
using System.Collections.Generic;
using Data.Characteristics;
using Settings;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] private CameraData _cameraData;
        public CameraData cameraData => _cameraData;


        [SerializeField] private float _foldingScreenSpeed;
        public float FoldingScreenSpeed => _foldingScreenSpeed;
        [SerializeField] private float _delayBeforeTransitionLevels;
        public float DelayBeforeTransitionLevels => _delayBeforeTransitionLevels;

        [SerializeField] private List<PlayerUpgrade> _playerUpgrades;
        public PlayerUpgrade GetPlayerUpgrade(int level) => _playerUpgrades.Find(item => item.Level == level) ?? _playerUpgrades[^1];

        [SerializeField] private JoystickData _joystickData;
        public JoystickData joystickData => _joystickData;
    }

    [Serializable]
    public struct CameraData
    {
        public float CameraMovementSpeed;
        public float CameraFileOfViewArena;
        public float CameraFileOfViewSquare;

        public float TutorMoveSpeed;
        public float TutorRotSpeed ;
        public float TutorPause;
        
    }

    [Serializable]
    public struct JoystickData
    {
        public float JoystickSensitivity;
        public float JoystickMaxMagnitude;
        public float MaxDistansStick;
        public float BeginningMoveSmallStick;
    }


    [Serializable]
    public class PlayerUpgrade
    {
        public int Level;
        public int NumberNextLevel;
    }
}