using Cysharp.Threading.Tasks;

namespace Managers.Interfaces
{
    public interface IController 
    {
        bool IsInit { get; }

        void Init();
    }
}
