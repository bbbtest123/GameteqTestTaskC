using GameteqTestTaskC.Helpers;
using GameteqTestTaskC.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GameteqTestTaskC.Pages
{
    // A class for Offers page
    internal class OffersPage : BasePage
    {
        private const string _offersTableXPath = "//tbody";
        private const string _offersTableRowXPath = "./tr";
        private const string _offersTableCellXPath = "./td";
        private const string _addButtonXpath = "//button[@class='mat-raised-button mat-button-base mat-primary ng-star-inserted']";
        private const string _deleteButtonXPath = "/button[@class='mat-raised-button mat-button-base mat-warn']";
        private const string _confirmDeleteButtonXPath = "//simple-snack-bar//button";

        private Element _offersTable;
        private Element _addButton;

        public OffersPage(IWebDriver driver) : base(driver)
        {

        }

        public override bool IsAt() => _offersTable.IsExistsWithWait();

        // Getting offers as List
        public List<Offer> GetOffers() 
        {
            List<Offer> result = new();

            List<Element> offers = _offersTable.GetChildren(By.XPath(_offersTableRowXPath));

            foreach (Element offer in offers)
            {
                result.Add(new Offer(name: offer.GetChildren(By.XPath(_offersTableCellXPath))[1].Text(),
                                     key: offer.GetChildren(By.XPath(_offersTableCellXPath))[2].Text(),
                                     id: int.Parse(offer.GetChildren(By.XPath(_offersTableCellXPath))[0].Text())));
            }

            return result;
        }

        // Creating new offer
        public void CreateOffer(Offer offer)
        { 
            ClickAdd().CreateOffer(offer);
        }

        public OfferPage ClickAdd()
        { 
            _addButton.Click();
            return new OfferPage(_driver);
        }

        // Deleting an offer by index in table
        public OffersPage DeleteOffer(int index)
        {
            int countBefore = _offersTable.GetChildren(By.XPath(_offersTableRowXPath)).Count;
            Element deleteButton = new(driver: _driver,
                                       locator: By.XPath($"({_offersTableXPath + _offersTableRowXPath[1..]})[{index} + 1]" +
                                                $"{_offersTableCellXPath[1..]}[4]{_deleteButtonXPath}"));
            deleteButton.Click();

            Element confirmDeleteButton = new(driver: _driver, locator: By.XPath(_confirmDeleteButtonXPath));

            new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(10))
                .Until(condition => _offersTable.GetChildren(By.XPath(_offersTableRowXPath)).Count == countBefore - 1);

            return this;
        }

        // Deleting an offer with id the same as an input offer id
        public OffersPage DeleteOffer(Offer offer)
        {
            int countBefore = _offersTable.GetChildren(By.XPath(_offersTableRowXPath)).Count;
            Element deleteButton = new(driver: _driver,
                                       locator: By.XPath($"({_offersTableXPath + _offersTableRowXPath[1..]})" +
                                                         $"[td[contains(text(), '{offer.Id}')][1]]" +
                                                         $"{_offersTableCellXPath[1..]}[4]{_deleteButtonXPath}"));
            deleteButton.Click();

            Element confirmDeleteButton = new(driver: _driver, locator: By.XPath(_confirmDeleteButtonXPath));

            confirmDeleteButton.Click();

            new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(10))
                .Until(condition => _offersTable.GetChildren(By.XPath(_offersTableRowXPath)).Count == countBefore - 1);

            return this;
        }

        protected override void InitElements()
        {
            _offersTable = new Element(driver: _driver, locator: By.XPath(_offersTableXPath));
            _addButton = new Element(driver: _driver, locator: By.XPath(_addButtonXpath));
        }
    }
}
