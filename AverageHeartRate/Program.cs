using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AverageHeartRate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            HttpClient httpClient = new();
            DateTime startDate = new(2021, 11, 30, 00, 00, 00, DateTimeKind.Local);
            startDate = startDate.AddDays(-31);
            string myToken = Environment.GetEnvironmentVariable("OURA_TOKEN", EnvironmentVariableTarget.User);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", myToken);
            string start_date;
            string end_date;
            //List<int> samples = new();
            DateTime endDate = DateTime.Now;
            double numberDaysBtEndAndStop = 30;
            Dictionary<string, string> dates = new();
            do
            {
                startDate = startDate.AddDays(numberDaysBtEndAndStop).AddTicks(1);
                start_date = startDate.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz");
                end_date = startDate.AddDays(numberDaysBtEndAndStop).ToString("yyyy-MM-ddTHH\\:mm\\:sszzz");
                dates.Add(start_date, end_date);
            } while (DateTime.Parse(end_date) < endDate);
            ConcurrentBag<int> samples = new();
            Parallel.ForEach(dates, (date) =>
            {
                string url = $"https://api.ouraring.com/v2/usercollection/heartrate?start_datetime={date.Key}&end_datetime={date.Value}";
                url = url.Replace("+", "-");
                Uri uri = new(url);
                Console.WriteLine("sending request");
                HttpRequestMessage request = new() { RequestUri = uri };
                var response = httpClient.Send(request); //.GetFromJson<Data>(url);
                response.EnsureSuccessStatusCode();
                var stream = response.Content.ReadAsStream();
                var streamReader = new StreamReader(stream, Encoding.UTF8);
                string content = streamReader.ReadToEnd();
                Data contentData = JsonSerializer.Deserialize<Data>(content);
                Console.WriteLine(contentData.data.Count + "   " + date.Key + "   " + date.Value);
                if (contentData.data.Count > 0)
                {
                    foreach (var item in contentData.data)
                    {
                        if (item.bpm > 0)
                        {
                            samples.Add(item.bpm);
                        }

                    }

                }
            });


            double totalAverage = 0;
            foreach (var item in samples)
            {
                totalAverage += item;
            }

            Console.WriteLine($"Kurt's average heart rate over {samples.Count} while awake is {totalAverage / samples.Count}");
            Console.WriteLine("Samples taken between 11-30-2021 and today.");

            Console.ReadLine();

        }
    }
}
