import { Layout } from "@docsvision/webclient/System/Layout";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { CancelableEventArgs } from "@docsvision/webclient/System/CancelableEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { ApplicationRequestBusinessTravelLogic } from "../Logic/ApplicationRequestBusinessTravelLogic";
import { IDataChangedEventArgs } from "@docsvision/webclient/System/IDataChangedEventArgs";

/**
 * Событие после открытия карточки.
 * @param layout Разметка.
 */
export function ddApplicationRequestBusiness_cardOpened(layout:Layout) {
    if (!layout) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    logic.initFields(layout);
}

/**
 * Событие перед сохранением карточки.
 * @param layout Разметка.
 * @param args Аргументы.
 */
export function ddApplicationRequestBusiness_cardSaving(
    layout:Layout,
    args: CancelableEventArgs<ILayoutBeforeSavingEventArgs>) {
    if (!layout || !args.data?.control) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    args.wait();
    if (logic.hasInvalidRequiredFields(layout, args.data.control)) {
        args.cancel();
        return;
    }
    args.accept();
}

/**
 * Событие после обновления даты командировки.
 * @param dateTravel Поле ввода даты.
 * @param args Аргументы.
 */
export function ddApplicationRequestBusiness_dateTravelUpdated(
    dateTravel:DateTimePicker,
    args: IDataChangedEventArgs) {
    if (!dateTravel || args.newValue === args.oldValue) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    if (!logic.updateDaysCount(dateTravel.layout))
        dateTravel.params.value = args.oldValue;
}

/**
 * Событие нажатия кнопки для отображения данных о карточке.
 * @param btn Кнопка.
 */
export function ddApplicationRequestBusiness_showCardDataClicked(
    btn:CustomButton) {
    if (!btn) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    logic.showCardData(btn.layout);
}