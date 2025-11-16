using DocsVision.Platform.Tools.LayoutEditor.Extensibility;
using DocsVision.Platform.Tools.LayoutEditor.ObjectModel.Descriptions;
using Task11.Controls;

namespace Task11;

class Task11DesignerExtension(IServiceProvider provider) : WebLayoutsDesignerExtension(provider)
{
    protected override Dictionary<string, PropertyDescription> GetPropertyDescriptions()
    {
        return SearchTicketsControl.GetPropertyDescriptions();
    }

    protected override List<ControlTypeDescription> GetControlTypeDescriptions()
    {
        return [ SearchTicketsControl.GetTypeDescription(PropertyFactory) ];
    }
}
