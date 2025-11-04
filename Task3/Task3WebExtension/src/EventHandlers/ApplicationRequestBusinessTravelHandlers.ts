import { Layout } from "@docsvision/webclient/System/Layout";
import { CustomButton } from "@docsvision/webclient/Platform/CustomButton";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { CancelableEventArgs } from "@docsvision/webclient/System/CancelableEventArgs";
import { ILayoutBeforeSavingEventArgs } from "@docsvision/webclient/System/ILayoutParams";
import { ApplicationRequestBusinessTravelLogic } from "../Logic/ApplicationRequestBusinessTravelLogic";
import { IDataChangedEventArgs, IDataChangedEventArgsEx } from "@docsvision/webclient/System/IDataChangedEventArgs";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { StaffDirectoryItems } from "@docsvision/webclient/BackOffice/StaffDirectoryItems";
import { GenModels } from "@docsvision/webclient/Generated/DocsVision.WebClient.Models";

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
 * @param dateTravel Поле выбора даты.
 * @param args Аргументы.
 */
export function ddApplicationRequestBusiness_dateTravelUpdated(
    dateTravel:DateTimePicker,
    args: IDataChangedEventArgs) {
    if (!dateTravel || args.newValue === args.oldValue) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    logic.onTravelDateChanged(dateTravel, args.oldValue);
}

/**
 * Событие после обновления города.
 * @param city Поле выбора города.
 * @param args Аргументы.
 */
export function ddApplicationRequestBusiness_CityUpdated(
    city: DirectoryDesignerRow,
    args: IDataChangedEventArgsEx<GenModels.DirectoryDesignerItem>) {
    if (!city || args.newValue === args.oldValue) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    logic.onCityChanged(city.layout, args.newValue);
}

/**
 * Событие после обновления командируемого.
 * @param traveler Поле выбора командируемого.
 * @param args Аргументы.
 */
export function ddApplicationRequestBusiness_TravelerUpdated(
    traveler: StaffDirectoryItems,
    args: IDataChangedEventArgsEx<GenModels.IDirectoryItemData>) {
    if (!traveler || args.newValue === args.oldValue) return;
    let logic = new ApplicationRequestBusinessTravelLogic();
    logic.onTravelerChanged(traveler.layout, args.newValue);
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