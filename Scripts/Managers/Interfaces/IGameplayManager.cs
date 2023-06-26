using Cysharp.Threading.Tasks;
using Settings;

namespace Managers.Interfaces
{
    public interface IGameplayManager 
    {
        T GetController<T>() where T : IController;

        Enumerators.AppState CurrentState { get; }
        bool IsPause { get;}
        void StartGameplay();
        void ChangeAppState(Enumerators.AppState stateTo);

    }
}
