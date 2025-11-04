import { serviceName } from "@docsvision/web/core/services";
import { ITravelInfoRequest } from "../../Model/ITravelInfoRequest";
import { ITravelInfo } from "../../Model/ITravelInfo";
import { IManagerInfo } from "../../Model/IManagerInfo";

export interface ITravelRequestService {
    getManagerInfoByEmployeeId(employeeId: string): Promise<IManagerInfo>
    calculateTravelInfo(model: ITravelInfoRequest): Promise<ITravelInfo>
}

export type $TravelRequestService = { activityPlanService: ITravelRequestService };
export const $TravelRequestService = serviceName<$TravelRequestService, ITravelRequestService>(x => x.activityPlanService);