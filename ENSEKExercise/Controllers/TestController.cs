using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CsvHelper;
using System.IO;

namespace ENSEKExercise.Controllers
{
    public class TestController : ApiController
    {
		[HttpPost]
		[Route("api/test/meter-reading-uploads")]
		public HttpResponseMessage Post()
		{
			string uploadResult = "";
			HttpResponseMessage result = null;
			var httpRequest = HttpContext.Current.Request;
			if (httpRequest.Files.Count > 0)
			{
				foreach (string file in httpRequest.Files)
				{
					var postedFile = httpRequest.Files[file];
					FileInfo fileDetails = new FileInfo(postedFile.FileName);
					if (fileDetails.Extension != ".csv")
					{
						uploadResult = "Failed to upload. File is not a CSV.";
						result = Request.CreateResponse(HttpStatusCode.BadRequest, uploadResult);
						return result;
					}
					var m = new Models.MeterReading();
					uploadResult = m.ValidateCSVFile(postedFile);
				}
				result = Request.CreateResponse(HttpStatusCode.Created, uploadResult);
			}
			else
			{
				result = Request.CreateResponse(HttpStatusCode.BadRequest, uploadResult);
			}
			return result;
		}
    }
}
