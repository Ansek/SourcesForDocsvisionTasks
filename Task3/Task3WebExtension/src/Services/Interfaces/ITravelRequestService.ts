import { serviceName } from "@docsvision/web/core/services";
import { ITravelInfoRequest } from "../../Model/ITravelInfoRequest";
import { ITravelInfo } from "../../Model/ITravelInfo";
import { IManagerInfo } from "../../Model/IManagerInfo";
import { IBusinessTripRequest } from "../../Model/IBusinessTripRequest";
import { ITicketInfoRequest } from "../../Model/ITicketInfoRequest";
import { ITicketInfo } from "../../Model/ITicketInfo";

export interface ITravelRequestService {
    getManagerInfoByEmployeeId(employeeId: string): Promise<IManagerInfo>
    calculateTravelInfo(model: ITravelInfoRequest): Promise<ITravelInfo>
    setBusinessTripStatus(model: IBusinessTripRequest): void
    searchTickets(model: ITicketInfoRequest): Promise<ITicketInfo[]>
}

export type $TravelRequestService = { travelRequestService: ITravelRequestService };
export const $TravelRequestService = serviceName<$TravelRequestService, ITravelRequestService>(x => x.travelRequestService);