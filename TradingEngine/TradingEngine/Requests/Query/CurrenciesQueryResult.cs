using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TradingEngine.Domain.CurrencyDomain;
using TradingEngine.Infrastructure.Repositories.Interface;

namespace TradingEngine.Requests.Query
{
    public class CurrenciesQuery : IRequest<IEnumerable<CurrenciesQueryResult>>
    {

    }

    public class CurrenciesQueryHandler : IRequestHandler<CurrenciesQuery, IEnumerable<CurrenciesQueryResult>>
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrenciesQueryHandler(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public Task<IEnumerable<CurrenciesQueryResult>> Handle(CurrenciesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Currency> currencies = _currencyRepository.GetAll();

            IEnumerable<CurrenciesQueryResult> result = currencies.Select(x => new CurrenciesQueryResult
            {
                Key = x.Key,
                Ratio = x.Ratio
            });

            return Task.FromResult(result);
        }
    }

    public class CurrenciesQueryResult
    {
        public string Key { get; set; }
        public decimal Ratio { get; set; }
    }
}