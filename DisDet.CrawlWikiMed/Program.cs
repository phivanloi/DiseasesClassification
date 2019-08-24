using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HtmlAgilityPack;
using System.IO;
using DisDet.Core;

namespace DisDet.CrawlWikiMed
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var listUrl = "https://api.wikimed.vn/api/search/fulltextsearch/{0}/100/{1}/1/false";
            var detailsUrl = "https://api.wikimed.vn/api/conditions/{0}";
            var keywords = "abcdefghijklmnopqrstuvwxyz0123456789";
            var diseasesNameAndSymptom = new Dictionary<string, string>();

            "Start crawl.".WriteConsole(ConsoleColor.Green);

            foreach (var keywork in keywords.ToCharArray())
            {
                var page = 1;
                while (true)
                {
                    var getListUrl = string.Format(listUrl, page, keywork);
                    var webListContent = Utility.GetWebContent(getListUrl);
                    if (string.IsNullOrEmpty(webListContent))
                    {
                        break;
                    }
                    var diseases = Utility.SafeJsonPaser<List<DiseasesDes>>(webListContent);
                    if (diseases?.Count <= 0)
                    {
                        break;
                    }

                    foreach (var diseas in diseases)
                    {
                        var getDetailsUrl = string.Format(detailsUrl, diseas.Code);
                        var detailsContent = Utility.GetWebContent(getDetailsUrl);
                        if (string.IsNullOrEmpty(detailsContent))
                        {
                            break;
                        }
                        var diseasesDetails = Utility.SafeJsonPaser<DiseasesDetails>(detailsContent);
                        if (diseasesDetails != null || !string.IsNullOrEmpty(diseasesDetails.Name) || diseasesDetails.Fields?.Count() >= 0)
                        {
                            var contentField = diseasesDetails.Fields.FirstOrDefault(q => q.UniqueKey == 1);
                            if (contentField != null && !string.IsNullOrEmpty(contentField.Content) && contentField.Content.Contains("TRIỆU CHỨNG"))
                            {
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(contentField.Content);
                                var symptom = htmlDocument.DocumentNode.SelectNodes("//p[2]")?.FirstOrDefault()?.InnerText;
                                if (!string.IsNullOrEmpty(symptom) && !diseasesNameAndSymptom.ContainsKey(diseasesDetails.Name))
                                {
                                    $"Add: {diseasesDetails.Name}, to list data".WriteConsole(ConsoleColor.Green);
                                    diseasesNameAndSymptom.Add(diseasesDetails.Name, symptom);
                                }
                            }
                        }
                    }
                    page++;
                }
            }

            $"Crawl {diseasesNameAndSymptom.Count} diseases".WriteConsole(ConsoleColor.Green);

            $"Start write file".WriteConsole(ConsoleColor.Green);
            File.WriteAllLines("training.txt", diseasesNameAndSymptom.Select(q => $"{q.Value}|{q.Key}"), Encoding.UTF8);

            "Write file Done!".WriteConsole(ConsoleColor.Green);
        }
    }
}
