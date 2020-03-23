using QM.AuthoringApi.OData.Entity;
using System;
using System.Collections.Generic;

namespace Examples
{
    /// <summary>
    /// Each method represent an example of request
    /// </summary>
    public interface IAuthoringApiExamples : IDisposable
    {
        /// <summary>
        /// Get the assessment revision filtering by the AssessmentRevisionId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AssessmentRevision by AssessmentRevisionId</returns>
        AssessmentRevision GetAssessmentRevisionById(int id);

        /// <summary>
        /// Get all AssessmentAML filtering by assessment revision Id
        /// Note: The maximum amount of results is limited by the page size defined by the server
        /// </summary>
        /// <param name="id">AssessmentRevisionId</param>
        /// <returns>All AssessmentAML with the specified AssessmentRevisionId</returns>
        IEnumerable<AssessmentAML> GetAssessmentAMLsByAssessmentRevisionId(int id);

        /// <summary>
        /// Get all AML streams filtering by the Assessment Name
        /// Note: The maximum amount of results is limited by the page size defined by the server
        /// </summary>
        /// <param name="assessmentName">Name of the assessment to search</param>
        /// <returns>List of all AMLs</returns>
        IEnumerable<string> GetAMLsByAssessmentName(string assessmentName);

        /// <summary>
        /// Get the AML with AssessmentRevisionId and Language
        /// </summary>
        /// <param name="id">AssessmentRevisionId</param>
        /// <param name="language">Language</param>
        /// <returns>AML of the corresponding Assessment</returns>
        string GetAMLByAssessmentRevisionIdAndLanguage(int id, string language);

        /// <summary>
        /// Get QML by QuestionRevisionId and Language
        /// </summary>
        /// <param name="questionRevisionId">QuestionRevisionId</param>
        /// <param name="language">Language</param>
        /// <returns>QML</returns>
        string GetQMLByQuestionRevisionIdAndLanguage(int questionRevisionId, string language);

        /// <summary>
        /// Get all QML streams filtering by the QuestionId (Id used in the delivery platform after publishing)
        /// Note: The maximum amount of results is limited by the page size defined by the server
        /// </summary>
        /// <param name="questionId">Question identifier as known in the delivery system after published</param>
        /// <returns>QML of each question with the given published Id</returns>
        IEnumerable<string> GetQMLsByQuestionId(long questionId);

        /// <summary>
        /// Get all QML streams filtering by Topic Name
        /// Note: The maximum amount of results is limited by the page size defined by the server
        /// </summary>
        /// <param name="topicName">Name of the topic where all questions belong to</param>
        /// <returns>QML of each question belonging to the topic with the given name</returns>
        IEnumerable<string> GetQMLsByTopicName(string topicName);

        /// <summary>
        /// Get all assessment names iterating through each page
        /// </summary>
        /// <returns>The name of each assessment</returns>
        IEnumerable<string> GetAllAssessmentNamesIteratingEachAvailablePage();
        
        /// <summary>
        /// Get list of Id of all sub topics of the topic with teh given Id
        /// </summary>
        /// <param name="topicId">Id of the topic</param>
        /// <returns>Lit of the sub topics Ids</returns>
        IEnumerable<int> GetSubTopicsIdOfTopic(int topicId);

        /// <summary>
        /// Get the latest QuestionRevision by Topic PublishedId
        /// </summary>
        /// <param name="publishedId">Published Id of the topic</param>
        /// <returns>The last version of the question revision</returns>
        IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByTopicPublishedId(int publishedId);

        /// <summary>
        /// Get the latest QuestionRevision by QuestionId
        /// </summary>
        /// <param name="questionId">Question Id as appear in QML and AML</param>
        /// <returns>The last version of the question revision</returns>
        IEnumerable<QuestionRevision> GetLatestQuestionRevisionsByQuestionId(long questionId);
    }
}
