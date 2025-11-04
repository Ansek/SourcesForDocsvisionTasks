import { Service } from "@docsvision/web/core/services";
import * as ApplicationRequestBusinessTravelHandlers from "./EventHandlers/ApplicationRequestBusinessTravelHandlers";
import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { TravelRequestService } from "./Services/TravelRequestService";
import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { $TravelRequestService } from "./Services/Interfaces/ITravelRequestService";

// Главная входная точка всего расширения
// Данный файл должен импортировать прямо или косвенно все остальные файлы, 
// чтобы rollup смог собрать их все в один бандл.

// Регистрация расширения позволяет корректно установить все
// обработчики событий, сервисы и прочие сущности web-приложения.
extensionManager.registerExtension({
    name: "Template web extension",
    version: "1.0",
    globalEventHandlers: [ ApplicationRequestBusinessTravelHandlers ],
    layoutServices: [
        Service.fromFactory($TravelRequestService, (services: $RequestManager) => new TravelRequestService(services))
    ],
    controls: []
})