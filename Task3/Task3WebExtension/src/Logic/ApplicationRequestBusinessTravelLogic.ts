import { Layout } from "@docsvision/webclient/System/Layout";
import { TextBox } from "@docsvision/webclient/Platform/TextBox";
import { TextArea } from "@docsvision/webclient/Platform/TextArea";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { NumberControl } from "@docsvision/webclient/Platform/Number";
import { LayoutControl } from "@docsvision/webclient/System/BaseControl";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { $TravelRequestService } from "../Services/Interfaces/ITravelRequestService";
import { $EmployeeController } from "@docsvision/webclient/Generated/DocsVision.WebClient.Controllers";
import { GenModels } from "@docsvision/webclient/Generated/DocsVision.WebClient.Models";
import { StaffDirectoryItems, StaffValueType } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";

export class ApplicationRequestBusinessTravelLogic {
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
     * Заполнение полей количества дней и суммы командировочных.
     * @param layout Поле выбора даты.
     * @param fromDate Дата командировки 'c'.
     * @param toDate Дата командировки 'по'.
     * @param cityId Город.
     */ 
    private async updateDaysCountAndAmount(layout: Layout, fromDate: Date, toDate: Date, cityId: string) {
        let dayCount = layout.controls.get<NumberControl>("dayCount");
        let amount = layout.controls.get<NumberControl>("amountTravel");
        const response = await layout.getService($TravelRequestService).calculateTravelInfo({
            fromDate: fromDate,
            toDate: toDate,
            cityId: cityId
        })
        dayCount.params.value = response.dayCount;
        amount.params.value = response.amount;
    }

    /**
     * Дополнительные действия после изменения даты командировки.
     * @param dateTravel Поле выбора даты.
     * @param oldValue Старая дата.
     */ 
    public onTravelDateChanged(dateTravel: DateTimePicker, oldValue: Date) {
        let layout = dateTravel.layout;
        let fromDate = layout.controls.get<DateTimePicker>("fromTravelDate").params.value;
        let toDate = layout.controls.get<DateTimePicker>("toTravelDate").params.value;
        if (fromDate && toDate) {
            if (this.checkTravelDateOrder(fromDate, toDate)) {
                let cityId = layout.controls.get<DirectoryDesignerRow>("city").params.value?.id;
                this.updateDaysCountAndAmount(layout, fromDate, toDate, cityId);
            }
            else
                dateTravel.params.value = oldValue;
        }
    }

    /**
     * Дополнительные действия после изменения города.
     * @param layout Разметка.
     * @param city Выбранный город.
     */ 
    public onCityChanged(layout: Layout, city: GenModels.DirectoryDesignerItem) {
        let fromDate = layout.controls.get<DateTimePicker>("fromTravelDate").params.value;
        let toDate = layout.controls.get<DateTimePicker>("toTravelDate").params.value;
        if (fromDate && toDate) {
            this.updateDaysCountAndAmount(layout, fromDate, toDate, city.id);
        }
    }

    /**
     * Дополнительные действия после обновления командируемого.
     * @param layout Разметка.
     * @param traveler Поле выбора командируемого.
     */ 
    public async onTravelerChanged(layout: Layout, traveler: GenModels.IDirectoryItemData) {
        if (traveler && traveler.dataType === GenModels.DirectoryDataType.Employee) {
            let managerItem = layout.controls.get<StaffDirectoryItems>("manager");
            let phoneNumber = layout.controls.get<TextBox>("phoneNumber");
            const managerInfo = await layout
                .getService($TravelRequestService).getManagerInfoByEmployeeId(traveler.id);
            const manager = await layout
                .getService($EmployeeController).getEmployee(managerInfo.id);
            managerItem.params.value = manager;
            phoneNumber.params.value = managerInfo.phone;
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

    /**
     * Дополнительные действия после обновления состояния.
     * @param layout Разметка.
     * @param branchName Название перехода в другое состояние.
     */
    public onStateButtonsChanged(layout: Layout, branchName: string) {
        if (branchName === "На оформление") {
            let fromDate = layout.controls.get<DateTimePicker>("fromTravelDate").params.value;
            let toDate = layout.controls.get<DateTimePicker>("toTravelDate").params.value;
            let traveler = layout.controls.get<StaffDirectoryItems>("traveler")
                .params.value as GenModels.IDirectoryItemData;
            if (traveler && traveler.dataType === GenModels.DirectoryDataType.Employee) {
                layout.getService($TravelRequestService).setBusinessTripStatus({
                    employeeId: traveler.id,
                    fromDate: fromDate,
                    toDate: toDate
                });
            }
        }
    }
}


