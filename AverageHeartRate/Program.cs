using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AverageHeartRate
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            HttpClient httpClient = new();
            DateTime startDate = new(2021, 11, 30, 00, 00, 00, DateTimeKind.Local);
            startDate = startDate.AddDays(-31);
            string myToken = "";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", myToken);
            string start_date;
            string end_date;
            List<int> samples = new();
            DateTime endDate = DateTime.Now;
            double numberDaysBtEndAndStop = 30;
            do
            {
                startDate = startDate.AddDays(numberDaysBtEndAndStop + 1);
                start_date = startDate.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz");
                end_date = startDate.AddDays(numberDaysBtEndAndStop).ToString("yyyy-MM-ddTHH\\:mm\\:sszzz");
                string url = $"https://api.ouraring.com/v2/usercollection/heartrate?start_datetime={start_date}&end_datetime={end_date}";
                url = url.Replace("+", "-");
                //var response = await httpClient.GetFromJsonAsync<Data>($"https://api.ouraring.com/v2/usercollection/heartrate?start_datetime=2021-12-01T00:00:00-00:00&end_datetime=2021-12-02T00:00:00-00:00");
                Console.WriteLine("sending request"); 
                var response = await httpClient.GetFromJsonAsync<Data>(url);
                if (response.data.Count > 0)
                {
                    foreach (var item in response.data)
                    {
                        if (item.bpm > 0)
                        {
                            samples.Add(item.bpm);
                        }

                    }

                }
            } while (startDate < endDate);


            double totalAverage = 0;
            foreach (var item in samples)
            {
                totalAverage += item;
            }

            Console.WriteLine($"Kurt's average heart rate over {samples.Count} while awake is {totalAverage/samples.Count}");
            Console.WriteLine("Samples taken between 11-30-2021 and today.");
            
            Console.ReadLine();

        }
    }
}
