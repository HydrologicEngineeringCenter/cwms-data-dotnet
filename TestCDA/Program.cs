// See https://aka.ms/new-console-template for more information


var api = new Usace.Cwms.CwmsDataApi();

string name = "Mount Morris.Elev.Inst.30Minutes.0.GOES-NGVD29-Rev";
string office = "LRB";
var begin = DateTime.Parse("2023-06-23T06:01:00");
var end = DateTime.Parse("2023 -06-24T06:01:00");
var ts = await api.GetTimeSeries(name, office, begin, end);
Console.WriteLine(ts);