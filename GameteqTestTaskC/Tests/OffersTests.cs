using GameteqTestTaskC.Helpers;
using GameteqTestTaskC.Models;
using GameteqTestTaskC.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace GameteqTestTaskC.Tests
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    internal class OffersTests<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        IWebDriver _driver;
        const string _url = "https://test-task.gameteq.com/";

        [SetUp]
        public void SetUp()
        {
            _driver = new TWebDriver
            {
                Url = _url
            };

            DashboardPage dashboardPage = new(_driver);

            dashboardPage.NavigateToOffersPage();
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        private Offer SampleOffer()
        {
            return new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "test_category1",
                             network: new List<string>()
                                 { "test_network1" },
                             group: "test_group1",
                             segments: new Group(connective: Connective.And,
                                                 segmentsValues: new List<string>()
                                                     { "test_segment1" },
                                                 groups: new List<Group>()));
        }

        private static IEnumerable<TestCaseData> OffersDataPositive()
        {
            yield return new TestCaseData(new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "test_category_a2",
                                                    network: new List<string>()
                                                    {
                                                        "test_network_a3",
                                                        "test_network_a4"
                                                    },
                                                    group: "test_group_a2",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                        {
                                                                            "test_segment_a5",
                                                                            "test_segment_a6"
                                                                        },
                                                                        groups: new List<Group>()
                                                                        { 
                                                                            new Group(Connective.Or,
                                                                                      new List<string>()
                                                                                      {
                                                                                          "test_segment_a5",
                                                                                          "test_segment_a6"
                                                                                      },
                                                                                      new List<Group>())
                                                                         })));
            
        }

        private static IEnumerable<TestCaseData> OffersDataNegative()
        {
            yield return new TestCaseData(new Offer(name: "", key: "test_key1", id: 0, forTest: true, category: "test_category1",
                                                    network: new List<string>() 
                                                        { "test_network1" },
                                                    group: "test_group1",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                            { "test_segment1" },
                                                                        groups: new List<Group>())));
            yield return new TestCaseData(new Offer(name: "test_name1", key: "", id: 0, forTest: true, category: "test_category1",
                                                    network: new List<string>()
                                                        { "test_network1" },
                                                    group: "test_group1",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                            { "test_segment1" },
                                                                        groups: new List<Group>())));
            yield return new TestCaseData(new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "",
                                                    network: new List<string>()
                                                        { "test_network1" },
                                                    group: "test_group1",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                            { "test_segment1" },
                                                                        groups: new List<Group>())));
            yield return new TestCaseData(new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "test_category1",
                                                    network: new List<string>(),
                                                    group: "test_group1",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                            { "test_segment1" },
                                                                        groups: new List<Group>())));
            yield return new TestCaseData(new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "test_category1",
                                                    network: new List<string>()
                                                        { "test_network1" },
                                                    group: "",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>()
                                                                            { "test_segment1" },
                                                                        groups: new List<Group>())));
            yield return new TestCaseData(new Offer(name: "test_name1", key: "test_key1", id: 0, forTest: true, category: "test_category1",
                                                    network: new List<string>()
                                                        { "test_network1" },
                                                    group: "test_group1",
                                                    segments: new Group(connective: Connective.And,
                                                                        segmentsValues: new List<string>(),
                                                                        groups: new List<Group>())));
        }

        [Test, TestCaseSource(nameof(OffersDataPositive))]
        public void AddOfferTestPositive(Offer offer)
        {
            OffersPage offersPage = new(_driver);

            List<Offer> beforeOffers = offersPage.GetOffers();

            Offer offerToAdd = offer;

            offersPage.CreateOffer(offerToAdd);

            OffersPage offersPageAfter = new(_driver);

            List<Offer> afterOffers = offersPageAfter.GetOffers();

            offerToAdd.Id = afterOffers.Max(obj => obj.Id);

            beforeOffers.Add(offerToAdd);

            Assert.That(afterOffers, Is.EqualTo(beforeOffers));
        }

        [Test, TestCaseSource(nameof(OffersDataNegative))]
        public void AddOfferTestNegative(Offer offer)
        {
            OffersPage offersPage = new(_driver);

            offersPage.CreateOffer(offer);

            OfferPage offerPage = new(_driver);

            Assert.That(offerPage.IsAt());
            Assert.That(offerPage.IsSaveButtonDisabled());
        }

        [Test]
        public void DeleteFirstOffetTest()
        {
            OffersPage offersPage = new(_driver);

            List<Offer> beforeOffers = offersPage.GetOffers();

            if (beforeOffers.Count == 0)
            {
                offersPage.CreateOffer(SampleOffer());
                beforeOffers = offersPage.GetOffers();
            }

            Offer offerToDelete = beforeOffers.First();

            offersPage.DeleteOffer(offerToDelete);

            OffersPage offersPageAfter = new(_driver);

            List<Offer> afterOffers = offersPageAfter.GetOffers();

            beforeOffers.Remove(offerToDelete);

            Assert.That(afterOffers, Is.EqualTo(beforeOffers));
        }
    }
}
