using DocsVision.Platform.Tools.LayoutEditor.Controls;
using DocsVision.Platform.Tools.LayoutEditor.Helpers;
using DocsVision.Platform.Tools.LayoutEditor.Infrostructure;
using DocsVision.Platform.Tools.LayoutEditor.ObjectModel.Descriptions;

// Пример Binding:
// DocsVision.Platform.WebLayoutsDesigner.PlatformPropertyFactory
// - GetDataFieldPropertyInstance()
// - GetBindingPropertyInstance()

namespace Task11.Controls;

internal class SearchTicketsControl
{
    private enum PropertyType { Label, Field, Binding, UIName }
    private static readonly List<PropertyDescription> _properties;

    static SearchTicketsControl()
    {
        _properties = [
            (GetPropertyDescription(PropertyType.UIName, "Destination", 3, "ЭУ пункта назначения")),
            (GetPropertyDescription(PropertyType.UIName, "DepartureAt", 2, "ЭУ даты отправления")),
            (GetPropertyDescription(PropertyType.UIName, "ReturnAt", 1, "ЭУ даты возвращения")),
        ];
        _properties.AddRange(GetPropertyDescriptionGroup(
            "Price", 6, "Стоимость", "Метка стоимости", typeof(IntMetadataEditor)));
        _properties.AddRange(GetPropertyDescriptionGroup(
            "DepartureFlight", 5, "Рейс отправления", "Метка рейса отправления", typeof(StringMetadataEditor)));
        _properties.AddRange(GetPropertyDescriptionGroup(
            "ReturnFlight", 4, "Рейс возвращения", "Метка рейса возвращения", typeof(StringMetadataEditor)));
    }

    public static ControlTypeDescription GetTypeDescription(IPropertyFactory factory)
    {
        var descriptions = new ControlTypeDescription("SearchTicketsControl")
        {
            DisplayName = "Поиск билетов",
            ControlGroupDisplayName = "Мои элементы управления",
            PropertyDescriptions = {
                factory.GetNameProperty(),
                factory.GetDataSourceProperty(),
                factory.GetVisibilityProperty(),
                factory.GetClickEvent()                
            }
        };
        descriptions.PropertyDescriptions.AddRange(_properties.Select(x => factory.Create(x.Name)));
        return descriptions;
    }

    public static Dictionary<string, PropertyDescription> GetPropertyDescriptions()
        => _properties.ToDictionary(x => x.Name);

    private static PropertyDescription GetPropertyDescription(
        PropertyType type, string name, int priority, string? label = null, Type? editorType = null)
    {
        var property = new PropertyDescription
        {
            Type = typeof(string),
            DisplayName = label,
            Priority = priority
        };
        switch (type)
        {
            case PropertyType.Label:
                property.Name = name + "Label";
                property.Category = PropertyCategoryConstants.LabelTextCategory;
                break;
            case PropertyType.Field:
                property.Name = name + "Field";
                property.Category = PropertyCategoryConstants.DataCategory;
                property.Editor = editorType;
                property.RefreshProperties = true;
                property.RefreshValue =  (_, currentValue, _, _) => currentValue;
                break;
            case PropertyType.Binding:
                property.Name = name + "Binding";
                property.Hidden = true;
                property.GetBindingLoader = () => new SimpleBindingLoader {
                    DataFieldProperty = name + "Field"
                };
                property.GenerateDefaultValue = () => Guid.NewGuid().ToString();
                property.GenerateValueOnPaste = _ => Guid.NewGuid().ToString();
                break;
            case PropertyType.UIName:
                property.Name = name + "UIName";
                property.Category = PropertyCategoryConstants.DataCategory;
                break;
        };
        return property;
    }

    private static PropertyDescription[] GetPropertyDescriptionGroup(
        string name, int priority, string nameLabel, string label, Type editorType) => [
            GetPropertyDescription(PropertyType.Label, name, priority, label),
            GetPropertyDescription(PropertyType.Field, name, priority, nameLabel, editorType),
            GetPropertyDescription(PropertyType.Binding, name, priority)
        ];
}
