using Kamsyk.Reget.Model.Repositories.Interfaces;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using System.Collections;

namespace Kamsyk.Reget.Model.Repositories {
    public class CurrencyRepository : BaseRepository<Currency> {
        
        #region Methods

        public List<CurrencyExtended> GetActiveCurrenciesJs() {
            var currencies = (from currDb in m_dbContext.Currency
                              where currDb.active == true
                              orderby currDb.currency_code
                              select currDb).ToList();

            List<CurrencyExtended> retCurrencies = new List<CurrencyExtended>();
            foreach (var currency in currencies) {
                CurrencyExtended c = new CurrencyExtended();
                SetValues(currency, c);
                c.currency_code_name = currency.currency_code + " " + currency.currency_name;
                retCurrencies.Add(c);
            }

            return retCurrencies;
        }

        public Currency GetCurrencyById(int id) {
            var currency = (from currDb in m_dbContext.Currency
                              where currDb.id == id
                              select currDb).FirstOrDefault();

            return currency;
        }

        #endregion
    }
}
