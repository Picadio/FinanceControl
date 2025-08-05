using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Utils;

public static class HttpUtil
{
    public const string BaseApiTaxGovUa = "https://cabinet.tax.gov.ua/ws/api_public/rro/chkAllWeb";

    public static Uri GetUrlApiTaxGovUa(double sum, DateTime dateTime, string fn, string id)
    {
        var date = dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        
        var dateParam = date.Replace(" ", "%20");
        
        var url = $"{BaseApiTaxGovUa}?fn={fn}&sm={sum.ToString("F2", CultureInfo.InvariantCulture)}&date={dateParam}&id={id}&type=1";

        return new Uri(url);
    }
    
    public static IActionResult GetResponse(HttpStatusCode code, object? data = null)
    {
        return new ObjectResult(data)
        {
            StatusCode = (int)code
        };
    }
    
    public static IActionResult SuccessResponse(object? data = null)
    {
        return new ObjectResult(data)
        {
            StatusCode = (int)HttpStatusCode.OK
        };
    }
    
    
}