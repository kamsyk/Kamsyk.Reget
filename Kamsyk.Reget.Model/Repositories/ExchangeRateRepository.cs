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
    public class ExchangeRateRepository : BaseRepository<Exchange_Rate> {


        #region Methods

        public List<Exchange_Rate> GetCrossExchangeRatesJs() {
            var activeCurr = (from currDb in m_dbContext.Currency
                              //where currDb.active == true
                              select currDb).ToList();

            var exchangeRates = (from exrDb in m_dbContext.Exchange_Rate
                              select exrDb).ToList();

            List<Exchange_Rate> retExchageRate = new List<Exchange_Rate>();

            for (int i = 0; i < activeCurr.Count; i++) {
                for (int j = i; j < activeCurr.Count; j++) {
                    if (i == j) {
                        continue;
                    }

                    var exR = (from exDb in exchangeRates
                               where exDb.source_currency_id == activeCurr[i].id &&
                               exDb.destin_currency_id == activeCurr[j].id
                               select exDb).FirstOrDefault();
                    if (exR != null) {
                        Exchange_Rate exchange_Rate = new Exchange_Rate();
                        SetValues(exR, exchange_Rate);
                        retExchageRate.Add(exchange_Rate);
                        continue;
                    }

                    exR = (from exDb in exchangeRates
                           where exDb.destin_currency_id == activeCurr[i].id &&
                           exDb.source_currency_id == activeCurr[j].id
                           select exDb).FirstOrDefault();
                    if (exR != null) {
                        Exchange_Rate exchange_Rate = new Exchange_Rate();
                        SetValues(exR, exchange_Rate);
                        retExchageRate.Add(exchange_Rate);
                        continue;
                    }

                    exR = GetCrossExchnageRate(activeCurr[i].id, activeCurr[j].id, exchangeRates);
                    if (exR != null) {
                        Exchange_Rate exchange_Rate = new Exchange_Rate();
                        SetValues(exR, exchange_Rate);
                        retExchageRate.Add(exchange_Rate);
                        continue;
                    }
                }
            }
                        
            return retExchageRate;
        }

        private Exchange_Rate GetCrossExchnageRate(int curr1Id, int curr2Id, List<Exchange_Rate> exchangeRates) {
            Hashtable htCrossExRate1 = new Hashtable();
            var exRates = (from exDb in exchangeRates
                       where exDb.source_currency_id == curr1Id
                       select exDb).ToList();

            if (exRates != null) {
                foreach (var exRate in exRates) {
                    if (!htCrossExRate1.ContainsKey(exRate.destin_currency_id)) {
                        htCrossExRate1.Add(exRate.destin_currency_id, exRate.exchange_rate1);
                    }
                }
            }

            exRates = (from exDb in exchangeRates
                       where exDb.destin_currency_id == curr1Id
                       select exDb).ToList();

            if (exRates != null) {
                foreach (var exRate in exRates) {
                    if (!htCrossExRate1.ContainsKey(exRate.source_currency_id)) {
                        decimal exRateRev = 1 / exRate.exchange_rate1;
                        htCrossExRate1.Add(exRate.source_currency_id, exRateRev);
                    }
                }
            }

            //Find Exchange Rate 2
            exRates = (from exDb in exchangeRates
                           where exDb.source_currency_id == curr2Id
                           select exDb).ToList();

            if (exRates != null) {
                foreach (var exRate in exRates) {
                    if (htCrossExRate1.ContainsKey(exRate.destin_currency_id)) {
                        Exchange_Rate tmpExRate = new Exchange_Rate();
                        tmpExRate.source_currency_id = curr1Id;
                        tmpExRate.destin_currency_id = curr2Id;
                        decimal exRateRev =
                            (decimal)htCrossExRate1[exRate.destin_currency_id] / (decimal)exRate.exchange_rate1;
                        tmpExRate.exchange_rate1 = exRateRev;

                        return tmpExRate;
                    }
                }
            }

            exRates = (from exDb in exchangeRates
                       where exDb.destin_currency_id == curr2Id
                       select exDb).ToList();

            if (exRates != null) {
                foreach (var exRate in exRates) {
                    if (htCrossExRate1.ContainsKey(exRate.source_currency_id)) {
                        Exchange_Rate tmpExRate = new Exchange_Rate();
                        tmpExRate.source_currency_id = curr1Id;
                        tmpExRate.destin_currency_id = curr2Id;
                        tmpExRate.exchange_rate1 = (decimal)htCrossExRate1[exRate.source_currency_id] * (decimal)exRate.exchange_rate1;

                        return tmpExRate;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
