using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        private Mock<IFrequentFlyerNumberValidator> _mockValidator;
        private CreditCardApplicationEvaluator _sut;
        public CreditCardApplicationEvaluatorShould()
        {
            _mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            _mockValidator.SetupAllProperties();
            _mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            _mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            _sut = new CreditCardApplicationEvaluator(_mockValidator.Object);
        }

        [Fact]
        public void AcceptHightIncomeApplications()
        {
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = _sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            _mockValidator.DefaultValue = DefaultValue.Mock;

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = _sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            //mockValidator.Setup(x => x.IsValid("x")).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsInRange("a", "z", Range.Inclusive))).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsIn("z", "y", "x"))).Returns(true);
            _mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);

            var application = new CreditCardApplication { GrossAnnualIncome = 19_999, Age = 42, FrequentFlyerNumber = "y" };

            CreditCardApplicationDecision decision = _sut.Evaluate(application);


            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            _mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = _sut.Evaluate(application);


            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new();

        //    bool isValid = true;
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication { GrossAnnualIncome = 19_999, Age = 42 };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);


        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");
            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);


            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpiryString()
        {
            return "EXPIRED";
        }

        [Fact]
        public void UsedDetailedLookupForOlderApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            // mockValidator.SetupProperty(x => x.ValidationMode); enables change tracking for property
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);


            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "q" };

            sut.Evaluate(application);


            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);


            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);


            mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            // mockValidator.SetupProperty(x => x.ValidationMode); enables change tracking for property
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);


            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);
            // mockValidator.VerifyNoOtherCalls();
        }


        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()));//.Throws(new Exception("Custom message"));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookupCount()
        {
            _mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true)
                .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            _sut.Evaluate(application);

            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            Assert.Equal(1, _sut.ValidatorLookupCount);
        }
        [Fact]
        public void ReferInvalidFrequentFlyerApplications_ReturnValueSequence()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                .Returns(false)
                .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);

            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_MultipleCallsSequence()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var frequentFlyerNumbersPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumbersPassed)));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            // Assert that IsValid was called 3 times with "aa", "bb" and "cc"
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumbersPassed);
        }

        [Fact]
        public void ReferFraudRisk()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            Mock<FraudLookup> mockFraudLookup = new();
            //mockFraudLookup.Setup(x => x.IsFraudRisk(It.IsAny<CreditCardApplication>())).Returns(true);
            mockFraudLookup.Protected()
                .Setup<bool>("CheckApplication", ItExpr.IsAny<CreditCardApplication>()).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object, mockFraudLookup.Object);

            var application = new CreditCardApplication();

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHumanFraudRisk, decision);
        }

        [Fact]
        public void LinqToMocks()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            IFrequentFlyerNumberValidator mockValidator = Mock.Of<IFrequentFlyerNumberValidator>
                (
                    validator => 
                    validator.ServiceInformation.License.LicenseKey == "OK" &&
                    validator.IsValid(It.IsAny<string>()) == true
                );
            var sut = new CreditCardApplicationEvaluator(mockValidator);

            var application = new CreditCardApplication { Age = 25 };

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}