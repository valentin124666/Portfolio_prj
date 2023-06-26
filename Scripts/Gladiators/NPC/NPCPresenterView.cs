using Core;
using Data;
using Level.Interactive;
using Settings;
using UnityEngine;
using DG.Tweening;
using General;
using Managers.Controller;
using Managers.Interfaces;

namespace Gladiators.NPC
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.PassantGladiator)]
    public class NPCPresenterView : SimplePresenterView<NPCPresenter, NPCPresenterView>
    {
        [SerializeField] private Renderer _mesh;
        [SerializeField] private Transform _bag;
        [SerializeField] private Emotions _emotions;
        private PoolController _poolController;
        private NPCController _npcController;
        
        public override void Init()
        {
            _poolController = GameClient.Get<IGameplayManager>().GetController<PoolController>();
            _npcController = GameClient.Get<IGameplayManager>().GetController<NPCController>();
            _mesh.material = GameClient.Get<IGameDataManager>().GetDataScriptable<ColorNPC>().GetRandomMaterialPresenter();
        }

        public void ActivationEmotions(Enumerators.EmotionsType typeActive)
        {
            _emotions.ActivationEmotions(typeActive);
        }

        public void Buy(NPCInteractiveObject npcInteractiveObject,TweenCallback callback)
        {
            var product = npcInteractiveObject.GetProduct();
            
            var cost = product.Cost;
            product.transform.DOScale(Vector3.zero,_npcController.ProductFlySpeed);
            product.transform.DOMove(_bag.position, _npcController.ProductFlySpeed)
                .OnComplete(() =>
                {
                    product.ReturnToPool();
                    var takeCoin = DOTween.Sequence();
                    takeCoin.AppendCallback(() =>
                    {
                        var coin = _poolController.GetPoolObj<ReceptacleObject>(Enumerators.NamePrefabAddressable.Coin);
                        var target = npcInteractiveObject.GetPlaceForMoney();
                        
                        coin.Activation();
                        coin.transform.SetParent(target);
                        coin.transform.SetPositionAndRotation(_bag.position, _bag.rotation);
                        npcInteractiveObject.TakeCoin(coin);

                        var position = target.position;
                        var path = new[] { (position + coin.transform.position) / 2 + Vector3.up * 1.5f, position };

                        coin.transform.DOPath(path, _npcController.CoinFlySpeed / 2, pathType: PathType.CatmullRom);
                    });
                    takeCoin.AppendInterval(_npcController.PauseBetweenEjectionCoin);
                    takeCoin.SetLoops(cost);
                    takeCoin.OnComplete(callback);
                });
        }
    }
}