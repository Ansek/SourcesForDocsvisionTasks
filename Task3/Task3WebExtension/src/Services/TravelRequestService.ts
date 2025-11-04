import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { ControllerBase, HttpMethods } from "@docsvision/webclient/System/ControllerBase";
import { ITravelRequestService } from "./Interfaces/ITravelRequestService";
import { ITravelInfoRequest } from "../Model/ITravelInfoRequest";
import { ITravelInfo } from "../Model/ITravelInfo";
import { IManagerInfo } from "../Model/IManagerInfo";

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
}