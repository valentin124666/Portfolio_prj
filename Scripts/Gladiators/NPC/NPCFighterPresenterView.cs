using Core;
using General;
using Gladiators.AnimatorCustom;
using Settings;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Gladiators.NPC
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.FighterGladiator)]
    public class NPCFighterPresenterView : SimplePresenterView<NPCFighterPresenter, NPCFighterPresenterView>
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _HPBar;
        [SerializeField] private Transform _posMasssge;

        [SerializeField] private AvatarSelection _avatarSelection;

        [SerializeField] private ImageAlpha _imageAlpha;

        public ImageAlpha imageAlpha => _imageAlpha;

        private Color _mainColor;

        public Vector3 PosMessage => _posMasssge.position;
        public Quaternion RotationMessage => _posMasssge.rotation;

        public override void Init()
        {
            _HPBar.fillAmount = 1;

            _imageAlpha.Hide(true);
        }
        
        public void SetColor(Color newColor)
        {
            _avatarSelection.SetColor(newColor);
        }

        public void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        public void SetCurrentAvatar(int id)
        {
            _avatarSelection.SetCurrentAvatar(id);
            Presenter.GetModule<FighterAnimatorModule>().SetAnimator(id);

        }
        public void SetSwordModel(int id) => _avatarSelection.SetSwordModel(id);
        public void SetShieldModel(int id) => _avatarSelection.SetShieldModel(id);
        public void SetHelmetModel(int id) => _avatarSelection.SetHelmetModel(id);
        public void SetName(string name) => _name.text = name;
        public void SetHP(float hpBar)=>_HPBar.fillAmount = hpBar;

        private void BlinkMesh()
        {
            _avatarSelection.BlinkMesh();
        }

        public void DamageEffect()
        {
            BlinkMesh();

        }
    }
}