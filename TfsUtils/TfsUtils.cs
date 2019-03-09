using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace TfsUtils
{
    public class TfsUtils
    {
        public string BaseUrl { get; set; }
        public Guid ProjectGuid { get; set;}

        public void Query(Guid queryGuid)
        {
            Uri uri = new Uri(BaseUrl);
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, new VssCredentials()))
            {
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByIdAsync(ProjectGuid, queryGuid).Result;

                if (!workItemQueryResult.WorkItems.Any())
                {
                    return;
                }

                foreach (WorkItemReference workItem in workItemQueryResult.WorkItems)
                {
                    // do something in here.
                }

            }
        }
    }
}
