using Settings;

namespace Gladiators.Interfaces
{
    public interface IStateModule
    {
        Enumerators.UpdateType UpdateType { get; }
        void UpdateCustom();
    }
    
}
