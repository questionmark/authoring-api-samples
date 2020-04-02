using QM.AuthoringApi.OData.Entity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Examples.Extensions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Examples
{
    public class HttpClientExamples : IAuthoringApiExamples
    {
        private readonly HttpClient apiClient;
        public HttpClientExamples(string baseUrl, string username, string password)
        {
            var authorizationKey = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(baseUrl);
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationKey);
        }

        public void Dispose()
        {
            apiClient.Dispose();
        }

        public IEnumerable<string> GetAllAssessmentNamesIteratingEachAvailablePage()
        {
            var httpResponseMessage = Get("AssessmentRevisions?$select=AssessmentName");
            var responseString = httpResponseMessage.GetContentRawContentAsString();
            var jsonArrayResponse = JArray.Parse(responseString);
            var result = new List<string>();

            string nextLink = null;
            do
            {
                foreach (JObject parsedObject in jsonArrayResponse.Children<JObject>())
                {
                    result.Add(parsedObject.Value<string>("AssessmentName"));
                    nextLink = httpResponseMessage.GetODataMetadata().GetNextLink();
                }
            } while (!string.IsNullOrEmpty(nextLink));

            return result;
        }

        public IEnumerable<string> GetAMLsByAssessmentName(string assessmentName)
        {
            var httpResponseMessage = Get("AssessmentRevisions?$select=AssessmentName");
            var responseString = httpResponseMessage.GetContentRawContentAsString();
            var jsonArrayResponse = JArray.Parse(responseString);
            var result = new List<string>();

            foreach (JObject parsedObject in jsonArrayResponse.Children<JObject>())
            {
                result.Add(parsedObject.Value<string>("AssessmentName"));
            }

            return result;
        }

        public string GetAMLByAssessmentRevisionIdAndLanguage(int id, string language)
        {
            var streamHttpResponseMessage = Get($"AssessmentAMLs(AssessmentRevisionId={id},Language='{language}')/$value");
            var aml = streamHttpResponseMessage.Content.ReadAsStringAsync().Result;
            return aml;
        }

        public IEnumerable<AssessmentAML> GetAssessmentAMLsByAssessmentRevisionId(int id)
        {
            var httpResponseMessage = Get($"AssessmentAMLs?$filter=AssessmentRevisionId eq {id}");
            var assessmentAMLs = httpResponseMessage.GetContentAs<List<AssessmentAML>>();
            return assessmentAMLs;
        }

        public AssessmentRevision GetAssessmentRevisionById(int id)
        {
            var httpResponseMessage = Get($"AssessmentRevisions/{id}");
            return httpResponseMessage.GetContentAs<AssessmentRevision>();
        }

        public IEnumerable<string> GetQMLsByQuestionId(long questionId)
        {
            var httpResponseMessage = Get($"QuestionQMLs?$filter=QuestionRevision/QuestionId eq {questionId}L");
            var questionQMLs = httpResponseMessage.GetContentAs<List<QuestionQML>>();

            var qmls = new List<string>();
            foreach (var questionQML in questionQMLs)
            {
                var streamHttpResponseMessage = Get($"QuestionQMLs(QuestionRevisionId={questionQML.QuestionRevisionId},Language='{questionQML.Language}')/$value");
                qmls.Add(streamHttpResponseMessage.Content.ReadAsStringAsync().Result);
            }

            return qmls;
        }

        public IEnumerable<string> GetQMLsByTopicName(string topicName)
        {
            var httpResponseMessage = Get($"QuestionQMLs?$filter=QuestionRevision/Topic/Name eq '{topicName}'");
            var questionQMLs = httpResponseMessage.GetContentAs<List<QuestionQML>>();

            var qmls = new List<string>();
            foreach (var questionQML in questionQMLs)
            {
                var streamHttpResponseMessage = Get($"QuestionQMLs(QuestionRevisionId={questionQML.QuestionRevisionId},Language='{questionQML.Language}')/$value");
                qmls.Add(streamHttpResponseMessage.Content.ReadAsStringAsync().Result);
            }

            return qmls;
        }

        public IEnumerable<int> GetSubTopicsIdOfTopic(int id)
        {
            var httpResponseMessage = Get($"Topics/{id}");
            var topicPath = httpResponseMessage.GetContentAs<Topic>().Path;

            var qmls = new List<string>();
            var subTopicsHttpResponseMessage = Get($"Topics?$filter=startswith(Path, '{topicPath + "/"}')");
            var subTopics = subTopicsHttpResponseMessage.GetContentAs<List<Topic>>();

            var result = new List<int>();
            foreach(var subTopic in subTopics)
            {
                if (result.Contains(subTopic.Id)) continue;
                result.Add(subTopic.Id);
            }

            return result;
        }
        
        public IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByTopicPublishedId(int topicId)
        {
            var httpResponseMessage = Get($"QuestionRevisions?$filter=Topic/PublishedId eq {topicId}&$orderby=ModifiedDateTime desc");
            var questionRevisions = httpResponseMessage.GetContentAs<List<QuestionRevision>>();

            var latestRevisions = new Dictionary<long, QuestionRevision>();

            foreach(var questionRevision in questionRevisions)
            {
                if (latestRevisions.ContainsKey(questionRevision.QuestionId)) continue;
                latestRevisions.Add(questionRevision.QuestionId, questionRevision);
            }

            return latestRevisions.Values;
        }

        public IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByQuestionId(long questionId)
        {
            var httpResponseMessage = Get($"QuestionRevisions?$filter=QuestionId eq {questionId}L&$OrderBy=ModifiedDateTime desc");
            
            var questionRevisions = httpResponseMessage.GetContentAs<List<QuestionRevision>>();

            var latestRevisions = new Dictionary<long, QuestionRevision>();

            foreach (var questionRevision in questionRevisions)
            {
                if (latestRevisions.ContainsKey(questionRevision.QuestionId)) continue;
                latestRevisions.Add(questionRevision.QuestionId, questionRevision);
            }

            return latestRevisions.Values;
        }

        public string GetQMLByQuestionRevisionIdAndLanguage(int questionRevision, string language)
        {
            var streamHttpResponseMessage = Get($"QuestionQMLs(QuestionRevisionId={questionRevision},Language='{language}')/$value");
            return streamHttpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        private HttpResponseMessage Get(string relativeUri)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = apiClient.GetAsync(relativeUri).Result;
            stopwatch.Stop();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" HTTP Request URL ({stopwatch.ElapsedMilliseconds} ms) - {relativeUri}");
            Console.ForegroundColor = color;

            return result;
        }
    }
}
