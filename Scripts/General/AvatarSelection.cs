using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace General
{
    public class AvatarSelection : MonoBehaviour
    {
        [SerializeField] private Avatar[] _avatars;
        private Avatar _current;
        public Transform TransformParticleStun => _current?.transformParticleStun;
        private float _timeBlink;

        private void Awake()
        {       
            _timeBlink = 0.3f;

            foreach (var avatar in _avatars)
            {
                avatar.Mesh.material = new Material(avatar.Mesh.material);
            }
        }

        public void SetColor(Color newColor) => _current.SetColor(newColor);
        public void BlinkMesh() => _current.BlinkMesh(_timeBlink);

        public void SetCurrentAvatar(int id)
        {
            _current?.SetActive(false);
            _current = _avatars.FirstOrDefault(item => item.Id == id) ?? _avatars[0];
            _current.SetActive(true);
        }
        
        public void SetSwordModel(int id)=>_current?.SetSwordModel(id);

        public void SetShieldModel(int id)=>_current?.SetShieldModel(id);

        public void SetHelmetModel(int id)=>_current?.SetHelmetModel(id);

        [Serializable]
        private class Avatar
        {
            [SerializeField] private int id;
            public int Id => id;

            [SerializeField] private GameObject _avatar;
            [SerializeField] private Renderer _mesh;
            public Renderer Mesh => _mesh;
            private Color _mainColor;


            [SerializeField] private ModelSelection _swordModel;
            [SerializeField] private ModelSelection _shieldModel;
            [SerializeField] private ModelSelection _helmetModel;
            public Transform transformParticleStun;
            
            public void SetColor(Color newColor)
            {
                _mainColor = newColor;
                _mesh.material.color = newColor;
            }
            public void BlinkMesh(float time)
            {
                _mesh.material.DOKill();
                _mesh.material.DOColor(Color.red, time).OnComplete(() => _mesh.material.DOColor(_mainColor, time));
            }

            public void SetActive(bool isActive)
            {
                _avatar.SetActive(isActive);
            }
            
            public void SetSwordModel(int id)
            {
                _swordModel.SetModel(id);
            }

            public void SetShieldModel(int id)
            {
                _shieldModel.SetModel(id);
            }

            public void SetHelmetModel(int id)
            {
                _helmetModel.SetModel(id);
            }


        }
    }
}
