namespace Core.Interfaces
{
    public interface IView {
        IPresenter Instantiate(params object[] args);
        void SetPresenter(IPresenter presenter);
    }
}
