using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace Calypso.Tests
{
    public class RegisterCultureTests
    {
        [Test, ExpectedException(typeof(InvalidEnumArgumentException))]
        public void ThrowExceptionOnInvalidInput()
        {
            RegisterCulture.ParseCultureAndRegion("notanenum");
        }

        [Test]
        public void IsInvalidRegionModifierEnum()
        {
            var cultureEnum = RegisterCulture.ParseCultureAndRegion("Neutral");

            //default in enum is "none"
            Assert.AreEqual(CultureAndRegionModifiers.Neutral, cultureEnum);
        }

        [Test]
        public void TestEnUsExampleExists()
        {
            Assert.IsTrue(RegisterCulture.CustomCultureExists("x-en-US-example"));
        }

        [Test]
        public void CanRegisterCulture()
        {
            RegisterCulture.RegisterAndBuild("x-en-US-example", "None", "en-US", "US");

            Assert.IsTrue(RegisterCulture.CustomCultureExists("x-en-US-example"));
        }
    }
}
