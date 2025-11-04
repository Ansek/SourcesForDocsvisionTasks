using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel.Search;
using DocsVision.Platform.WebClient;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle.Options;
using Task7.Const;
using Task7.Services;

namespace Task7.CardLifeCycle
{
    internal class TravelRequestCardLifeCycle : ICardLifeCycleEx
    {
        public TravelRequestCardLifeCycle(ICardLifeCycleEx baseLifeCycle, ITravelRequest service)
        {
            BaseLifeCycle = baseLifeCycle;
            TravelRequestService = service;
        }
        protected ICardLifeCycleEx BaseLifeCycle { get; }
        protected ITravelRequest TravelRequestService { get; }
        public Guid CardTypeId => BaseLifeCycle.CardTypeId;

        public Guid Create(SessionContext sessionContext, CardCreateLifeCycleOptions options)
        {
            var cardId = BaseLifeCycle.Create(sessionContext, options);
            var context = sessionContext.ObjectContext;
            var travelRequestKind = context.FindObject<KindsCardKind>(
                new QueryObject(KindsCardKind.NameProperty.Name, TravelRequestCard.KindName));

            if (options.CardKindId == travelRequestKind.GetObjectId())
            {
                TravelRequestService.InitTravelRequestKind(sessionContext, cardId);
            }
            return cardId;
        }

        public bool Validate(SessionContext sessionContext, CardValidateLifeCycleOptions options, out List<ValidationResult> validationResults)
            => BaseLifeCycle.Validate(sessionContext, options, out validationResults);

        public void OnSave(SessionContext sessionContext, CardSaveLifeCycleOptions options)
            => BaseLifeCycle.OnSave(sessionContext, options);

        public bool CanDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options, out string message)
            => BaseLifeCycle.CanDelete(sessionContext, options, out message);

        public void OnDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options)
            => BaseLifeCycle.OnDelete(sessionContext, options);

        public string GetDigest(SessionContext sessionContext, CardDigestLifeCycleOptions options) 
            => BaseLifeCycle.GetDigest(sessionContext, options);
    }
}
