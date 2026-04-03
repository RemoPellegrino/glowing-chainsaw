using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System.Xml.Serialization;

namespace FlaUI.PoC.Models
{
    public abstract class Model(Application application)
    {
        protected const int _ticks = 100;
        protected ConditionFactory ConditionFactory = new(new UIA3PropertyLibrary());
        protected Application Application = application;
        protected Window? MainWindow => Application.GetMainWindow(new UIA3Automation());

        protected void ClickButton(ConditionBase? condition)
        {
            var button = MainWindow!.FindFirst(TreeScope.Descendants, condition).AsButton();
            button!.Click();            
        }
        protected static void Pause(int ticks = _ticks)
        {
            Thread.Sleep(ticks);    
        }
    }
}
