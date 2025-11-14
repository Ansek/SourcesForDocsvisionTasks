using DocsVision.Platform.Tools.LayoutEditor.Extensibility;
using DocsVision.Platform.Tools.LayoutEditor.ObjectModel.Descriptions;
using Task11.Controls;

namespace Task11;

class Task11DesignerExtension(IServiceProvider provider) : WebLayoutsDesignerExtension(provider)
{
    protected override Dictionary<string, PropertyDescription> GetPropertyDescriptions()
    {
        return new Dictionary<string, PropertyDescription> {
            { "CostLabel", SearchTicketsControl.GetCostLabelPropertyDescription() },
            { "DestinationUI", SearchTicketsControl.GetDestinationUIPropertyDescription() },
            { "DepartureAtUI", SearchTicketsControl.GetDepartureAtUIPropertyDescription() },
            { "ReturnAtUI", SearchTicketsControl.GetReturnAtUIPropertyDescription() }
        };
    }

    protected override List<ControlTypeDescription> GetControlTypeDescriptions()
    {
        return [
            SearchTicketsControl.GetTypeDescription(PropertyFactory)
        ];
    }
}
