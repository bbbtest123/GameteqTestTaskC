using GameteqTestTaskC.Helpers;
using OpenQA.Selenium;

namespace GameteqTestTaskC.Pages
{
    // Dashboard page class
    internal class DashboardPage : BasePage
    {
        private const string _dashboardXPath = "//app-dashboard";

        private Element? _dashboard;

        public DashboardPage(IWebDriver driver) : base(driver)
        {

        }

        public override bool IsAt() => _dashboard.IsExistsWithWait();

        protected override void InitElements()
        {
            _dashboard = new Element(driver: _driver, locator: By.XPath(_dashboardXPath));
        }
    }
}
