using FlaUI.PoC.Extensions;
using FlaUI.PoC.Factories;
using FlaUI.PoC.Models;
using Shouldly;

namespace FlaUI.PoC
{
    public class Tests
    {
        private AppHandle? _app = null;
        private Calculator Calc => new(_app!.Application);

        [SetUp]
        public void Setup()
            => _app = AppFactory
                .GetApplication(@"C:\Windows\System32\calc.exe", "ApplicationFrameHost");
        
        [TearDown]
        public void TearDown()
            => _app?.Close();
        
        [TestCase(112312321, 23525325)]
        [TestCase(2, -5)]
        public void ValidateSumming(decimal firstNumber, decimal secondNumber)
            => Calc
                .Reset()
                .Enter(firstNumber)
                .Plus(secondNumber)
                .Result()
                .ShouldBe(firstNumber + secondNumber);
        
        [TestCase(11.23, 25.1956)]
        [TestCase(2, 5)]
        public void ValidateSubtracting(decimal firstNumber, decimal secondNumber)
            => Calc
                .Reset()
                .Enter(firstNumber)
                .Minus(secondNumber)
                .Result()
                .ShouldBe(firstNumber - secondNumber);

        [TestCase(11.23, 25.1956)]
        [TestCase(2, 5)]
        public void ValidateMultiplying(decimal firstNumber, decimal secondNumber)
            => Calc
                .Reset()
                .Enter(firstNumber)
                .MultiplyWith(secondNumber)
                .Result()
                .ShouldBeCloseTo(firstNumber * secondNumber, 16);

        [TestCase(-11.23, -25.1956)]
        [TestCase(2, 5)]
        public void ValidateDivision(decimal firstNumber, decimal secondNumber)
            => Calc
                .Reset()
                .Enter(firstNumber)
                .DivideBy(secondNumber)
                .Result()
                .ShouldBeCloseTo(firstNumber / secondNumber, 16);   

        [Test]
        public void ValidateDivisionByZero()
            => Should.Throw<DivideByZeroException>(() =>
                Calc
                    .Reset()
                    .Enter(123)
                    .DivideBy(0)
                    .Result());
        
        [Test]
        public void ClearingOfTheResult()
            => Calc
                .Reset()
                .Enter(1131)
                .Reset()
                .Result()
                .ShouldBe(0);

    }
}