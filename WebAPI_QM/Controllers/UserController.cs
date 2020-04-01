using Models.UniversalModels;
using Service.UniversalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebAPI_QM.Controllers
{
    public class UserController : ApiController
    {
        [Route("Login")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Login(dynamic Account)
        {
            var v = HttpContext.Current.Request;
            int API = 1;
            if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(Convert.ToString(Account.LoginID), API))
            {
                string token = UniversalServiceBase.Login(Convert.ToString(Account.LoginID), Convert.ToString(Account.Password));
                return Json<dynamic>(new { token });
            }

            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//1

        [System.Web.Http.HttpGet]
        public string GetApplicantName(string ApplicantID)
        {
            string ApplicantName = UniversalServiceBase.GetApplicantName(ApplicantID);
            return ApplicantName;
        }
    }
}