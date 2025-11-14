using DocsVision.Platform.Tools.LayoutEditor.Helpers;
using DocsVision.Platform.Tools.LayoutEditor.Infrostructure;
using DocsVision.Platform.Tools.LayoutEditor.ObjectModel.Descriptions;

namespace Task11.Controls;

internal class SearchTicketsControl
{
    public static ControlTypeDescription GetTypeDescription(IPropertyFactory property) =>
        new("SearchTicketsControl")
        {
            DisplayName = "Поиск билетов",
            ControlGroupDisplayName = "Мои элементы управления",
            PropertyDescriptions = {
                property.GetNameProperty(),
                property.GetVisibilityProperty(),
                property.GetClickEvent(),
                property.Create("CostLabel"),
                property.Create("DestinationUI"),
                property.Create("DepartureAtUI"),
                property.Create("ReturnAtUI")
            }
        };

    public static PropertyDescription GetCostLabelPropertyDescription() =>
        new()
        {
            Name = "CostLabel",
            DisplayName = "Метка стоимости билета",
            Type = typeof(string),
            Category = PropertyCategoryConstants.DataCategory
        };
    public static PropertyDescription GetDestinationUIPropertyDescription() =>
        new()
        {
            Name = "DestinationUI",
            DisplayName = "ЭУ Пункт назначения",
            Type = typeof(string),
            Category = PropertyCategoryConstants.DataCategory
        };

    public static PropertyDescription GetDepartureAtUIPropertyDescription() =>
        new()
        {
            Name = "DepartureAtUI",
            DisplayName = "ЭУ Дата вылета",
            Type = typeof(string),
            Category = PropertyCategoryConstants.DataCategory
        };

    public static PropertyDescription GetReturnAtUIPropertyDescription() =>
        new()
        {
            Name = "ReturnAtUI",
            DisplayName = "ЭУ Дата возращения",
            Type = typeof(string),
            Category = PropertyCategoryConstants.DataCategory
        };
}
