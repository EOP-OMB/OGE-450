using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

using OGC.Data.SharePoint.Models;

namespace OGC.Form450.API.Controllers
{
    [Form450Authorize("requireAuthorization")]
    public class OGEForm450Controller : BaseController
    {
 
        public const int MIN_ASSETS = 5;
        public const int MIN_LIABILITIES = 2;
        public const int MIN_POSITIONS = 6;
        public const int MIN_AGREEMENTS = 4;
        public const int MIN_GIFTS = 3;

        [HttpGet]
        public IHttpActionResult Get()
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            OGE450User = UserInfo.GetUser(identity);
            List<OGEForm450> list;

            if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                list = OGEForm450.GetAll().OrderByDescending(x => x.Year).ThenBy(x => x.FormStatus).ToList();
            else
                list = GetMyForms(OGE450User);

            return Json(list, CamelCase);
        }

        [HttpGet]
        public IHttpActionResult Get(string a)
        {
            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            OGE450User = UserInfo.GetUser(identity);
            List<OGEForm450> list;

            try
            {
                switch (a)
                {
                    case "myforms":
                        list = GetMyForms(OGE450User);
                        break;
                    case "certifyblank":
                        if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                        {
                            list = OGEForm450.CertifyBlankForms(OGE450User);
                            break;
                        }
                        else
                            return Unauthorized();
                    case "certifyunchanged":
                        if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                        {
                            list = OGEForm450.CertifyUnchangedForms(OGE450User);
                            break;
                        }
                        else
                            return Unauthorized();
                    case "reviewer":
                        if (OGE450User.IsAdmin || OGE450User.IsReviewer)
                        {
                            list = GetReviewerForms(OGE450User);
                            break;
                        }
                        else
                            return Unauthorized();
                    default:
                        return BadRequest("No such action.");
                }

                return Json(list, CamelCase);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

        }

        private List<OGEForm450> GetMyForms(UserInfo user)
        {
            var list = OGEForm450.GetAllByUser("Filer", user.Id).OrderByDescending(x => x.DueDate).ToList();

            return list;
        }

        private List<OGEForm450> GetReviewerForms(UserInfo user)
        {
            List<OGEForm450> list;

            list = OGEForm450.GetAllReviewable();

            return list;
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                var form = OGEForm450.Get(id);

                if (form != null)
                {
                    // Return unauthorized if user is not admin or reviewer and trying to access someone elses filing
                    if (!OGE450User.IsAdmin && !OGE450User.IsReviewer && form.Filer != OGE450User.Upn)
                        return Unauthorized();

                    SetReportableInformation(form);

                    return Json(form, CamelCase);
                }
                else
                    return BadRequest("Form not found.");
                
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(OGEForm450 item)
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                OGE450User = UserInfo.GetUser(identity);

                // Return unauthorized if user is not admin or reviewer and trying to update someone elses filing
                if (!OGE450User.IsAdmin && !OGE450User.IsReviewer && item.Filer != OGE450User.Upn)
                    return Unauthorized();

                item = ClearEmptyReportableInformation(item);
                var oldItem = OGEForm450.Get(item.Id);

                var filer = UserInfo.GetUser(item.Filer);
                item.AppUser = OGE450User.DisplayName;
                item.CorrelationId = Guid.NewGuid().ToString();

                if (item.ReportableInformationList != null)
                {
                    item.ReportableInformationList.ForEach(x => x.CorrelationId = item.CorrelationId);
                    item.ReportableInformationList.ForEach(x => x.AppUser = item.AppUser);
                }

                item.RunBusinessRules(OGE450User, filer, oldItem);

                if (item.ReportableInformationList != null)
                    item.SaveReportableInformation();

                var form = item.Save();

                if (form.FormStatus == Constants.FormStatus.CERTIFIED)
                {
                    Employee.FormCertified(form);

                    var extensions = ExtensionRequest.GetPendingExtensions(form.Id);

                    foreach (ExtensionRequest ext in extensions)
                    {
                        // If there are any pending requests for this form, cancel them.
                        ext.Status = Constants.ExtensionStatus.CANCELED;
                        ext.Save();
                    }
                }

                // wait until after Save to process emails, if an error occurs it will be caught and the emails will not get processed.
                item.ProcessEmails();

                SetReportableInformation(form);

                return Json(form, CamelCase);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private void SetReportableInformation(OGEForm450 form)
        {
            var count = form.ReportableInformationList.Count(x => x.InfoType == InfoType.AssetsAndIncome);
            for (int i = count; i < MIN_ASSETS; i++)
                form.ReportableInformationList.Add(new ReportableInformation() { InfoType = InfoType.AssetsAndIncome, Name = "", NoLongerHeld = false });

            count = form.ReportableInformationList.Count(x => x.InfoType == InfoType.Liabilities);
            for (int i = count; i < MIN_LIABILITIES; i++)
                form.ReportableInformationList.Add(new ReportableInformation() { InfoType = InfoType.Liabilities, Name = "", Description = "" });

            count = form.ReportableInformationList.Count(x => x.InfoType == InfoType.OutsidePositions);
            for (int i = count; i < MIN_POSITIONS; i++)
                form.ReportableInformationList.Add(new ReportableInformation() { InfoType = InfoType.OutsidePositions, Name = "", Description = "", AdditionalInfo = "", NoLongerHeld = false });

            count = form.ReportableInformationList.Count(x => x.InfoType == InfoType.AgreementsOrArrangements);
            for (int i = count; i < MIN_AGREEMENTS; i++)
                form.ReportableInformationList.Add(new ReportableInformation() { InfoType = InfoType.AgreementsOrArrangements, Name = "", AdditionalInfo = "" });

            count = form.ReportableInformationList.Count(x => x.InfoType == InfoType.GiftsOrTravelReimbursements);
            for (int i = count; i < MIN_GIFTS; i++)
                form.ReportableInformationList.Add(new ReportableInformation() { InfoType = InfoType.GiftsOrTravelReimbursements, Name = "", AdditionalInfo = "" });
        }

        private OGEForm450 ClearEmptyReportableInformation(OGEForm450 form)
        {
            if (form.ReportableInformationList != null)
                form.ReportableInformationList.RemoveAll(x => x.Id == 0 && x.IsEmpty());

            return form;
        }
    }
}
