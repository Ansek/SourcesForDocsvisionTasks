import { Layout } from "@docsvision/webclient/System/Layout";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { NumberControl } from "@docsvision/webclient/Platform/Number";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";

export class ApplicationRequestBusinessTravelLogic {
    // Количество миллисекунд в одном дне
    private static readonly MS_IN_DAY = 1000 * 60 * 60 * 24;

    /**
     * Проверяет поля на наличие значений.
     * @param layout Разметка.
     */ 
    public hasInvalidRequiredFields(layout: Layout, control: LayoutControl) {
        const controls = layout.layout.controls;
        let validationResults = control.validate({ ShowErrorMessage: true }) || [];
        let invalidResults = validationResults.filter((value) => !value.Passed);
        let labelTexts = invalidResults.map((control) => controls.get<any>(`${control.ControlName}`).params.labelText);
            
        let tickets = layout.controls.get<Dropdown>("tickets");
        if (tickets.params.value === null)
            labelTexts.push(tickets.params.labelText)

        if (labelTexts.length !== 0) {
            const text = 'Необходимо заполнить ' + (labelTexts.length === 1 ? 'поле': 'поля');
            layout.layout.params.services.messageWindow.showWarning(`${text}: ${labelTexts.join(', ')}.`);
            return true;
        }
        return false;
    }

    /**
     * Проверка корректного расположения дат командировки.
     * @param fromDate Дата командировки 'c'.
     * @param toDate Дата командировки 'по'.
     */ 
    private checkTravelDateOrder(fromDate: Date, toDate: Date) {
        if (fromDate.getTime() > toDate.getTime()) {
            let msg = "Значение даты командировки 'с' ("
            msg += fromDate.toLocaleDateString('ru-RU');
            msg += ") должно быть меньше или равно дате 'по' ("
            msg += toDate.toLocaleDateString('ru-RU') + ')';
            MessageBox.ShowWarning(msg);
            return false;
        }
        return true;
    }

    /**
     * Обновляет значение количества дней командировки.
     * @param layout Разметка.
     * @param fromDate Дата командировки 'c'.
     * @param toDate Дата командировки 'по'.
     */ 
    private updateDaysCount(layout: Layout, fromDate: Date, toDate: Date) {
        let dayCount = layout.controls.get<NumberControl>("dayCount");
        let ms_in_day = ApplicationRequestBusinessTravelLogic.MS_IN_DAY;
        dayCount.params.value = (toDate.getTime() - fromDate.getTime()) / ms_in_day + 1;
    }

    /**
     * Дополнительные действия после изменения даты командировки.
     * @param layout Разметка.
     */ 
    public onTravelDateChanged(dateTravel: DateTimePicker, oldValue: Date) {
        let layout = dateTravel.layout;
        let fromDate = layout.controls.get<DateTimePicker>("fromTravelDate").params.value;
        let toDate = layout.controls.get<DateTimePicker>("toTravelDate").params.value;
        if (fromDate && toDate) {
            if (this.checkTravelDateOrder(fromDate, toDate))
                this.updateDaysCount(layout, fromDate, toDate);
            else
                dateTravel.params.value = oldValue;
        }
    }

    /**
     * Показывает данные о карточке в диалоговом окне.
     * @param layout Разметка.
     */
    public showCardData(layout: Layout) {
        let cardName = layout.controls.get<TextBox>("cardName");
        let createDate = layout.controls.get<DateTimePicker>("createDate");
        let fromTravelDate = layout.controls.get<DateTimePicker>("fromTravelDate");
        let toTravelDate = layout.controls.get<DateTimePicker>("toTravelDate");
        let reasonTravel = layout.controls.get<TextArea>("reasonTravel");
        let city = layout.controls.get<DirectoryDesignerRow>("city");

        let notValueMsg = "<не задано>";
        const formatDate = (d: Date): string => {
            if (d)
                return d.toLocaleDateString('ru-RU');
            else 
                return notValueMsg;
        } 

        let cardNameValue = cardName.params.value ?? notValueMsg;
        let createDateValue = formatDate(createDate.params.value);
        let fromTravelDateValue = formatDate(fromTravelDate.params.value);
        let toTravelDateValue = formatDate(toTravelDate.params.value);
        let reasonTravelValue = reasonTravel.params.value ?? notValueMsg;
        let cityValue = city.params.value?.name ?? notValueMsg;

        let msg = `Название: ${cardNameValue}\n`;
        msg += `Дата создания: ${createDateValue}\n`;
        msg += `Дата командировки с ${fromTravelDateValue} по ${toTravelDateValue}\n`;
        msg += `Город: ${cityValue}\n`;
        msg += 'Основание для поездки:\n' + reasonTravelValue;

        MessageBox.ShowInfo(msg);
    }
}