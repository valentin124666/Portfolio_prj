using Core;
using Settings;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using General;
using Level;
using Managers.Controller;
using Managers.Interfaces;

namespace Gladiators.Player
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.PlayerFighterGladiator)]
    public class PlayerFighterGladiatorPresenterView : SimplePresenterView<PlayerFighterGladiatorPresenter,PlayerFighterGladiatorPresenterView>
    {
        [SerializeField] private Image _HPBar;
        [SerializeField] private Transform _camPos;
        [SerializeField] private GameObject _handWithShield;
        [SerializeField] private Renderer _mesh;
        [SerializeField] private ModelSelection _swordModel;
        [SerializeField] private ModelSelection _shieldModel;
        
        private GameObject _hand;

        private float _timeBlink;
        private Color _mainColor;
        
        public override void Init()
        {
            var anchor = GameClient.Get<IGameplayManager>().GetController<LevelController>().GetLevel<ArenaColiseumPresenter>().GetCameraAnchor();
            anchor.SetParent(_camPos);
            anchor.SetPositionAndRotation(_camPos.position,_camPos.rotation);
            _HPBar.fillAmount = 1;
            _timeBlink = 0.3f;
            
            var material = _mesh.material;
            material = new Material(material);
            _mesh.material = material;
            _mainColor = material.color;
        }
        
        private void BlinkMesh()
        {
            _mesh.material.DOKill();
            _mesh.material.DOColor(Color.red, _timeBlink).OnComplete(()=>_mesh.material.DOColor(_mainColor, _timeBlink));
        }

        public void SetSwordModel(int id)
        {
            _swordModel.SetModel(id);
        }

        public void SetShieldModel(int id)
        {
            _shieldModel.SetModel(id);
        }

        public void SetHP(float hpBar)
        {
            _HPBar.fillAmount = hpBar;
        }

        public void RemoveArmShield()
        {          
            _hand = MonoBehaviour.Instantiate(_handWithShield, _handWithShield.transform.parent, true);
            
            _handWithShield.SetActive(false);

            _hand.transform.DOLocalMove(new Vector3(-1.2f, 2.9f, 0), 3f);
        }
        
        public void DamageEffect()
        {
            BlinkMesh();
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            _handWithShield.SetActive(true);

            if(_hand==null) return;
            
            _hand.transform.DOKill();
            Destroy(_hand);
            _hand = null;
        }
    }
}
