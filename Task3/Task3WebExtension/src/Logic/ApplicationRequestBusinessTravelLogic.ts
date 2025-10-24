import { Layout } from "@docsvision/webclient/System/Layout";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { NumberControl } from "@docsvision/webclient/Platform/Number";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";

export class ApplicationRequestBusinessTravelLogic {
    // Количество миллисекунд в одном дне
    private static readonly MS_IN_DAY = 1000 * 60 * 60 * 24;

    /**
     * Инициализация полей разметки.
     * @param layout Разметка.
     */ 
    public initFields(layout: Layout) {
        let toTravelDate = layout.controls.get<DateTimePicker>("toTravelDate");
        let newDate = toTravelDate.params.value.getTime();
        newDate += ApplicationRequestBusinessTravelLogic.MS_IN_DAY;
        toTravelDate.params.value = new Date(newDate);
    }

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
     * Обновляет значение количества дней командировки.
     * @param layout Разметка.
     */ 
    public updateDaysCount(layout: Layout) {
        let fromTravelDate = layout.controls.get<DateTimePicker>("fromTravelDate");
        let toTravelDate = layout.controls.get<DateTimePicker>("toTravelDate");
        let dayCount = layout.controls.get<NumberControl>("dayCount");
        let fromDate = fromTravelDate.params.value;
        let toDate = toTravelDate.params.value;
        dayCount.params.value = (toDate.getTime() - fromDate.getTime()) / ApplicationRequestBusinessTravelLogic.MS_IN_DAY;
        if (dayCount.params.value < 0) {
            let msg = "Значение даты командировки 'c' больше 'по':\n";
            msg += `${fromDate.toLocaleDateString('ru-RU')} > `;
            msg += `${toDate.toLocaleDateString('ru-RU')}`;
            MessageBox.ShowWarning(msg);
            return false;
        } else if (dayCount.params.value === 0) {
            MessageBox.ShowWarning("Даты командировки 'c' и 'по' равны");
            return false;
        }
        return true;
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

        let msg = `Название: ${cardNameValue}\n`;
        msg += `Дата создания: ${createDateValue}\n`;
        msg += `Дата командировки с ${fromTravelDateValue} по ${toTravelDateValue}\n`;
        msg += 'Основание для поездки:\n' + reasonTravelValue;

        MessageBox.ShowInfo(msg);
    }
}