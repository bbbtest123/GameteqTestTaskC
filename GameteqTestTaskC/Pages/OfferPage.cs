using GameteqTestTaskC.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using GameteqTestTaskC.Models;
using OpenQA.Selenium.Support.UI;

namespace GameteqTestTaskC.Pages
{
    // Class for offer creation/edit dialog
    internal class OfferPage : BasePage
    {
        private const string _overlayContainerXPath = "//div[@class='cdk-overlay-container']";

        private const string _forTestsCheckboxXPath = "//mat-checkbox";
        private const string _forTestsCheckboxInputXPath = "//mat-checkbox//input[@name='forTest']";


        private const string _nameInputXPath = "//input[@name='name']";
        private const string _keyInputXPath = "//input[@name='key']";

        private const string _categorySelectXPath = "//select[@name='category']";
        private const string _categoryAddButtonXPath =
            "//div[@class='mat-form-field-flex' and div[select[@name='category']]]//button";
        private const string _categoryOptionsXpath = "./option";
        private const string _networksSelectXPath = "//mat-select[@name='networks']";
        private const string _networkAddButtonXPath =
            "//div[@class='mat-form-field-flex' and div[mat-select[@name='networks']]]//button";
        private const string _groupSelectXPath = "//mat-select[@name='group']";
        private const string _groupAddButtonXPath =
            "//div[@class='mat-form-field-flex' and div[mat-select[@name='group']]]//button";
        private const string _matOptionXPath = ".//mat-option";
        private const string _matOptionTextXPath = "./span";

        private const string _newEntityFormXPath = "//div[@class='cdk-overlay-pane']";
        private const string _newEntityInputXPath = _newEntityFormXPath + "//input";
        private const string _newEntityCreateButtonXPath = _newEntityFormXPath + "//button[span[contains(text(), 'Create')]]";
        private const string _newEntityCancelButtonXPath = _newEntityFormXPath + "//button[span[contains(text(), 'Cancel')]]";


        private const string _addSegmentButtonXpath = "//mat-card-title[contains(text(), 'Segments')]/button";
        private const string _segmentFormXPath = "/app-form-segments/mat-card";
        private const string _matCardTitleXPath = "/mat-card-title";
        private const string _orButtonXpath = "//mat-radio-button[@value='or']";
        private const string _andButtonXpath = "//mat-radio-button[@value='and']";
        private const string _addGroupButtonXpath =
            "/mat-card-title" +
            "//button[span[contains(text(), 'Add group')]]";
        private const string _addSegmentForGroupButtonXpath =
            "/mat-card-title" +
            "//button[span[contains(text(), 'Add segment')]]";
        private const string _groupContentXpath = "/mat-card-content";
        private const string _segmentValueSelectXPath = "//mat-select[@name='val']";

        private const string _saveButtonXpath =
            "//mat-card-title" +
            "/div[mat-checkbox]" +
            "/button[@class='mat-raised-button mat-button-base mat-accent']";

        private Element _forTestsCheckbox;
        private Element _forTestsCheckboxInput;

        private Element _nameInput;
        private Element _keyInput;

        private Element _categorySelect;
        private Element _categoryAddButton;
        private Element _networksSelect;
        private Element _networkAddButton;
        private Element _groupSelect;
        private Element _groupAddButton;

        private Actions _builder;

        public OfferPage(IWebDriver driver) : base(driver)
        {

        }

        public override bool IsAt() => _nameInput.IsExistsWithWait();

        public void CreateOffer(Offer offer)
        {
            SetForTestCheckbox(offer?.ForTest ?? false);
            EnterName(offer?.Name ?? String.Empty);
            EnterKey(offer?.Key ?? String.Empty);
            SelectCategory(offer?.Category ?? String.Empty);
            SelectNetworks(offer?.Networks ?? new List<string>());
            SelectGroup(offer?.Group ?? String.Empty);
            AddSegments(offer?.Segments ?? new Group());
            PressSave();
        }

        public OfferPage ClickForTestCheckbox()
        {
            _forTestsCheckbox.Click();
            return this;
        }

        public bool IsForTestChecked() => _forTestsCheckboxInput.GetAttribute("aria-checked") == "true";

        public OfferPage SetForTestCheckbox(bool isForTest)
        {
            if (isForTest)
            {
                CheckForTestCheckbox();
            }
            else
            { 
                UncheckForTestCheckbox();
            }
            return this;
        }

        public OfferPage CheckForTestCheckbox()
        {
            if (!IsForTestChecked()) 
            {
                ClickForTestCheckbox();
            }
            return this;
        }

        public OfferPage UncheckForTestCheckbox()
        {
            if (IsForTestChecked())
            {
                ClickForTestCheckbox();
            }
            return this;
        }

        public OfferPage EnterName(string name)
        { 
            _nameInput.SendKeys(name);
            return this;
        }

        public OfferPage EnterKey(string key)
        {
            _keyInput.SendKeys(key);
            return this;
        }

        // A method for category selection
        public OfferPage SelectCategory(string category)
        {
            _categorySelect.Click();

            // A flag for checking if the category actually added
            bool isCategorySelected = false;

            // A list of Element for category options
            var categoryOptions = _categorySelect.GetChildren(By.XPath(_categoryOptionsXpath));

            // A loop for Iterating over category options and selecting one
            foreach (Element categoryOption in categoryOptions)
            {
                if (categoryOption.Text() == category)
                {
                    categoryOption.Click();
                    isCategorySelected = true;
                    break;
                }
            }

            // If no such category option exist, creating one and selecting it again
            if (!isCategorySelected)
            {
                AddCategory(category);
                SelectCategory(category);
            }

            return this;
        }

        // A method for creating new category
        public OfferPage AddCategory(string category)
        {
            // Category options count to manage waits
            int countBefore = _categorySelect.GetChildren(By.XPath(_categoryOptionsXpath)).Count();

            _categoryAddButton.Click();

            CreateNewEntity(category);

            // Waiting until category options count increases
            new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
                .Until(condition =>
                       _categorySelect.GetChildren(By.XPath(_categoryOptionsXpath)).Count() == countBefore + 1);

            return this;
        }

        // A method for networks selection
        public OfferPage SelectNetworks(List<string> networks)
        { 
            _networksSelect.Click();

            Element overlayContainer = new Element(_driver, By.XPath(_overlayContainerXPath));

            List<Element>? networkOptions = overlayContainer.GetChildren(By.XPath(_matOptionXPath));

            // An empty list for not yet existing networks
            List<string> missingNetworks = new();

            // An empty list for networks to select
            List<Element> networksToSelect = new();

            // Iterating over networks to select
            foreach (string network in networks)
            {
                if (network.Length > 0)
                {
                    // Capitalizing network
                    string capitalized = Capitalize(network);

                    // Selecting networks from options with the same name as a network to select
                    var networkElementsWithText =
                        from Element networkOption in networkOptions
                        where new Element(_driver, 
                                          By.XPath(networkOption.LocatorCriteria() + _matOptionTextXPath[1..]))
                            .Text() == capitalized
                        && !networksToSelect.Contains(networkOption)
                        select networkOption;

                    // If no network options with such text exist, adding it to a list to add later.
                    // Otherwise adding the first one to a list to select later
                    if (networkElementsWithText.Count() == 0)
                    {
                        missingNetworks.Add(network);
                    }
                    else
                    {
                        networksToSelect.Add(networkElementsWithText.First());
                    }
                }
            }

            // If there are missing networks
            if (missingNetworks.Count() > 0)
            {
                // A count of networks
                int countBefore = networkOptions.Count();

                // Sending Escape key to hide network options
                _builder.SendKeys(Keys.Escape).Perform();

                // Adding missing networks
                AddNetworks(missingNetworks);

                Element networksSelect = new Element(_driver, By.XPath(_networksSelectXPath));

                // Clicking on the network selection element
                networksSelect.Click();

                Element overlayContainerAfter = new Element(_driver, By.XPath(_overlayContainerXPath));
                
                // A count of networks after addition
                int countAfter = overlayContainerAfter.GetChildren(By.XPath(_matOptionXPath)).Count();

                // Wait until networks are actually added
                new WebDriverWait(_driver, TimeSpan.FromSeconds(10))
                    .Until(condition =>
                           overlayContainerAfter.GetChildren(By.XPath(_matOptionXPath)).Count() ==
                           countBefore + missingNetworks.Count());

                // Calling this method again
                SelectNetworks(networks);
            }
            else
            {
                // If there is no need in adding any networks, select networks from our list to add
                foreach (Element network in networksToSelect)
                {
                    network.Click();
                }

                // Hiding network options
                _builder.SendKeys(Keys.Escape).Perform();
            }

            return this;
        }

        public OfferPage AddNetworks(List<string> networks)
        {
            foreach (string network in networks)
            {
                _networkAddButton.Click();

                CreateNewEntity(network);
            }

            return this;
        }

        // A method for group selection
        public OfferPage SelectGroup(string groupName)
        {
            if (groupName.Length > 0)
            {
                _groupSelect.Click();

                Element overlayContainer = new Element(_driver, By.XPath(_overlayContainerXPath));

                // Geting a list of group options
                // Thread.Sleep(TimeSpan.FromMilliseconds(100));
                List<Element> groupOptions = overlayContainer.GetChildren(By.XPath(_matOptionXPath));

                // A flag for selection
                bool isGroupSelected = false;

                // Iterating over options and selecting one with desired text
                foreach (Element groupOption in groupOptions)
                {
                    Element groupTextSpan = new(_driver, By.XPath(groupOption.LocatorCriteria() + _matOptionTextXPath[1..]));
                    if (groupTextSpan.Text() == groupName)
                    {
                        groupOption.Click();
                        isGroupSelected = true;
                        break;
                    }
                }

                // If no gropus selected, adding the group
                if (!isGroupSelected)
                {
                    _builder.SendKeys(Keys.Escape).Perform();

                    AddGroup(groupName);
                    SelectGroup(groupName);
                }
            }

            return this;
        }

        public OfferPage AddGroup(string groupName)
        {
            _groupAddButton.Click();

            CreateNewEntity(groupName);

            return this;
        }

        // A public method for adding groups/segments
        public OfferPage AddSegments(Group group)
        {
            AddSegments(group, new(_driver, By.XPath("/" + _segmentFormXPath)));

            return this;
        }

        // A private method for adding groups/segments
        private void AddSegments(Group group, Element thisElement)
        {
            // Selecting correct connective for the group
            if (group.Connective == Connective.Or)
            {
                Element orRadioButton = new(_driver, By.XPath(thisElement.LocatorCriteria() +
                                                              _matCardTitleXPath + _orButtonXpath));
                orRadioButton.Click();
            }
            else
            {
                Element andRadioButton = new(_driver, By.XPath(thisElement.LocatorCriteria() +
                                                               _matCardTitleXPath + _andButtonXpath));
                andRadioButton.Click();
            }

            // Pressing 'Add segment' button for each segment to add
            // It's necessary to add all the segments before selecting segment values
            // Otherwise selected values are discarded on pressing 'Add segment' button
            foreach (string segment in group.SegmentsValues)
            {
                Element addSegmentForGroupButton = new(_driver, By.XPath(thisElement.LocatorCriteria() + _addSegmentForGroupButtonXpath));

                addSegmentForGroupButton.Click();
            }

            // Iterating over segments and selecting values
            foreach (string segment in group.SegmentsValues)
            {
                SelectSegmentValue(segment, group.SegmentsValues.IndexOf(segment), thisElement);
            }

            // Iterating over children groups and adding them recursively
            foreach (Group childGroup in group.Groups)
            {
                Element addGroupButton = new(_driver, By.XPath(thisElement.LocatorCriteria() + 
                                                               _addGroupButtonXpath));

                addGroupButton.Click();

                AddSegments(childGroup, new Element(_driver, By.XPath("(" + thisElement.LocatorCriteria() +
                                                                      _groupContentXpath 
                                                                      + _segmentFormXPath + ")" +
                                                                      $"[{group.Groups.IndexOf(childGroup) + 1}]")));
            }
        }

        // Adding new segment value
        private OfferPage AddSegmentValue(string segment)
        {
            Element addSegmentButton = new(_driver, By.XPath(_addSegmentButtonXpath));
            addSegmentButton.Click();

            CreateNewEntity(segment);

            return this;
        }

        // Selecting segment values
        private void SelectSegmentValue(string segment, int index, Element parent)
        {
            if (segment.Length > 0)
            {
                Element segmentSelect =
                    new(_driver, By.XPath($"({parent.LocatorCriteria() + _groupContentXpath + _segmentValueSelectXPath})[{index + 1}]"));
                
                segmentSelect.Click();

                Element overlayContainer = new Element(_driver, By.XPath(_overlayContainerXPath));

                // Segment options list
                List<Element> segmentOptions = overlayContainer.GetChildren(By.XPath(_matOptionXPath));

                // Flaf for segment selection
                bool isSegmentSelected = false;

                // Capitalizing segment value
                string capitalized = Capitalize(segment);

                // Iterating over segment options and selecting one
                foreach (Element segmentOption in segmentOptions)
                {
                    Element segmentTextSpan = new(_driver, By.XPath(segmentOption.LocatorCriteria() + _matOptionTextXPath[1..]));

                    if (segmentTextSpan.Text() == capitalized)
                    {
                        segmentOption.Click();
                        isSegmentSelected = true;
                        break;
                    }
                }

                // If no segments selected - adding missing value
                if (!isSegmentSelected)
                {
                    _builder.SendKeys(Keys.Escape).Perform();

                    AddSegmentValue(segment);
                    SelectSegmentValue(segment, index, parent);
                }
            }
        }
    
        // Creating new entity through a dialog
        public OfferPage CreateNewEntity(string value)
        {
            Element newEntityInput = new(_driver, By.XPath(_newEntityInputXPath));
            newEntityInput.SendKeys(value);

            Element newEntityCreateButton = new(_driver, By.XPath(_newEntityCreateButtonXPath));
            newEntityCreateButton.Click();

            // It's necessary to wait until creation dialog is closed
            new WebDriverWait(_driver, TimeSpan.FromSeconds(10)).Until(condition => newEntityInput.IsExistsWithoutWait() == false);

            return this;
        }

        // Pressing Save button
        public void PressSave()
        {
            Element saveButton = new(_driver, By.XPath(_saveButtonXpath));

            try
            {
                // It's necessary to wait until Save button become enabled
                new WebDriverWait(_driver, TimeSpan.FromSeconds(2)).Until(condition => saveButton.GetAttribute("disabled") != "true");
            }
            catch (WebDriverTimeoutException)
            {
                
            }

            saveButton.Click();
        }

        // Checking if Save button is disabled
        public bool IsSaveButtonDisabled()
        {
            Element saveButton = new(_driver, By.XPath(_saveButtonXpath));

            bool result = saveButton.GetAttribute("disabled") != "true";

            return saveButton.GetAttribute("disabled") == "true";
        }

        // Initing initial elements
        protected override void InitElements()
        {
            _forTestsCheckbox = new Element(_driver, By.XPath(_forTestsCheckboxXPath));
            _forTestsCheckboxInput = new Element(_driver, By.XPath(_forTestsCheckboxInputXPath));

            _nameInput = new Element(_driver, By.XPath(_nameInputXPath));
            _keyInput = new Element(_driver, By.XPath(_keyInputXPath));

            _categorySelect = new Element(_driver, By.XPath(_categorySelectXPath));
            _categoryAddButton = new Element(_driver, By.XPath(_categoryAddButtonXPath));
            _networksSelect = new Element(_driver, By.XPath(_networksSelectXPath));
            _networkAddButton = new Element(_driver, By.XPath(_networkAddButtonXPath));
            _groupSelect = new Element(_driver, By.XPath(_groupSelectXPath));
            _groupAddButton = new Element(_driver, By.XPath(_groupAddButtonXPath));

            _builder = new(_driver);
        }

        // Capitalizing a string
        private string Capitalize(string input)
        {
            string capitalized = "";
            if (input.Length == 1)
            {
                return char.ToUpper(input[0]).ToString();
            }
            else if (input.Length > 1)
            {
                return char.ToUpper(input[0]).ToString() + input[1..].ToLower();
            }
            else
            { 
                return String.Empty;
            }
        }
    }
}
