using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.Layout.WebClient.Services;
using DocsVision.WebClient.Extensibility;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle;
using Microsoft.Extensions.DependencyInjection;
using Task7.CardLifeCycle;
using Task7.DataGridPlugin;
using Task7.Services;

namespace Task7;

/// <summary>
/// Задаёт описание расширения для WebClient, которое задано в текущей сборке
/// </summary>
public class TravelRequestServiceExtension : WebClientExtension
{
    /// <summary>
    /// Создаёт новый экземпляр <see cref="TravelRequestServiceExtension" />
    /// </summary>
    public TravelRequestServiceExtension() : base() {}

    /// <summary>
    /// Получить название расширения
    /// </summary>
    public override string ExtensionName => "TravelRequestServiceExtension";

    /// <summary>
    /// Получить версию расширения
    /// </summary>
    public override Version ExtensionVersion => new(1, 0, 0);

    /// <summary>
    /// Регистрация типов в IoC контейнере
    /// </summary>
    /// <param name="containerBuilder"></param>
    public override void InitializeServiceCollection(IServiceCollection services)
    {
        services.AddSingleton<ITravelRequest, TravelRequest>();
		services.AddSingleton<IDataGridControlPlugin, BusinessTripRequestPlugin>();
        // Декорируем базовый CardLifeCycle через Scrutor
        services.Decorate<ICardLifeCycleEx>((original, serviceProvider) =>
        {
            var typeId = original.CardTypeId;
            if (typeId == CardDocument.ID)
            {
                var feature1Service = serviceProvider.GetRequiredService<ITravelRequest>();
                return new TravelRequestCardLifeCycle(original, feature1Service);
            }
            return original;
        });
    }
}