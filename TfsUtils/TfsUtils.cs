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
        public Guid ProjectGuid { get; private set; }

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
        /// <returns>DataTable with the returned information</returns>
        public DataTable Query(Guid queryGuid, string queryTitle = null)
        {
            DataTable dataTable = new DataTable();
            WorkItemQueryResult workItemQueryResult;
            List<WorkItem> workItems;

            Uri uri = new Uri(BaseUrl);
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient =
                new WorkItemTrackingHttpClient(uri, new VssCredentials()))
            {
                if (queryTitle == null)
                {
                    var queryWorkItem = workItemTrackingHttpClient.GetQueryAsync(ProjectGuid, queryGuid.ToString()).Result;
                    queryTitle = queryWorkItem.Name;
                }

                // get a list of referenced work items
                workItemQueryResult = workItemTrackingHttpClient.QueryByIdAsync(ProjectGuid, queryGuid).Result;

                if (!workItemQueryResult.WorkItems.Any())
                {
                    return dataTable;
                }

                dataTable.TableName = queryTitle;

                List<int> workItemsIds = workItemQueryResult.WorkItems.Select(item => item.Id).ToList();

                // now get the actual work items
                workItems = workItemTrackingHttpClient.GetWorkItemsAsync(ProjectGuid, workItemsIds).Result;
            }

            IEnumerable<WorkItemFieldReference> importantColumns = workItemQueryResult.Columns;

            // prepare the datatable
            dataTable.Columns.AddRange(importantColumns.Select(field => new DataColumn(field.Name)).ToArray());
            foreach (WorkItem workItem in workItems)
            {
                DataRow row = dataTable.NewRow();
                foreach (var importantColumn in importantColumns)
                {
                    if (workItem.Fields.ContainsKey(importantColumn.ReferenceName))
                    {
                        row[importantColumn.Name] = workItem.Fields[importantColumn.ReferenceName];
                    }

                    if (importantColumn.Name == "ID" && workItem.Id != null)
                    {
                        row["ID"] = workItem.Id;
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
