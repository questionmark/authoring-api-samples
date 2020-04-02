using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Examples
{
    /// <summary>
    /// Helper class to parse QML
    /// </summary>
    public static class QMLHelper
    {
        public static List<string> GetOutcomeContentsQML(string qml)
        {
            var xmlElement = XElement.Parse(qml);
            var result = new List<string>();
            
            foreach (var outcome in xmlElement?.Elements("OUTCOME"))
            {                
                var content = CleanCData(outcome?.Element("CONTENT")?.Value);
                result.Add(content);
            }

            return result;
        }
       
        public static string GetScoreQML(string qml)
        {
            var xmlElement = XElement.Parse(qml);
            var scoresFromAdd = "";
            var scores = 0;
            foreach (var outcome in xmlElement?.Elements("OUTCOME"))
            {
                var scoreString = outcome?.Attribute("ADD")?.Value;

                if(!string.IsNullOrWhiteSpace(scoreString))
                {
                    var value = int.Parse(scoreString);
                    var sign = (value >= 0 ? "+" : "-");
                    scoresFromAdd += ((value.ToString().StartsWith(sign) ? value.ToString() : (sign + value.ToString())) + " ");
                    continue;
                }

                scoreString = outcome?.Attribute("SCORE")?.Value ?? "0";
                scores = Math.Max(int.Parse(scoreString), scores);
            }
            scoresFromAdd = scoresFromAdd.TrimEnd(' ');

            var addSpace = !string.IsNullOrWhiteSpace(scoresFromAdd) && scores > 0;
            var scoreFromScores = scoresFromAdd == "" ? scores.ToString() : (scores == 0 ? "" : scores.ToString());

            return scoresFromAdd + (addSpace ? " " : "") + scoreFromScores;
        }

        public static string GetContentFromQML(string qml)
        {
            var xmlElement = XElement.Parse(qml);
            return CleanCData(xmlElement?.Element("CONTENT")?.Value);
        }
        
        public static string GetTopicNameFromQML(string qml) {
            var xmlElement = XElement.Parse(qml);
            return xmlElement?.Attribute("TOPIC")?.Value;
        }

        public static string GetDescriptionQML(string qml) {
            var xmlElement = XElement.Parse(qml);
            return xmlElement?.Attribute("DESCRIPTION")?.Value;
        }

        public static string GetAnswerQTypeQML(string qml) {
            var xmlElement = XElement.Parse(qml);
            return xmlElement?.Element("ANSWER")?.Attribute("QTYPE")?.Value;
        }

        public static string GetQuestionStatusQML(string qml) {
            var xmlElement = XElement.Parse(qml);
            return xmlElement?.Attribute("STATUS")?.Value;
        }
                
        public static IDictionary<string,string> GetTagsFromQML(string qml)
        {
            var xmlElement = XElement.Parse(qml);

            var tags = xmlElement?.Elements("TAG");

            var result = new Dictionary<string, string>();
            foreach(var tag in tags)
            {
                result.Add(tag.Attribute("NAME").Value.ToLower(), CleanCData(tag.Value));
            }

            return result;
        }

        private static string CleanCData(string value)
        {
            return value.Replace("<![CDATA[", "").Replace("]]>","");
        }
    }
}
