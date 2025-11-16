import React from "react";
import { format, parseISO } from 'date-fns';
import { ITicketInfo } from "../../Model/ITicketInfo";
import { r } from "@docsvision/webclient/System/Readonly";
import { rw } from "@docsvision/webclient/System/Readwrite";
import { ControlImpl } from "@docsvision/webclient/System/ControlImpl";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { Button, ButtonAlignModes } from "@docsvision/webclient/Helpers/Button";
import { $TravelRequestService } from "../../Services/Interfaces/ITravelRequestService";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { BaseControl, BaseControlParams, BaseControlState } from "@docsvision/webclient/System/BaseControl";
import { Dropdown } from "@docsvision/webclient/Platform/Dropdown";
import { MessageBox } from "@docsvision/webclient/Helpers/MessageBox/MessageBox";
import { IBindingResult } from "@docsvision/webclient/System/IBindingResult";
import { at, handler } from "@docsvision/webclient/System/Handler";
import { getBindingResult } from "@docsvision/webclient/System/GetBindingResult";

export class SearchTicketsControlParams extends BaseControlParams {
    @rw price?: number;
    @rw departureFlight?: string;
    @rw returnFlight?: string;
    @r priceLabel?: string;
    @r departureFlightLabel?: string;
    @r returnFlightLabel?: string;
    @r destinationUIName?: string;
    @r departureAtUIName?: string;
    @r returnAtUIName?: string;
    @r services?: $TravelRequestService & $ControlStore;
}

export interface SearchTicketsControlState extends SearchTicketsControlParams, BaseControlState {
    ticketInfo?: ITicketInfo[];
    priceBinding: IBindingResult<number>;
    departureFlightBinding: IBindingResult<string>;
    returnFlightBinding: IBindingResult<string>;
}

export class SearchTicketsControl extends BaseControl<SearchTicketsControlParams, SearchTicketsControlState> {
    protected construct(): void {
        super.construct();
        this.state.ticketInfo = null;
        this.state.price = null;
    }

    protected createParams() {
        return new SearchTicketsControlParams();
    }

    protected createImpl() { 
        return new ControlImpl(this.props, this.state, this.renderControl.bind(this));
    }
    
    @handler("priceBinding")
    private set priceBinding(binding: IBindingResult<number>) {
        this.state.price = binding && binding.value;
        this.state.priceBinding = binding;
    }

    @handler("departureFlightBinding")
    private set departureFlightBinding(binding: IBindingResult<string>) {
        this.state.departureFlight = binding && binding.value;
        this.state.departureFlightBinding = binding;
    }

    @handler("returnFlightBinding")
    private set returnFlightBinding(binding: IBindingResult<string>) {
        this.state.returnFlight = binding && binding.value;
        this.state.returnFlightBinding = binding;
    }

    protected getBindings() {
        return [
            getBindingResult(this.state.priceBinding, this.params.price, () => at(SearchTicketsControlParams).price),
            getBindingResult(this.state.departureFlightBinding, this.params.departureFlight, () => at(SearchTicketsControlParams).departureFlight),
            getBindingResult(this.state.returnFlightBinding, this.params.returnFlight, () => at(SearchTicketsControlParams).returnFlight),        
        ];
    }

    changedHandler = (event) => {
        this.state.price = event.target.price;
        this.state.departureFlight = event.target.departureFlight;
        this.state.returnFlight = event.target.returnFlight;
        this.forceUpdate();
    }

    private departureFlightToString(info: ITicketInfo): string {
        return `${info.originAirport}→${info.destinationAirport} ${info.airline}-${info.flightNumber} ${format(parseISO(info.departureAt), 'HH:mm dd.MM.yyyy')}`
    }

    private returnFlightToString(info: ITicketInfo): string {
        return `${info.destinationAirport}→${info.originAirport} ${info.airline}-${info.flightNumber} ${format(parseISO(info.returnAt), 'HH:mm dd.MM.yyyy')}`
    }

    private ticketInfoToString(info: ITicketInfo): string {
        return `${info.price}₽ (`+
            this.departureFlightToString(info) + ', ' +
            this.returnFlightToString(info) + ')';
    }

    private async onButtonClick() {
        const destinationId = this.params.services.controlStore.tryGet<DirectoryDesignerRow>(this.params.destinationUIName)?.params.value?.id; 
        const departureAt = this.params.services.controlStore.tryGet<DateTimePicker>(this.params.departureAtUIName)?.params.value;
        const returnAt = this.params.services.controlStore.tryGet<DateTimePicker>(this.params.returnAtUIName)?.params.value;
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
        const info = this.state.ticketInfo[index]
        this.setState({
            price: info.price,
            departureFlight: this.departureFlightToString(info),
            returnFlight: this.returnFlightToString(info)
        });
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
                <div style={{ margin: 10 }}>
                    <table style={{ float: 'right' }}>
                        <tbody>
                            <tr>
                                <td>{this.params.priceLabel}</td>
                                <td>{this.params.price}₽</td>
                            </tr>
                            <tr>
                                <td>{this.params.departureFlightLabel}</td>
                                <td>{this.params.departureFlight}</td>
                            </tr>
                            <tr>
                                <td>{this.params.returnFlightLabel}&ensp;</td>
                                <td>{this.params.returnFlight}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            )}
            </div>
        );
    }
}