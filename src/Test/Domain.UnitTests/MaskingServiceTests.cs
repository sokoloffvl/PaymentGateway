using AutoFixture;
using Domain.Interfaces;
using Domain.Models;
using Domain.Services;
using Domain.Translators;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.UnitTests;

public class MaskingServiceTests
{
    private IMaskingService sut;
    [SetUp]
    public void Setup()
    {
        sut = new MaskingService();
    }
    
    [TestCase("1234 5678 9876 5432", "1234 *** 5432")]
    [TestCase("abcd efgh ijkl mnop", "abcd *** mnop")]
    [TestCase("1234 5677 8910 111214", "1234 *** 1214")]
    [TestCase("12345678", "1234 *** 5678")]
    [TestCase("123456", "1234 *** 3456")]
    [TestCase("123", "123 *** 123")]
    public void CanMaskCardNumbers(string cardNumber, string expectedMaskedCardNumber)
    {
        //Arrange
        //Act
        var masked = sut.MaskCardNumber(cardNumber);
        //Assert
        masked.Should().Be(expectedMaskedCardNumber);
    }

}