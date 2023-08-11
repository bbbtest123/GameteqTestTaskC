using GameteqTestTaskC.Helpers;
using GameteqTestTaskC.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GameteqTestTaskC.Modules
{
    // A class for the menu
    internal class MenuModule
    {
        private const string _menuToggleXPath = "//label[@class='mat-slide-toggle-label']";
        private const string _menuInputXPath = _menuToggleXPath + "//input";
        private const string _menuSideNavXPath = "//mat-sidenav";
        private const string _buttonXPath = "//button[@class='mat-button mat-button-base' and span[contains(text(), '{0}')]]";
        private string _dashboardButtonXPath = String.Format(_buttonXPath, "Dashboard");
        private string _offersButtonXPath = String.Format(_buttonXPath, "Offers");

        private IWebDriver _driver;

        private Element _menuToggle;
        private Element _menuInput;
        private Element? _dashboardButton;
        private Element? _offersButton;

        public MenuModule(IWebDriver driver)
        {
            _driver = driver;

            _menuToggle = new Element(driver: _driver, locator: By.XPath(_menuToggleXPath));
            _menuInput = new Element(driver: _driver, locator: By.XPath(_menuInputXPath));
        }

        // Toggling the menu
        public void ToggleMenu()
        {
            if (!IsMenuShown())
            {
                _menuToggle.Click();

                Element menuSideNav = new(driver: _driver, locator: By.XPath(_menuSideNavXPath));

                string classString = menuSideNav.GetAttribute("class");

                new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(10))
                    .Until(condition => menuSideNav.GetAttribute("class").Contains("mat-drawer-opened"));

                _dashboardButton = new Element(driver: _driver, locator: By.XPath(_dashboardButtonXPath));
                _offersButton = new Element(driver: _driver, locator: By.XPath(_offersButtonXPath));
            }
        }

        // Navigating to dashboard page
        public DashboardPage NavigateToDashboardPage()
        {
            if (!IsMenuShown())
            {
                ToggleMenu();
            }

            _dashboardButton.Click();
            return new DashboardPage(_driver);
        }

        // Navigating to offers page
        public OffersPage NavigateToOffersPage()
        {
            if (!IsMenuShown())
            {
                ToggleMenu();
            }

            _offersButton.Click();
            return new OffersPage(_driver);
        }

        // Verifying if menu is shown
        private bool IsMenuShown() => _menuInput.GetAttribute("aria-checked") == "true";
    }
}
