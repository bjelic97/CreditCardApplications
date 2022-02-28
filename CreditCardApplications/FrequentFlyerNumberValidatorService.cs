namespace CreditCardApplications
{
    internal class FrequentFlyerNumberValidatorService : IFrequentFlyerNumberValidator
    {
        public event EventHandler ValidatorLookupPerformed;

        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("Simulate this real dependency being hard to use");
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Simulate this real dependency being hard to use");
        }

        //public string LicenseKey => throw new NotImplementedException();
        public IServiceInformation ServiceInformation => throw new NotImplementedException();

        public ValidationMode ValidationMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
