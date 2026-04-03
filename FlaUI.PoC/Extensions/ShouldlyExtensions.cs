using NUnit.Framework.Constraints;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaUI.PoC.Extensions
{
    public static class ShouldlyExtensions
    {
        private const decimal Ten = 10;
        public static void ShouldBeCloseTo(
            this decimal? actual,
            decimal expected,
            int numberOfDecimals = 5,
            string? customMessage = null)
        {
            actual.ShouldNotBeNull();
            var difference = actual.Value - expected;
            var resolution = Ten.ToTheNthPower(-numberOfDecimals);
            var tolerance = resolution / 2;

            if (difference < 0) difference *= -1;

            if (difference > tolerance)
            {
                var min = expected - tolerance;
                var max = expected + tolerance;
                var message =
                    $"Expected {actual} to be within ±{tolerance} of {expected} " +
                    $"(range: [{min} - {max}]) " + 
                    $"-> Difference is {difference}";

                if (!string.IsNullOrWhiteSpace(customMessage))
                {
                    message = $"{customMessage}\n{message}";
                }

                throw new ShouldAssertException(message);
            }
        }
        private static decimal ToTheNthPower(this decimal basis, int power)
        {
            if (power == 0) return 1;

            var result = 1m;
            var isNegative = power < 0;
            var absPower = Math.Abs(power);

            for (int i = 0; i < absPower; i++)
            {
                result *= basis;
            }

            return isNegative ? 1 / result : result;
        }
    }
}
