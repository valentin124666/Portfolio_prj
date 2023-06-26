using Managers;
using Managers.Interfaces;

namespace Core
{
    public class GameClient : ServiceLocatorBase
    {
        private static GameClient _Instance;
        public static GameClient Instance
        {
            get
            {
                if (_Instance==null)
                {
                    _Instance = new GameClient();
                }
                return _Instance;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GameClient"/> class.
        /// </summary>

        internal GameClient(): base()
        {
            AddService<IGameDataManager>(new GameDataManager());
            AddService<IUIManager>(new UIManager());
            AddService<IGameplayManager>(new GameplayManager());
        }
        public static T Get<T>()
        {
            return Instance.GetService<T>();
        }
    }
}
