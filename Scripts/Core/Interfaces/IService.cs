using Cysharp.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IService
    {
        UniTask Init();
    }
}