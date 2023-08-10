using GameteqTestTaskC.Modules;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GameteqTestTaskC.Pages
{
    // Base abstract class for pages
    internal abstract class BasePage
    {
        protected IWebDriver _driver;

        private MenuModule _menu;

        public BasePage(IWebDriver driver)
        { 
            _driver = driver;

            _menu = new MenuModule(driver);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            InitElements();
            
            Assert.That(wait.Until(condition => IsAt()), Is.True);
        }

        // Checker method for page display
        public abstract bool IsAt();

        // Toggle the menu
        public void ToggleMenu() => _menu.ToggleMenu();

        // Navigate to Dashboard page
        public DashboardPage NavigateToDashboardPage() => _menu.NavigateToDashboardPage();

        // Navigate to Offers page
        public OffersPage NavigateToOffersPage() => _menu.NavigateToOffersPage();

        // Init initial elements, mainly for IsAt checks
        protected abstract void InitElements();
    }
}
