using System;
using DonaRogApp.ValueObjects;
using Shouldly;
using Xunit;

namespace DonaRogApp.ValueObjects
{
    public class TaxCode_Tests
    {
        [Fact]
        public void Should_Create_Valid_Individual_TaxCode()
        {
            var validCode = "RSSMRA80A01H501U";

            var taxCode = new TaxCode(validCode);

            taxCode.Value.ShouldBe("RSSMRA80A01H501U");
            taxCode.Type.ShouldBe(TaxCodeType.Individual);
            taxCode.IsIndividual().ShouldBeTrue();
        }


        [Fact]
        public void Should_Normalize_To_Uppercase()
        {
            var lowercaseCode = "rssmra80a01h501u";

            var taxCode = new TaxCode(lowercaseCode);

            taxCode.Value.ShouldBe("RSSMRA80A01H501U");
        }

        [Fact]
        public void Should_Throw_On_Invalid_Length()
        {
            var invalidCode = "INVALID";

            Should.Throw<ArgumentException>(() => new TaxCode(invalidCode));
        }

        [Fact]
        public void Should_Throw_On_Empty_String()
        {
            Should.Throw<ArgumentException>(() => new TaxCode(""));
            Should.Throw<ArgumentException>(() => new TaxCode(null!));
        }

        [Fact]
        public void Should_Validate_Individual_Format()
        {
            var invalidFormat = "1234567890123456";

            Should.Throw<ArgumentException>(() => new TaxCode(invalidFormat));
        }

        [Fact]
        public void TaxCodes_With_Same_Value_Should_Be_Equal()
        {
            var taxCode1 = new TaxCode("RSSMRA80A01H501U");
            var taxCode2 = new TaxCode("RSSMRA80A01H501U");

            taxCode1.ShouldBe(taxCode2);
            (taxCode1 == taxCode2).ShouldBeTrue();
        }

    }
}
