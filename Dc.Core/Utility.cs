using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Dc.Core
{
    public static class Utility
    {
        /// <summary>
        /// Write a log console with color
        /// </summary>
        /// <param name="message">Content to write log</param>
        /// <param name="color">Color</param>
        public static void WriteConsole(this string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Get a webpage content as string
        /// </summary>
        /// <param name="url">The url to get</param>
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

        /// <summary>
        /// Get folder path of execute file
        /// </summary>
        /// <param name="assemblyLocation">Assembly location of program</param>
        /// <param name="relativePath">folder name to mapp</param>
        /// <returns></returns>
        public static string GetAbsolutePath(string assemblyLocation,string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(assemblyLocation);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        /// <summary>
        /// check duplicate file name and edit it
        /// </summary>
        /// <param name="fullPath">Full path file name</param>
        /// <param name="newNameFormat">format edit if file duplicate</param>
        /// <returns>string</returns>
        public static string IdentityFileName(string fullPath, string newNameFormat = "{0}({1})")
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return string.Empty;
            }

            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format(newNameFormat, fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }
    }
}
