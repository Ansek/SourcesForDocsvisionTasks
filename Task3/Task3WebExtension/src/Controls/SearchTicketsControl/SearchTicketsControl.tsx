import React from "react";
import { format, parseISO } from 'date-fns';
import { ITicketInfo } from "../../Model/ITicketInfo";
import { r } from "@docsvision/webclient/System/Readonly";
import { ControlImpl } from "@docsvision/webclient/System/ControlImpl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { Button, ButtonAlignModes } from "@docsvision/webclient/Helpers/Button";
import { $TravelRequestService } from "../../Services/Interfaces/ITravelRequestService";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { BaseControl, BaseControlParams, BaseControlState } from "@docsvision/webclient/System/BaseControl";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";

export class SearchTicketsControlParams extends BaseControlParams {
    @r costLabel?: string;
    @r destinationUI?: string;
    @r departureAtUI?: string;
    @r returnAtUI?: string;
    @r services?: $TravelRequestService & $ControlStore;
}

export interface SearchTicketsControlState extends SearchTicketsControlParams, BaseControlState {
    ticketInfo?: ITicketInfo[];
    price?: number;
}

export class SearchTicketsControl extends BaseControl<SearchTicketsControlParams, SearchTicketsControlState> {
    protected construct(): void {
        super.construct();
        this.state.ticketInfo = null;
    }

    protected createParams() {
        return new SearchTicketsControlParams();
    }

    protected createImpl() { 
        return new ControlImpl(this.props, this.state, this.renderControl.bind(this));
    }

    private async onButtonClick() {
        const destinationId = this.params.services.controlStore.tryGet<DirectoryDesignerRow>(this.params.destinationUI)?.params.value?.id; 
        const departureAt = this.params.services.controlStore.tryGet<DateTimePicker>(this.params.departureAtUI)?.params.value;
        const returnAt = this.params.services.controlStore.tryGet<DateTimePicker>(this.params.returnAtUI)?.params.value;
        if (destinationId && departureAt && returnAt) {
            const ticketInfo = await this.params.services.travelRequestService.searchTickets({
                destinationId: destinationId,
                departureAt: departureAt,
                returnAt: returnAt
            });
            this.setState({
                ticketInfo: ticketInfo
            })
        } else {
            MessageBox.ShowWarning("Должны быть заданы значения дат командировок и города.");
        }
    }

    private onDataChanged(index: number) {
        this.setState({
            price: this.state.ticketInfo[index].price
        });
    }

    private ticketInfoToString(info: ITicketInfo): string {
        return `${info.airline} ${info.flightNumber} ${info.price}₽ [`+
            `аэропорт: ${info.originAirport} → ${info.destinationAirport}, ` +
            `отправление: ${format(parseISO(info.departureAt), 'HH:mm dd.MM.yyyy')}, ` +
            `возвращение: ${format(parseISO(info.returnAt), 'HH:mm dd.MM.yyyy')}]`;
    }

    renderControl() { 
        return (
            <div>
                <Button 
                    text={ 'Запросить стоимость билетов' }
                    align={ ButtonAlignModes.Center }
                    onClick={ () => this.onButtonClick() }/>
            { this.state.ticketInfo != null && (
                <Dropdown isEmptyKeyAllowed={false} dataChanged={(props) => this.onDataChanged(props.valueCode)}
                    items={this.state.ticketInfo.map(t => ({
                        key: t.id.toString(),
                        value: this.ticketInfoToString(t),
                        valueCode: t.id
                    }))}/>
            )}
            { this.state.price != null && (
                <p style={{ textAlign: 'right', margin: 10 }}>{this.params.costLabel} {this.state.price}</p>
            )}
            </div>
        );
    }
}