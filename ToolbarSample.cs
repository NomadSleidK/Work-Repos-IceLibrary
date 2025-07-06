using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Toolbar;
using System.ComponentModel.Composition;

namespace MyIceLibrary
{
    [Export(typeof(IToolbar<ObjectsViewContext>))]
    public class ToolbarSample : IToolbar<ObjectsViewContext>
    {
        public void Build(IToolbarBuilder builder, ObjectsViewContext context)
        {
            IToolbarButtonItemBuilder button = builder.AddButtonItem("Моя кнопка", builder.Count);

            builder.AddSeparator(1);
            builder.AddSeparator(0);
            
            byte[] svgIcon = Properties.Resources.money;

            button.WithIcon(svgIcon);
            button.WithHeader("Money Header Button");
        }

        public void OnToolbarItemClick(string name, ObjectsViewContext context)
        {

        }
    }
}