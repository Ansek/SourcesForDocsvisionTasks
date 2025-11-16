using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using DocsVision.Platform.WebLayoutsDesigner.NewEditors;
using DocsVision.Platform.Data.Metadata.CardModel;

namespace Task11.Controls;

internal class IntMetadataEditor() : BaseMetadataEditor(FieldType.Int);
internal class StringMetadataEditor() : BaseMetadataEditor(FieldType.String);

internal class BaseMetadataEditor(FieldType fieldType) : ITypeEditor
{
    public FrameworkElement ResolveEditor(PropertyItem propertyItem)
    {
        var editor = new FieldMetadataEditor
        {
            FieldFilter = (field) => { return field.FieldType == fieldType; }
        };
        return editor.ResolveEditor(propertyItem);
    }
}
