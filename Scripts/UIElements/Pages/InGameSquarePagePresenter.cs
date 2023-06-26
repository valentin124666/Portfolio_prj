using Managers.Interfaces;

namespace UIElements.Pages
{
    public class InGameSquarePagePresenter : SimpleUIPresenter<InGameSquarePagePresenter,InGameSquarePagePresenterView>,IUIElement
    {
        public InGameSquarePagePresenter(InGameSquarePagePresenterView view) : base(view)
        {
            View.HideCharacteristics();
        }

        public void MonetaryOperations(int money)=>View.UpdateCoinCounter(money);
        public void UpdateLevelCounter(int level) => View.UpdateLevelCounter(level);
        public void UpdateCharacteristics(int health, int attack, int defense,bool showDifference = false) => View.UpdateCharacteristics(health, attack, defense,showDifference);
        
        public void SetActiveCharacteristics(bool isActive)
        {
            if (isActive)
            {
                View.ShowCharacteristics();
            }
            else
            {
                View.HideCharacteristics();
            }
        }

        public void Show()
        {
            SetActive(true);
        }

        public void Hide()
        {
            SetActive(false);
        }

        public void Reset()
        {
           
        }
    }
}
