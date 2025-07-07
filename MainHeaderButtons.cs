using System;
using System.ComponentModel.Composition;
using Ascon.Pilot.SDK;

namespace MyIceLibrary
{
    [Export(typeof(INewTabPage))]
    public class MainHeaderButtons : INewTabPage
    {
        public void BuildNewTabPage(INewTabPageHost host)
        {
            byte[] data = Properties.Resources.money;
            var sampleGuid = Guid.NewGuid();
            //host.SetGroup("My Group", sampleGuid);
            host.AddButton("Money Header Button", "MoneyHeaderButton", "Новая кнопка", data);
        }

        public void OnButtonClick(string name)
        {
            
        }
    }
}