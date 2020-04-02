using System;
using Microsoft.Extensions.Configuration;
using Examples;
using System.Collections.Generic;
using QM.AuthoringApi.OData.Entity;
using System.Linq;
using System.Diagnostics;

namespace ConsoleApp1
{
    public class Program
    {
        private static int AssessmentRevisionId_of_Assessment_with_MultipleAssessmentQML;
        private static long QuestionId_of_Assessment_with_MultipleLanguages;
        private static string Topic_of_Assessment_with_MultipleLanguages;
        private static int AssessmentRevisionId_of_Assessment_with_Multiple_QuestionBlocks;
        private static string Language_of_Assessment_with_Multiple_QuestionBlocks;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            // Initialise connection configuration
            var urlTemplate = config.GetSection("AuthoringApi:UrlTemplate").Value;
            var host = config.GetSection("AuthoringApi:HostName").Value;
            var tenant = config.GetSection("AuthoringApi:Tenant").Value;
            var username = config.GetSection("AuthoringApi:UserName").Value;
            var password = config.GetSection("AuthoringApi:Password").Value;
            var profile = config.GetSection("ExamplesProfile").Value;
            
            InitialiseDefaultData(config);

            var baseUrl = urlTemplate.Replace("{hostname}", host).Replace("{tenant}", tenant);

            IAuthoringApiExamples examples;

            // Determine which set of examples to use
            if (profile == "UseHttpClient")
                examples = new HttpClientExamples(baseUrl, username, password);
            else
                examples = new ContainerExamples(baseUrl, username, password);

            // Start execution of the examples
            GetAllQMLForAssessment(examples);

            GetAssessmentNamesIteratingThroughEachPage(examples);

            GetAssessmentNamesFirstPage(examples);

            AssessmentAMLsWitTheGivenAssessmentRevisionId(examples);

            AssessmentRevisionWithGivenAssessmentRevisionIdAndLanguage(examples);

            QMLsAssociatedToQuestionId(examples);

            QMLsAssociatedToTopicName(examples);

            Console.ReadKey();
        }

        private static void InitialiseDefaultData(IConfiguration config)
        {
            // Initialize default Data
            AssessmentRevisionId_of_Assessment_with_MultipleAssessmentQML =
                int.Parse(config.GetSection("Data:AssessmentRevisionId_of_Assessment_with_MultipleAssessmentQML").Value);
            QuestionId_of_Assessment_with_MultipleLanguages =
                long.Parse(config.GetSection("Data:QuestionId_of_Assessment_with_MultipleLanguages").Value);
            Topic_of_Assessment_with_MultipleLanguages =
                config.GetSection("Data:Topic_of_Assessment_with_MultipleLanguages").Value;
            AssessmentRevisionId_of_Assessment_with_Multiple_QuestionBlocks =
                int.Parse(config.GetSection("Data:AssessmentRevisionId_of_Assessment_with_Multiple_QuestionBlocks").Value);
            Language_of_Assessment_with_Multiple_QuestionBlocks =
                config.GetSection("Data:Language_of_Assessment_with_Multiple_QuestionBlocks").Value;
        }

        private static void QMLsAssociatedToTopicName(IAuthoringApiExamples examples)
        {
            ShowTitle("QMLs associated to Topic Name");
            var qmls = examples.GetQMLsByTopicName(Topic_of_Assessment_with_MultipleLanguages);
            foreach (var qml in qmls)
            {
                Console.WriteLine(qml);
            }

            PromptContinue();
        }

        private static void QMLsAssociatedToQuestionId(IAuthoringApiExamples examples)
        {
            ShowTitle("QMLs associated to QuestionId");
            var qmls = examples.GetQMLsByQuestionId(QuestionId_of_Assessment_with_MultipleLanguages);
            foreach (var qml in qmls)
            {
                Console.WriteLine(qml);
            }

            PromptContinue();
        }

        private static void AssessmentRevisionWithGivenAssessmentRevisionIdAndLanguage(IAuthoringApiExamples examples)
        {
            ShowTitle("AssessmentRevision with the given AssessmentRevisionId and Language");
            var assessmentRevisionByKey = examples.GetAssessmentRevisionById(AssessmentRevisionId_of_Assessment_with_MultipleAssessmentQML);
            Console.WriteLine($" AssessmentRevisionID: {assessmentRevisionByKey.Id} - Language: {assessmentRevisionByKey.Language} - AssessmentName: {assessmentRevisionByKey.AssessmentName}");

            PromptContinue();
        }

        private static void AssessmentAMLsWitTheGivenAssessmentRevisionId(IAuthoringApiExamples examples)
        {
            ShowTitle("AssessmentAMLs with the given AssessmentRevisionId");
            var assessmentAMLsOfRevisionId = examples.GetAssessmentAMLsByAssessmentRevisionId(AssessmentRevisionId_of_Assessment_with_MultipleAssessmentQML);
            foreach (var assessmentAMLs in assessmentAMLsOfRevisionId)
            {
                Console.WriteLine($" AssessmentRevisionID: {assessmentAMLs.AssessmentRevisionId} - Language: {assessmentAMLs.Language}");
            }

            PromptContinue();
        }

        private static void GetAssessmentNamesFirstPage(IAuthoringApiExamples examples)
        {
            ShowTitle("Assessment Names in first page");
            var pageAssessmentNames = examples.GetAllAssessmentNamesIteratingEachAvailablePage();
            foreach (var name in pageAssessmentNames)
            {
                Console.WriteLine($" Assessment Name: {name}");
            }

            PromptContinue();
        }

        private static void GetAssessmentNamesIteratingThroughEachPage(IAuthoringApiExamples examples)
        {
            ShowTitle("Assessment Names iterating through each page");
            var assessmentNames = examples.GetAllAssessmentNamesIteratingEachAvailablePage();
            foreach (var name in assessmentNames)
            {
                Console.WriteLine($" Assessment Name: {name}");
            }

            PromptContinue();
        }

        private static void GetAllQMLForAssessment(IAuthoringApiExamples examples)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            ShowTitle("Get all QML belonging to Assessment");

            ShowProgress("1) Get AML ");
            var aml = examples.GetAMLByAssessmentRevisionIdAndLanguage(AssessmentRevisionId_of_Assessment_with_Multiple_QuestionBlocks, Language_of_Assessment_with_Multiple_QuestionBlocks);
            Console.WriteLine(aml);

            ShowProgress("2) Parse AML to get topics and questions ");
            List<int> topics;
            List<int> topicsWithSubTopics;
            List<long> questionIds;
            AMLHelper.GetTopicsAndQuestionsFromAML(aml, out topics, out topicsWithSubTopics, out questionIds);
            Console.WriteLine($"    - Topics: {String.Join(",", topics)}");
            Console.WriteLine($"    - Topics with sub topics: {String.Join(",", topicsWithSubTopics)}");
            Console.WriteLine($"    - Questions: {String.Join(",", questionIds)}");

            ShowProgress("3) Get all sub topics needed ");
            foreach (var topic in topicsWithSubTopics)
            {
                var subTopicIds = examples.GetSubTopicsIdOfTopic(topic);
                foreach (var subTopicId in subTopicIds)
                {
                    if (topics.Contains(subTopicId)) continue;
                    topics.Add(subTopicId);
                }
            }
            Console.WriteLine($"    - All Topics: {String.Join(",", topics)}");

            ShowProgress("4) Get all latest Question Revisions in topics ");
            var questionRevisions = new List<QuestionRevision>();

            foreach (var topicId in topics)
            {
                questionRevisions.AddRange(examples.GetLatestQuestionRevisionsByTopicPublishedId(topicId));
            }
            Console.WriteLine($"    - QuestionRevisions: {String.Join(",", questionRevisions.Select(qr => $"{qr.Id}/{qr.Language}/{qr.QuestionId}"))}");

            ShowProgress("5) Remove duplicated QuestionId ");
            questionIds.RemoveAll(q => questionRevisions.Select(qr => qr.QuestionId).Contains(q));
            Console.WriteLine($"    - QuestionIds after removing duplicates: {String.Join(",", questionIds)}");

            ShowProgress("6) Get all latest Question Revisions by QuestionId ");
            foreach (var questionId in questionIds)
            {
                var questionRevisionsForQuestionId = examples.GetLatestQuestionRevisionsByQuestionId(questionId);
                foreach (var questionRevisionForQuestionId in questionRevisionsForQuestionId)
                {
                    if (questionRevisions.Select(qr => qr.QuestionId).Contains(questionRevisionForQuestionId.QuestionId)) continue;
                    questionRevisions.Add(questionRevisionForQuestionId);
                }
            }

            ShowProgress("7) Get QML for each Question Revision");
            foreach (var questionRevision in questionRevisions)
            {
                var qml = examples.GetQMLByQuestionRevisionIdAndLanguage(questionRevision.Id, questionRevision.Language);
                Console.WriteLine($"    - QML Found:");
                Console.WriteLine(qml);
            }

            stopWatch.Stop();
            ShowTitle($"Completed in: {stopWatch.ElapsedMilliseconds} ms");

            PromptContinue();
        }

        #region UI Helpers
        private static void PromptContinue()
        {
            Console.WriteLine("Do you want to continue with the next example? (y)/(n)");
            var keyStroke = Console.ReadKey();
            if (keyStroke.KeyChar == 'y') return;
            if (keyStroke.KeyChar == 'n') Environment.Exit(0);
            PromptContinue();
        }

        private static void ShowProgress(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine($"\r\n {message}");

            Console.ForegroundColor = color;
        }

        private static void ShowTitle(string title)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine($"************************************************************************");
            Console.WriteLine($" {title} ");
            Console.WriteLine($"************************************************************************");

            Console.ForegroundColor = color;
        }
        #endregion
    }
}
