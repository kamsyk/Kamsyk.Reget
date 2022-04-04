using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kamsyk.Reget.Model.Repositories.AppTextStoreRepository;

namespace Kamsyk.Reget.ScheduledTasks {
    public class Request {
        public void AddRequestTextFullIndex(DateTime dateFrom, DateTime dateTo) {
            RequestRepository requestRepository = new RequestRepository();
            AppTextStoreRepository appTextStoreRepository = new AppTextStoreRepository();
            var lastEvents = requestRepository.GetAllLastEvents(dateFrom, dateTo);
            int iIndex = 0;
            int iCount = lastEvents.Count;
            foreach (var request in lastEvents) {
                if (String.IsNullOrEmpty(request.request_text.Trim())) {
                    continue;
                }

                Console.WriteLine(dateFrom.Year + " Updating Full Index " + iIndex + "/" + iCount);

                //Request Text
                AddFullText(appTextStoreRepository, request, TextType.RequestText, request.request_text);

                //Request Nr
                AddFullText(appTextStoreRepository, request, TextType.RequestNr, request.request_nr);

                iIndex++;
            }
        }

        private void AddFullText(
            AppTextStoreRepository appTextStoreRepository, 
            Request_Event request,
             AppTextStoreRepository.TextType textType,
             string requestText) {

            var appStoreText = appTextStoreRepository.GetAppTextStoreByIdVersionType(
                    request.id,
                    request.version,
                    textType);

            if (appStoreText == null) {
                if (request.country_id != null) {
                    var actText = appTextStoreRepository.GetActiveAppTextStoreByIdType(request.id, textType);
                    if (actText != null) {
                        if (actText.text_content != request.request_text) {
                            appTextStoreRepository.AddRequestAppTest(
                            request.id,
                            request.version,
                            requestText,
                            textType,
                            (int)request.country_id,
                            request.modify_date);
                        }
                    } else {
                        appTextStoreRepository.AddRequestAppTest(
                            request.id,
                            request.version,
                            requestText,
                            textType,
                            (int)request.country_id,
                            request.modify_date);
                    }
                }
            }
        }
    }
}
