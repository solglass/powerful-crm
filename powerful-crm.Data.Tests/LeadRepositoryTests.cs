using Microsoft.Extensions.Options;
using NUnit.Framework;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;

namespace powerful_crm.Data.Tests
{
    public class LeadRepositoryTests
    {
        private LeadRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new LeadRepository(Options.Create(new AppSettings
            {
                CONNECTION_STRING = "Data Source=80.78.240.16;Initial Catalog=CRM;Persist Security Info=True;User ID=student;Password=qwe!23"
            }));
        }

        [Test]
        public void Test1()
        {
            var result = _repository.SearchLeads(new SearchLeadDto { FirstName = "John", LastName = "Medina" });
            Assert.Pass();
        }
    }
}