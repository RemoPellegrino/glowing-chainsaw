using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using System.Globalization;

namespace FlaUI.PoC.Models
{
    internal class Calculator(Application application)
        : Model(application)
    {
        #region Selectors
        private ConditionBase ResultSelector => ConditionFactory.ByAutomationId("CalculatorResults").And(ConditionFactory.ByControlType(ControlType.Text));
        private AndCondition GetActionButton(CalculationAction action)
            => action switch
            {
                CalculationAction.Plus => GetNamedButtonCondition("plus"),
                CalculationAction.Equals => GetNamedButtonCondition("equal"),
                CalculationAction.Minus => GetNamedButtonCondition("minus"),
                CalculationAction.Divide => GetNamedButtonCondition("divide"),
                CalculationAction.Multiply => GetNamedButtonCondition("multiply"),
                _ => throw new NotImplementedException(action.ToString())
            };
        private AndCondition GetNumberCondition(char number) => GetNamedButtonCondition($"num{number}");
        private AndCondition Negate => GetNamedButtonCondition("negate");
        private AndCondition DecimalSeparator => GetNamedButtonCondition("decimalSeparator");
        private AndCondition ClearButton => GetNamedButtonCondition("clear");
        #endregion

        #region Exposed builder methods
        public Calculator Reset()
        {
            ClickButton(ClearButton);
            return this;
        }
        public Calculator Enter(decimal number)
        {
            FillInNumber(number);
            return this;
        }
        public Calculator Plus(decimal number)
            => ChainCalculation(number, CalculationAction.Plus);
        public Calculator Minus(decimal number)
            => ChainCalculation(number, CalculationAction.Minus);
        public Calculator MultiplyWith(decimal number)
            => ChainCalculation(number, CalculationAction.Multiply);
        public Calculator DivideBy(decimal number)
            => ChainCalculation(number, CalculationAction.Divide);
        public decimal? Result()
        {
            ClickButton(GetActionButton(CalculationAction.Equals));
            if (!TryReadResultAsdecimal(out var result))
                return null;
            return result;
        }
        #endregion

        #region Private abstractions
        private AndCondition GetNamedButtonCondition(string name) => ConditionFactory.ByAutomationId($"{name}Button").And(ConditionFactory.ByControlType(ControlType.Button));
        private string ReadResult()
        {
            var resultWindow = MainWindow!
                            .FindFirstDescendant(ResultSelector)
                            .AsLabel() ?? throw new Exception("Result Window not found");

            var text = resultWindow.Text.Replace("Display is ", "");

            return text switch
            {
                "Cannot divide by zero" => throw new DivideByZeroException(),
                _ => text
            };

        }
        private bool TryReadResultAsdecimal(out decimal value)
        {
            var result = ReadResult();

            // Normalize weird characters
            result = result
                .Replace("−", "-")     // unicode minus
                .Replace("\u202F", "") // narrow no-break space
                .Replace("\u00A0", ""); // non-breaking space

            return decimal.TryParse(
                result,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out value
            );
        }
        private void FillInNumber(decimal number)
        {
            var numberString = Math.Abs(number).ToString();
            for(var i = 0; i < numberString.Length; i++)
            {
                var c = numberString[i];
                if (char.IsDigit(c))
                {
                    ClickButton(GetNumberCondition(c));
                }
                else if (c.ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                {
                    ClickButton(DecimalSeparator);
                }
            }
            if (number < 0)
            {
                ClickButton(Negate);
            }

        }
        private Calculator ChainCalculation(decimal number, CalculationAction calculationAction)
        {
            ClickButton(GetActionButton(calculationAction));
            FillInNumber(number);
            return this;
        }
        #endregion

    }
    public enum CalculationAction
    {
        Plus,
        Minus,
        Multiply,
        Divide,
        Equals
    }
}
