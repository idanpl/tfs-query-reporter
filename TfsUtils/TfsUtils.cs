using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace TfsQueryReporter.Tfs
{
    /// <summary>
    /// TFS staff for the project
    /// </summary>
    public class TfsUtils
    {
        public string BaseUrl { get; private set; }
        public Guid ProjectGuid { get; private set;}

        /// <summary>
        /// c'tor
        /// </summary>
        /// <param name="baseUrl">the base url for TFS</param>
        /// <param name="projectGuid">The TFS project guid</param>
        public TfsUtils(string baseUrl, Guid projectGuid)
        {
            this.BaseUrl = baseUrl;
            this.ProjectGuid = projectGuid;
        }

        /// <summary>
        /// Perform Query to TFS and return the results
        /// </summary>
        /// <param name="queryTitle">the title of the query</param>
        /// <param name="queryGuid">the query's guid </param>
        /// <param name="importantFields">Fields to be returned</param>
        /// <returns>DataTable with the returned information</returns>
        public DataTable Query(string queryTitle, Guid queryGuid, List<string> importantFields)
        {
            DataTable dataTable = new DataTable();
            List<IDictionary<string, object>> queryResult = new List<IDictionary<string, object>>();

            Uri uri = new Uri(BaseUrl);
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, new VssCredentials()))
            {
                // get a list of referenced work items
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByIdAsync(ProjectGuid, queryGuid).Result;

                if (!workItemQueryResult.WorkItems.Any())
                {
                    return dataTable;
                }

                dataTable.TableName = queryTitle;

                List<int> workItemsIds = workItemQueryResult.WorkItems.Select(item => item.Id).ToList();

                // now get the actual work items
                List<WorkItem> workItems = workItemTrackingHttpClient.GetWorkItemsAsync(ProjectGuid, workItemsIds).Result;

                // just a temp data structure.
                
                foreach (WorkItem workItem in workItems)
                {
                    if (workItem.Id == null)
                    {
                        continue;
                    }

                    queryResult.Add(workItem.Fields);
                }
            }

            // prepare the datatable
            dataTable.Columns.AddRange( importantFields.Select(field => new DataColumn(field)).ToArray());
            foreach (IDictionary<string, object> queryRecord in queryResult)
            {
                DataRow row = dataTable.NewRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (queryRecord.ContainsKey(column.ColumnName))
                    {
                        row[column.ColumnName] = queryRecord[column.ColumnName];
                    }
                }
                                
                dataTable.Rows.Add(row);
            }        

            return dataTable;
        }
    }
}
