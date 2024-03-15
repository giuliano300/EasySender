using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Utility")]
    public class UtilityController : ApiController
    {

        [HttpGet]
        [Route("GetByteArray")]

        public byte[] GetByteArray(string pathUrl)
        {
            try
            {
                var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
                var filePath = HostingEnvironment.MapPath("/public/" + fileName);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(pathUrl, filePath);
                }

                return File.ReadAllBytes(filePath);
            }
            catch (Exception e) { }
            return null;
        }
    }
}
