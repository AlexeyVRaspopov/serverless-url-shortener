#load "../UrlIngest/models.csx"
#r "Microsoft.WindowsAzure.Storage"
#r "System.Runtime.Serialization"

using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.ApplicationInsights;
using System.Net;
using System;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;


public static TelemetryClient telemetry = new TelemetryClient()
{
    InstrumentationKey = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY")
};

public static readonly string FALLBACK_URL = System.Environment.GetEnvironmentVariable("FALLBACK_URL");

public static HttpResponseMessage Run(HttpRequestMessage req, CloudTable inputTable, IEnumerable<dynamic> inputDocument, string shortUrl, TraceWriter log)
{
    int totalDocuments = inputDocument.Count();
    log.Info($"Found {totalDocuments} documents");
    var longLink = "";
     foreach (var item in inputDocument)
        {
            log.Info("Found "+ item.Url);
            longLink = item.Url;
        }
    
    log.Info($"C# HTTP trigger function processed a request for longUrl {longLink}");

    var redirectUrl = FALLBACK_URL;

    if (!String.IsNullOrWhiteSpace(shortUrl))
    {
        if (longLink != "")
        {
            log.Info($"Found it: {longLink}");
            redirectUrl = WebUtility.UrlDecode(longLink);
        }
    }
    else 
    {
    }
    
    var res = req.CreateResponse(HttpStatusCode.Redirect);
    res.Headers.Add("Location", redirectUrl);
    return res;
}
