import { serviceName } from "@docsvision/web/core/services";
import { ITravelInfoRequest } from "../../Model/ITravelInfoRequest";
import { ITravelInfo } from "../../Model/ITravelInfo";
import { IManagerInfo } from "../../Model/IManagerInfo";
import { IBusinessTripRequest } from "../../Model/IBusinessTripRequest";

export interface ITravelRequestService {
    getManagerInfoByEmployeeId(employeeId: string): Promise<IManagerInfo>
    calculateTravelInfo(model: ITravelInfoRequest): Promise<ITravelInfo>
    setBusinessTripStatus(model: IBusinessTripRequest): void
}

export type $TravelRequestService = { activityPlanService: ITravelRequestService };
export const $TravelRequestService = serviceName<$TravelRequestService, ITravelRequestService>(x => x.activityPlanService);