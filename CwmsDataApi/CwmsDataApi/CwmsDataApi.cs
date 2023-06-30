using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Usace.Cwms
{
  public class CwmsDataApi
  {

    //const string API_BASE = "https://cwms-data.usace.army.mil/cwms-data/";
    const string API_BASE = "https://cwms-data-test.cwbi.us/cwms-data/";

    public CwmsDataApi()
    {

    }

    private async Task<string> GetJsonFromWeb(string path = "timeseries", string query = "")
    {
      string apiUrlWithQuery =API_BASE + path + query;
      Console.WriteLine(apiUrlWithQuery);

      string rval = "";
      using (HttpClient client = new HttpClient())
      {
        client.DefaultRequestHeaders.Accept.Clear();
       // var acceptHeader = new MediaTypeWithQualityHeaderValue("application/json");
        //acceptHeader.Parameters.Add(new NameValueWithParametersHeaderValue("version","2"));
        //client.DefaultRequestHeaders.Accept.Add(acceptHeader);
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json;version=2");
        

        Console.WriteLine("DefaultRequestVersion :"+client.DefaultRequestVersion);
        Console.WriteLine(client.DefaultRequestHeaders);

        HttpResponseMessage response = await client.GetAsync(apiUrlWithQuery, HttpCompletionOption.ResponseContentRead);
        
        rval = await response.Content.ReadAsStringAsync();
        Console.WriteLine(rval);

      }
      return rval;
    }

    static string FormatDate(DateTime t)
    {
     return t.ToString("O");
    }
    public async Task<List<(DateTime, double)>> GetTimeSeries(string name, string office, DateTime begin, DateTime end)
    {
      string queryString = $"?name={Uri.EscapeDataString(name)}&office={Uri.EscapeDataString(office)}&"
                        + $"begin={Uri.EscapeDataString(FormatDate(begin))}&end={Uri.EscapeDataString(FormatDate(end))}";

      string json = await GetJsonFromWeb("timeseries", queryString);

      var doc = JsonDocument.Parse(json);
      var root = doc.RootElement;
      var ts = root.GetProperty("time-series").GetProperty("time-series");
      if (ts.GetArrayLength() != 1)
      {
        throw new Exception("array length was " + ts.GetArrayLength() + " expected 1");
      }
      ts = ts[0];
      List<(DateTime, double)> data = new List<(DateTime, double)>();
      if (ts.TryGetProperty("regular-interval-values", out JsonElement rtsv))
      {
        var interval = rtsv.GetProperty("interval").GetString();
        var span = System.Xml.XmlConvert.ToTimeSpan(interval);
        Console.WriteLine(span.Minutes);
      }
      return data;
    }
  }
}