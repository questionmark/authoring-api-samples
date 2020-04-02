using QM.AuthoringApi.OData.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Examples
{
    public class ContainerExamples : IAuthoringApiExamples
    {
        private readonly Default.Container apiClient;

        public ContainerExamples(string baseUrl, string username, string password)
        {
            // Reference: https://docs.microsoft.com/en-gb/odata/client/async-operations
            apiClient = new Default.Container(new Uri(baseUrl));
            apiClient.Credentials = new NetworkCredential(username, password);
        }

        public void Dispose()
        {
            foreach (var entity in apiClient.Entities.ToList())
            {
                apiClient.Detach(entity.Entity);
            }

            foreach (var link in apiClient.Links.ToList())
            {
                apiClient.DetachLink(link.Source, link.SourceProperty, link.Target);
            }
        }

        public IEnumerable<string> GetAllAssessmentNamesIteratingEachAvailablePage()
        {
            throw new NotImplementedException();
        }

        public string GetAMLByAssessmentRevisionIdAndLanguage(int id, string language)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAMLsByAssessmentName(string assessmentName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssessmentAML> GetAssessmentAMLsByAssessmentRevisionId(int id)
        {
            throw new NotImplementedException();
        }

        public AssessmentRevision GetAssessmentRevisionById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByQuestionId(long questionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByTopicPublishedId(int topicId)
        {
            throw new NotImplementedException();
        }

        public string GetQMLByQuestionRevisionIdAndLanguage(int questionRevisionId, string language)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetQMLsByQuestionId(long questionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetQMLsByTopicName(string topicName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QuestionQML> GetQuestionQMLsByQuestionRevisionId(int id)
        {
            throw new NotImplementedException();
        }

        public QuestionRevision GetQuestionRevisionByIdAndLanguage(int id, string language)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetSubTopicsIdOfTopic(int id)
        {
            throw new NotImplementedException();
        }
    }
}
