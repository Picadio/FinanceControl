using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Utils;

public static class HttpUtil
{
    public const string BaseApiTaxGovUa = "https://cabinet.tax.gov.ua/ws/api_public/rro/chkAllWeb";

    public static Uri GetUrlApiTaxGovUa(double sum, DateTime dateTime, string fn, string id)
    {
        return new Uri(BaseApiTaxGovUa + $"?fn={fn}&sm={sum.ToString("F2", CultureInfo.InvariantCulture)}&date={dateTime:yyyy-MM-dd HH:mm:ss}&id={id}&type=3");
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