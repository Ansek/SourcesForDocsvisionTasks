import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { ControllerBase, HttpMethods } from "@docsvision/webclient/System/ControllerBase";
import { ITravelRequestService } from "./Interfaces/ITravelRequestService";
import { ITravelInfoRequest } from "../Model/ITravelInfoRequest";
import { ITravelInfo } from "../Model/ITravelInfo";
import { IManagerInfo } from "../Model/IManagerInfo";
import { IBusinessTripRequest } from "../Model/IBusinessTripRequest";
import { ITicketInfoRequest } from "../Model/ITicketInfoRequest";
import { ITicketInfo } from "../Model/ITicketInfo";

export class TravelRequestService extends ControllerBase implements ITravelRequestService {

    protected controllerName: string = "TravelRequest";

    constructor(protected services: $RequestManager) {
        super(services);
    }

    getManagerInfoByEmployeeId(employeeId: string): Promise<IManagerInfo> {
        return super.doRequest({
            controller: this.controllerName,
            action: 'GetManagerInfoByEmployeeId',
            isApi: false,
            method: HttpMethods.Get,
            data: { employeeId },
            options: { isShowOverlay: true }
        });
    }

    calculateTravelInfo(model: ITravelInfoRequest): Promise<ITravelInfo> {
        return super.doRequest({
            controller: this.controllerName,
            action: 'CalculateTravelInfo',
            isApi: false,
            method: HttpMethods.Post,
            data: model,
            options: { isShowOverlay: true }
        });
    }

    setBusinessTripStatus(model: IBusinessTripRequest): void {
        super.doRequest({
            controller: this.controllerName,
            action: 'SetBusinessTripStatus',
            isApi: false,
            method: HttpMethods.Post,
            data: model,
            options: { isShowOverlay: true }
        });
    }

    searchTickets(model: ITicketInfoRequest): Promise<ITicketInfo[]> {
        return super.doRequest({
            controller: this.controllerName,
            action: 'SearchTickets',
            isApi: false,
            method: HttpMethods.Post,
            data: model,
            options: { isShowOverlay: true }
        });
    }
}