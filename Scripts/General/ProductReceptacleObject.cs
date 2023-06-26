using Data.Characteristics;
using Settings;
using Tools;
using UnityEngine;

namespace General
{
    public class ProductReceptacleObject : ReceptacleObject
    {
        private readonly ModelSelection _receptacleModel;
        private ProductCharacteristics _productCharacteristics;
        public int Cost => _productCharacteristics.price;

        public ProductReceptacleObject(Enumerators.NamePrefabAddressable name, GameObject gameObject) : base(name, gameObject)
        {
            _receptacleModel = gameObject.GetComponent<ModelSelection>();
        }
        public override void Activation(params object[] args)
        {
            base.Activation(args);

            if (_receptacleModel == null) return;

            if (args.Length == 0)
                Debug.LogError("an object that tries to create without data about the model -name " + _receptacleModel.name);

            _productCharacteristics = InternalTools.GetTypeObject<ProductCharacteristics>(args[0]);

            _receptacleModel.SetModel(_productCharacteristics.idMod);
        }

        public override int GetCurrentIdMod() => _receptacleModel == null ? 0 : _receptacleModel.GetCurrentId();

    }
}
