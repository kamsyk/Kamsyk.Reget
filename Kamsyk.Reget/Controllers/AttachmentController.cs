using Kamsyk.Reget.Content.Sessions;
using Kamsyk.Reget.Controllers.Interface;
using Kamsyk.Reget.Controllers.RegetExceptions;
using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.ExtendedModel;
using Kamsyk.Reget.Model.ExtendedModel.Attachment;
using Kamsyk.Reget.Model.ExtendedModel.HttpResult;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static Kamsyk.Reget.Model.Repositories.RequestRepository;

namespace Kamsyk.Reget.Controllers {
    public class AttachmentController : BaseController {

        #region Properties

        #endregion

        #region Get, Post Methods
        [HttpPost]
        public ActionResult UploadAttachment(HttpPostedFileBase file) {
            //if (!IsUserInOfficeRole(UserRole.OfficeAdministrator, officeId)) {
            //    throw new ExNotAuthorizedUpdateUser("Not authorized to update Address");
            //}

            AttachmentExtend attUpload = new AttachmentExtend();
            if (file.ContentLength > 0) {
                var fileName = Path.GetFileName(file.FileName);

                byte[] fileContent = null;
                int fileSizeBytes = Request.Files[0].ContentLength;
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream)) {
                    fileContent = binaryReader.ReadBytes(fileSizeBytes);
                }

                var dKb = Math.Round(ConvertData.ToKiloBytes(fileSizeBytes), 0);
                attUpload.id = new AttachmentRepository().AddAttachment(fileName, fileContent, (decimal)dKb, CurrentUser.ParticipantId);
                attUpload.file_name = fileName;
                
                if (dKb == 0) dKb = 1;
                attUpload.size_kb = dKb;
                attUpload.icon_url = GetFileIconUrl(fileName, GetRootUrl());
                //var path = Path.Combine(@"c:\temp\upload", fileName);
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                //file.SaveAs(path);
            }
                       
            return GetJson(attUpload);
        }

        [HttpGet]
        public ActionResult GetAttachment(int id) {
            var att = new AttachmentRepository().GetAttachmentById(id);
            MemoryStream outputStream = new MemoryStream(att.file_content);
            var mimeType = MimeMapping.GetMimeMapping(att.file_name);
            return File(outputStream, mimeType, att.file_name);
            //return File(outputStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Centres" + GetTimeStampFileSuffix() + ".xlsx");
        }

        [HttpPost]
        public ActionResult DeleteAttachment(int id) {
            HttpResult httpResult = new HttpResult();

            //Is User Author?
            var attLight = new AttachmentRepository().GetAttachmentLightById(id);
            if (CurrentUser.ParticipantId != attLight.modify_user_id) {
                httpResult.error_id = HttpResult.NOT_AUTHORIZED_ERROR;
                return GetJson(httpResult);
                //throw new ExNotAuthorizedUpdateUser("Not authorized to delete Attachment");
            }

            //Is Request in acceptable status?
            bool isAllowed = false;
            if (attLight.Request_Event == null || (attLight.Request_Event.Count == 0)) {
                isAllowed = true;
            } else {

                var requestEvent = new RequestRepository().GetRequestEventById(attLight.Request_Event.ElementAt(0).id);
                if (requestEvent.request_status == (int)RequestStatus.Draft
                    && requestEvent.requestor == CurrentUser.ParticipantId) {
                    isAllowed = true;
                } else if (RequestController.IsWaitForUserApproval(requestEvent, CurrentUser.ParticipantId)) {
                    isAllowed = true;
                } else if ((requestEvent.request_status == (int)RequestStatus.Approved
                    || requestEvent.request_status == (int)RequestStatus.Ordered)
                    && requestEvent.orderer_id == CurrentUser.ParticipantId) {
                    isAllowed = true;
                }
            }
            

            if (!isAllowed) {
                httpResult.error_id = HttpResult.NOT_AUTHORIZED_ERROR;
                return GetJson(httpResult);
            }

            
            //httpResult.error_id = 0;
            return GetJson(new HttpResult());
        }

        //[HttpPost]
        //public ActionResult Upload() {
        //    try {


        //        return GetJson(new HttpResult());
        //    } catch (Exception ex) {
        //        HandleError(ex);
        //        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return Content(ex.Message, MediaTypeNames.Text.Plain);

        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AsynFileUpload(IEnumerable<HttpPostedFileBase> files) {
        //    int count = 0;
        //    if (files != null) {
        //        foreach (var file in files) {
        //            if (file != null && file.ContentLength > 0) {
        //                //var path = Path.Combine(Server.MapPath("~/UploadedFiles"), file.FileName);
        //                //file.SaveAs(path);
        //                count++;
        //            }
        //        }
        //    }
        //    return new JsonResult {
        //        Data = "Successfully " + count + " file(s) uploaded"
        //    };
        //}
        #endregion

        #region Methods
        public static string GetFileIconUrl(string fileName, string rootUrl) {
            if (String.IsNullOrEmpty(fileName)) {
                return rootUrl + "Content/Images/FileIcon/Unknown.png";
            }

            string[] fileParts = fileName.Split('.');
            string extention = fileParts[fileParts.Length - 1].ToUpper();
            switch (extention) {
                case "JPG":
                case "JPEG":
                case "BMP":
                case "GIF":
                case "PNG":
                    return rootUrl + "Content/Images/FileIcon/Img.png";
                case "DOC":
                case "DOT":
                case "DOCX":
                case "DOCM":
                case "DOCT":
                    return rootUrl + "Content/Images/FileIcon/Doc.png";
                case "ZIP":
                    return rootUrl + "Content/Images/FileIcon/Zip.png";
                case "PDF":
                    return rootUrl + "Content/Images/FileIcon/Pdf.png";
                case "PPT":
                case "PPTX":
                    return rootUrl + "Content/Images/FileIcon/Ppt.png";
            }

            return rootUrl + "Content/Images/FileIcon/Unknown.png";
        }
        #endregion
    }
}