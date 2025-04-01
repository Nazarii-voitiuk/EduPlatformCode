using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ExCSS;
using Jint;
using System;
using System.Linq;
using System.Text;

namespace EduCodePlatform.Services
{
    public class CodeCheckService
    {
        // ======= (1) Перевірки за TaskTestCase (HTML-rules, CSS-rules, JS output) =======
        public bool CheckHtml(string userHtml, string htmlRules)
        {
            if (string.IsNullOrWhiteSpace(userHtml)) return false;

            // Приклад: “require-h1”
            if (!string.IsNullOrEmpty(htmlRules) && htmlRules.Contains("require-h1"))
            {
                var parser = new HtmlParser();
                var doc = parser.ParseDocument(userHtml);
                var h1Count = doc.QuerySelectorAll("h1").Length;
                if (h1Count < 1) return false;
            }

            // Можна додати інші правила: “require-img”, “require-ul”, ...
            return true;
        }

        public bool CheckCss(string userCss, string cssRules)
        {
            if (string.IsNullOrWhiteSpace(userCss)) return false;

            // Приклад: “require-body-background”
            if (!string.IsNullOrEmpty(cssRules) && cssRules.Contains("require-body-background"))
            {
                var parser = new StylesheetParser();
                var styleSheet = parser.Parse(userCss);

                bool foundBodyBg = false;
                foreach (var rule in styleSheet.StyleRules)
                {
                    if (rule is StyleRule styleRule && styleRule.SelectorText == "body")
                    {
                        var backgroundVal = styleRule.Style.GetPropertyValue("background");
                        if (!string.IsNullOrEmpty(backgroundVal))
                        {
                            foundBodyBg = true;
                            break;
                        }
                    }
                }
                if (!foundBodyBg) return false;
            }

            // Інші перевірки...
            return true;
        }

        public string RunJsWithJint(string userJs, string inputData, int timeLimitSec)
        {
            var sb = new StringBuilder();

            var engine = new Engine(cfg => {
                cfg.TimeoutInterval(TimeSpan.FromSeconds(timeLimitSec));
                cfg.Strict();
            });

            engine.SetValue("console", new
            {
                log = new Action<object>(msg => sb.Append(msg))
            });

            engine.SetValue("input", inputData ?? "");
            engine.Execute(userJs);

            return sb.ToString();
        }

        // ======= (2) Порівняння “кінцевого рендерингу” (DOM) user vs reference) =======
        /// <summary>
        /// Порівнює DOM-дерево userHtml/Css/Js із referenceHtml/Css/Js (без реального виконання JS).
        /// Ускладнене порівняння: 
        ///   - Кількість елементів
        ///   - Теги у порядку обходу 
        ///   - (за бажанням) текст елементів
        /// Повертає true, якщо структура досить схожа.
        /// </summary>
        public bool CompareRenderedOutput(
            string userHtml, string userCss, string userJs,
            string refHtml, string refCss, string refJs)
        {
            // (A) Будуємо повний HTML:
            var userFull = BuildFullHtml(userHtml, userCss, userJs);
            var refFull = BuildFullHtml(refHtml, refCss, refJs);

            // (B) Парсимо через AngleSharp
            var parser = new HtmlParser();

            // Увага: JS не виконається, DOM залишиться “як є”
            var userDoc = parser.ParseDocument(userFull);
            var refDoc = parser.ParseDocument(refFull);

            // (C) Перевірка 1: загальна кількість елементів
            var userAll = userDoc.QuerySelectorAll("*").ToList();
            var refAll = refDoc.QuerySelectorAll("*").ToList();
            // Якщо кількість дуже відрізняється, вважаємо “не схоже”
            double ratio = userAll.Count / (double)Math.Max(refAll.Count, 1);
            if (ratio < 0.7 || ratio > 1.3)
            {
                // Якщо розбіжність більше 30%
                return false;
            }

            // (D) Перевірка 2: послідовність тегів (у спрощеному DFS-порядку)
            // Візьмемо “userAll” і “refAll”, порівняємо, наприклад, 30% 
            // Умовно, якщо 70% тегів співпали 
            int matchedCount = 0;
            int compareLength = Math.Min(userAll.Count, refAll.Count);

            for (int i = 0; i < compareLength; i++)
            {
                var uTag = userAll[i].TagName;   // наприклад, “DIV”
                var rTag = refAll[i].TagName;    // “DIV”
                // якщо збіг — matchedCount++
                if (uTag.Equals(rTag, StringComparison.OrdinalIgnoreCase))
                {
                    matchedCount++;
                }
            }
            double matchedRatio = matchedCount / (double)compareLength;
            if (matchedRatio < 0.6)
            {
                // Менше 60% тегів співпали
                return false;
            }

            // (E) Перевірка 3: можна порівнювати тексти <h1>, <p>, ...
            // Тут — наприклад, кількість <h1> з ідентичними innerText
            var userH1s = userDoc.QuerySelectorAll("h1");
            var refH1s = refDoc.QuerySelectorAll("h1");
            if (userH1s.Length > 0 && refH1s.Length > 0)
            {
                // Співставимо перший h1
                var userText = userH1s[0].TextContent.Trim();
                var refText = refH1s[0].TextContent.Trim();
                if (!string.IsNullOrEmpty(refText))
                {
                    // Якщо різниця текстів > 30% символів, вважаємо fail
                    if (!IsStringSimilar(userText, refText, 0.7))
                    {
                        return false;
                    }
                }
            }

            // Якщо дійшли сюди => структура “досить схожа”
            return true;
        }

        // Допоміжний метод: будує повний HTML
        private string BuildFullHtml(string html, string css, string js)
        {
            return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'><style>{css ?? ""}</style></head>
<body>
  {html ?? ""}
  <script>{js ?? ""}</script>
</body>
</html>";
        }

        /// <summary>
        /// Спрощений метод: перевіряємо, що userText і refText
        /// мають коефіцієнт схожості >= threshold.
        /// Наприклад, 0.7 => 70% символів збігається (Levenshtein?).
        /// Для простоти — порівняємо довжину.
        /// </summary>
        private bool IsStringSimilar(string userText, string refText, double threshold)
        {
            // Для швидкості — простий підхід: порівнюємо min(Length) / max(Length).
            // Реальний алгоритм — Levenshtein distance.
            int minLen = Math.Min(userText.Length, refText.Length);
            int maxLen = Math.Max(userText.Length, refText.Length);
            if (maxLen == 0) return true; // порожні
            double ratio = minLen / (double)maxLen;
            return (ratio >= threshold);
        }
    }
}
