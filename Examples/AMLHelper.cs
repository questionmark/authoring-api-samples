using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Examples
{
    /// <summary>
    /// Helper class to parse AML
    /// </summary>
    public static class AMLHelper
    {
        public static void GetTopicsAndQuestionsFromAML(string aml, out List<int> topics, out List<int> topicsWithSubTopics, out List<long> questions)
        {
            var xmlElement = XElement.Parse(aml);

            var questionBlocks = xmlElement.Elements("BLOCK").Where(e => e.Element("BLOCK_TYPE").Value == "Question");

            questions = new List<long>();
            topics = new List<int>();
            topicsWithSubTopics = new List<int>();

            foreach (var block in questionBlocks)
            {
                foreach(var questionItem in block.Element("QuestionBlock").Elements("QUESTION_ITEM"))
                {
                    var includeSubTopics = questionItem.Attribute("INCLUDE_SUBTOPICS").Value.ToLower() == "yes";
                    var blockType = questionItem.Element("METHOD").Value;

                    switch (blockType)
                    {
                        case "SingleTopic":
                        case "RandomQuestions":
                            var topicId = int.Parse(questionItem.Element("TOPIC_ID").Value);
                            if (includeSubTopics)
                            {
                                if (!topicsWithSubTopics.Contains(topicId))
                                    topicsWithSubTopics.Add(topicId);
                            }
                            else
                            {
                                if (!topics.Contains(topicId))
                                    topics.Add(topicId);
                            }
                            break;
                        case "SingleQuestion":
                            var questionId = long.Parse(questionItem.Element("QUESTION_ID").Value);

                            if (!questions.Contains(questionId))
                                questions.Add(questionId);
                            break;
                        default:
                            throw new NotSupportedException($"The question block method {blockType} is not supported");
                    }
                }                
            }
        }
    }
}
