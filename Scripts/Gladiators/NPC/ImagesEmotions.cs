using Core;
using DG.Tweening;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Gladiators.NPC
{
    public class ImagesEmotions : MonoBehaviour
    {
        [SerializeField] private Enumerators.EmotionsType _type;
        public Enumerators.EmotionsType Type => _type;
        private Vector3 _camPos;

        public void SetActive(bool isActive)
        {
            transform.DOKill();
            if (isActive)
            {
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;

                _camPos = GameClient.Get<IGameplayManager>().GetController<CameraController>().CameraPos;
                transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    transform.localScale = Vector3.one;
                });
            }
        }

        private void FixedUpdate()
        {
            if (gameObject.activeSelf)
                transform.rotation = Quaternion.LookRotation(transform.position - _camPos, Vector3.up);
        }
    }
}