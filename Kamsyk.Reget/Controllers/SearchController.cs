using OTISCZ.ActiveDirectory;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.Repositories;
using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.CentreRepository;
using static Kamsyk.Reget.Model.Repositories.UserRepository;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.ExtendedModel.Search;
using static Kamsyk.Reget.Model.Repositories.AppTextStoreRepository;

namespace Kamsyk.Reget.Controllers
{
    public class SearchController : BaseController {

        #region Virtual Properties
        public override string HeaderImgUrl {
            get {
                return GetRootUrl() + "/Content/Images/Menu/Search16.png";
            }
        }

        public override string HeaderTitle {
            get {
                return RequestResource.SearchRequests;
            }
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public ActionResult GetSearch(string searchText, int currentPage, bool isSimpleSearch, bool isMyRequestsOnly) {
            try {
                string decFilter = DecodeUrl(searchText);
                List<int> compIds = CurrentUser.ParticipantAdminCompanyIds;
                int rowsCount;

                int iPageSize = (isSimpleSearch) ? 100 : 10;
                int iStartPage = (isSimpleSearch) ? 1 : currentPage;

                int rowsCountFullIndex;
                var appTexts = new AppTextStoreRepository().SearchText(
                    searchText, 
                    compIds,
                    iPageSize,
                    iStartPage,
                    isMyRequestsOnly,
                    CurrentUser.ParticipantId,
                    out rowsCountFullIndex);

                rowsCount = rowsCountFullIndex;

                List<RequestSearchResult> searchResults = new List<RequestSearchResult>();

                RequestTextRepository requestTextRepository = new RequestTextRepository();
                List<string> simpleResult = new List<string>();
                foreach (var appText in appTexts) {
                    RequestSearchResult requestSearchResult = new RequestSearchResult();

                    if (isSimpleSearch) {
                        string strSimpleText = GetShortResultText(appText.text_content, searchText);
                        if (simpleResult.Contains(strSimpleText.ToLower())) {
                            continue;
                        } else {
                            simpleResult.Add(strSimpleText.ToLower());
                        }
                        requestSearchResult.found_text_short = strSimpleText;
                    } else {
                        if (appText.text_type == (int)TextType.RequestText) {
                            var reqText = requestTextRepository.GetRequestTextByAppTextId(appText.id);
                            if (reqText != null) {
                                requestSearchResult.request_id = reqText.Request_Event.id;
                                requestSearchResult.request_nr = reqText.Request_Event.request_nr;
                                requestSearchResult.found_text = GetResultFoundFullText(appText.text_content);
                            }
                        } else if (appText.text_type == (int)TextType.RequestDisc) {
                            var discText = requestTextRepository.GetDiscussionTextByAppTextId(appText.id);
                            if (discText != null) {
                                requestSearchResult.request_id = discText.Request_Discussion.ElementAt(0).request_id;
                                requestSearchResult.request_nr = discText.Request_Discussion.ElementAt(0).Request_Event.request_nr;
                                requestSearchResult.found_text = GetResultFoundFullText(appText.text_content);
                            }
                        } else if (appText.text_type == (int)TextType.RequestNr) {
                            var reqText = requestTextRepository.GetRequestTextByAppTextId(appText.id);
                            if (reqText != null) {
                                requestSearchResult.request_id = reqText.Request_Event.id;
                                requestSearchResult.request_nr = reqText.Request_Event.request_nr;
                                requestSearchResult.found_text = GetResultFoundFullText(reqText.Request_Event.request_text);
                            }
                        }
                    }
                                        
                    searchResults.Add(requestSearchResult);
                }

                PartData<RequestSearchResult> sr = new PartData<RequestSearchResult>();
                sr.db_data = searchResults;
                sr.rows_count = rowsCount;

                return GetJson(sr);

            } catch (Exception ex) {
                HandleError(ex);
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, MediaTypeNames.Text.Plain);
            }
        }
        #endregion

        #region Methods
        private string GetShortResultText(string foundText, string searchText) {
            if (searchText.Contains(" ")) {
                return GetShortResultTextWithSpace(GetResultFoundText(foundText), searchText);
            } else {
                return GetShortResultTextNoSpace(GetResultFoundText(foundText), searchText);
            }
        }

        private string GetShortResultTextWithSpace(string foundText, string searchText) {
            string[] searchItems = searchText.Split(' ');
            string strResult = "";
            foreach (var searchItem in searchItems) {
                int iPos = foundText.ToLower().IndexOf(searchItem.ToLower());
                string strTmpResult = foundText.Substring(iPos, 1);
                string strLetter = "";
                iPos++;
                while (iPos < foundText.Length && strLetter != " ") {
                    strLetter = foundText.Substring(iPos, 1);
                    strTmpResult += strLetter;
                    iPos++;
                }
                if (strResult.Length > 0) {
                    strResult += " ";
                }

                strResult += strTmpResult.Trim();
            }

            return strResult;
        }

        private string GetShortResultTextNoSpace(string foundText, string searchText) {
            int iStart = foundText.ToLower().IndexOf(searchText.ToLower());
            int iStartResult = iStart;
            if (iStart < 0) {
                return foundText;
            }

            if(iStart > 0) {
                string strStartPart = foundText.Substring(0, iStart);
                int iLetterIndex = iStart - 1;
                string strLetter = foundText.Substring(iLetterIndex, 1);
                while (strLetter != " " && iLetterIndex > 0) {
                    iLetterIndex--;
                    strLetter = foundText.Substring(iLetterIndex, 1);
                }

                iStartResult = iLetterIndex;
            }
            

            string strResult = "";
            int iNextWhiteSpace = foundText.IndexOf(" ", iStart + 1);
            if (iNextWhiteSpace > 0) {
                strResult = foundText.Substring(iStartResult, iNextWhiteSpace - iStartResult);
            } else { 
                strResult = foundText.Substring(iStartResult);
            }

            return strResult.Trim();
        }

        private string GetResultFoundText(string foundText) {
            string resFoundText = foundText.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            return resFoundText;
        }

        private string GetResultFoundFullText(string foundText) {
            if (String.IsNullOrEmpty(foundText)) {
                return null;
            }

            string strResultText = foundText;
            strResultText = foundText.Replace("\r\n\r\n", "\r\n");//.Replace("\n", "<br />").Replace("\r", " ");
            while (strResultText.EndsWith("\r\n")) {
                strResultText = strResultText.Substring(0, strResultText.Length - 2);
            }
            strResultText = strResultText.Replace("\r\n", "<br />");//.Replace("\n", "<br />").Replace("\r", " ");

            //string strResultText = foundText.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", " ");

            //return resFoundText;
            //string strResult = foundText;
            //while(strResult.EndsWith("\r\n")) {
            //    strResult = strResult.Substring(0, strResult.Length - 2);
            //}

            return strResultText.Trim();
        }
        #endregion
    }
}