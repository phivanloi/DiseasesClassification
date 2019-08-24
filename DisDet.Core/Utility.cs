using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DisDet.Core
{
    public static class Utility
    {
        /// <summary>
        /// Ghi log ra màng hình console và log vào file
        /// log file theo ngày
        /// </summary>
        /// <param name="message">Nội dung log</param>
        /// <param name="color">Màu sắc ở màn hình console</param>
        public static void WriteConsole(this string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Lấy nội dung html của web qua url
        /// </summary>
        /// <param name="url">Url cần lấy</param>
        /// <returns>string</returns>
        public static string GetWebContent(string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Ai Crawler");
                    HttpResponseMessage httpResponseMessage = httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result;
                    return Encoding.UTF8.GetString(httpResponseMessage.Content.ReadAsByteArrayAsync().Result);
                }
            }
            catch (Exception ex)
            {
                ex.ToString().WriteConsole(ConsoleColor.Red);
                return string.Empty;
            }
        }

        /// <summary>
        /// Safe paser json to object
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="content">Json content</param>
        /// <returns>T</returns>
        public static T SafeJsonPaser<T>(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                ex.ToString().WriteConsole(ConsoleColor.Red);
                return default;
            }
        }

        public static float CalculatePercentage(double value)
        {
            return 100 * (1.0f / (1.0f + (float)Math.Exp(-value)));
        }
    }
}
