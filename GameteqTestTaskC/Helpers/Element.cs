using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GameteqTestTaskC.Helpers
{
    // A helper class for WebElement manipulation
    internal class Element
    {
        private readonly IWebDriver _driver;
        private readonly By _locator;
        private readonly int _waitTimeout;

        private IWebElement? _element;

        public Element(IWebDriver driver, By locator, int waitTimeout = 10)
        {
            _driver = driver;
            _locator = locator;
            _waitTimeout = waitTimeout;

            try
            {
                _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout)).Until(condition => TryFind());
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException($"No element with locator {_locator} exists on the page");
            }
        }

        public Element(Element element, By locator, int waitTimeout = 10)
        {
            _driver = element._driver; 
            _locator = locator;
            _waitTimeout = waitTimeout;

            _element = element._element;
        }

        public string LocatorCriteria() => _locator.Criteria;

        public bool IsExistsWithWait()
        {
            try
            {
                _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                    .Until(condition => TryFind());
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;   
            }
        }

        public bool IsExistsWithoutWait()
        {
            try
            {
                _element = _driver.FindElement(_locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void Click()
        {
            InvokeAction(ClickAction);
        }

        public void SendKeys(string value)
        {
            InvokeAction(value, SendKeysAction);
        }

        public string Text()
        {
            return InvokeFunc(GetTextFunc);
        }

        public string GetAttribute(string attributeKey)
        {
            return InvokeFunc(attributeKey, GetAttributeFunc);
        }

        public List<Element> GetChildren(By locator)
        {
            return InvokeFunc(locator.Criteria, GetChildrenFunc);
        }

        public Element GetChild(By locator)
        {
            return InvokeFunc(locator.Criteria, GetChildFunc);
        }

        private IWebElement? TryFind()
        {
            try
            {
                if (_element == null)
                {
                    return _driver.FindElement(_locator);
                }
                else
                { 
                    return _element;
                } 
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        private bool TryClick()
        {
            try
            {
                _element.Click();
                return true;
            }
            catch (Exception e) when (e is ElementClickInterceptedException || e is ElementNotInteractableException)
            {
                return false;
            }
        }

        private void InvokeAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    action();
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private void InvokeAction(string actionInput, Action<string> action)
        {
            try
            {
                action(actionInput);
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    action(actionInput);
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private string InvokeFunc(Func<string> func)
        {
            try
            {
                return func();
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    return func();
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private string InvokeFunc(string funcInput, Func<string, string> func)
        {
            try
            {
                return func(funcInput);
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    return func(funcInput);
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private List<Element> InvokeFunc(string funcInput, Func<string, List<Element>> func)
        {
            try
            {
                return func(funcInput);
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    return func(funcInput);
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private Element InvokeFunc(string funcInput, Func<string, Element> func)
        {
            try
            {
                return func(funcInput);
            }
            catch (Exception e) when (e is NullReferenceException || e is StaleElementReferenceException)
            {
                try
                {
                    _element = new WebDriverWait(driver: _driver, timeout: TimeSpan.FromSeconds(_waitTimeout))
                        .Until(condition => TryFind());
                    return func(funcInput);
                }
                catch (NullReferenceException)
                {
                    throw new NoSuchElementException($"No element with locator {_locator} exists on the page");
                }
            }
        }

        private void ClickAction() => TryClick();

        private void SendKeysAction(string value)
        {
            if (value != null && value.Length > 0)
            {
                _element.SendKeys(value);
            }
        }

        private string GetAttributeFunc(string attributeKey) => _element.GetAttribute(attributeKey);
        private string GetTextFunc() 
        {
            string text = _element.Text;

            return _element.Text;
        }
        private List<Element> GetChildrenFunc(string xpath) 
        {
            List<Element> result = new();

            //Sleaping to avoid weird cases of wrong number of children
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            var elements = _element.FindElements(By.XPath(xpath));

            result.AddRange(from IWebElement element in elements
                            select new Element(driver: _driver,
                                               locator: By.XPath($"({_locator.Criteria + xpath[1..]})[{elements.IndexOf(element) + 1}]"),
                                               waitTimeout: _waitTimeout));
            return result;
        }
        private Element GetChildFunc(string xpath) => 
            new(driver: _driver, locator: By.XPath(_locator.Criteria + xpath[1..]), waitTimeout: _waitTimeout);
    }
}
