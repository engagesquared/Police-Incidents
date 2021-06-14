// <copyright file="GraphUtilityHelper.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using PoliceIncidents.Tab.Models;

    public class GraphUtilityHelper
    {
        private readonly GraphServiceClient graphClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphUtilityHelper"/> class.
        /// </summary>
        /// <param name="accessToken">Token to access MS graph.</param>
        public GraphUtilityHelper(string accessToken)
        {
            this.graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        await Task.Run(() =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                accessToken);
                        });
                    }));
        }

        public async Task<List<string>> GetUserUpns(List<Guid> ids)
        {
            var users = await this.graphClient.Users.Request().Filter(string.Join(" or ", ids.Select(x => $"id eq '{x}'"))).Select(x => x.UserPrincipalName).GetAsync();
            return users.Select(x => x.UserPrincipalName).ToList();
        }

        public async Task<string> CreateCopyOfPlanner(string planToCopyId, IncidentInputModel incidentInput)
        {
            var planToCopy = await this.graphClient.Planner.Plans[planToCopyId].Request().GetAsync();
            var planToCopyBuckets = await this.graphClient.Planner.Plans[planToCopyId].Buckets.Request().GetAsync();
            var planToCopyTasks = await this.graphClient.Planner.Plans[planToCopyId].Tasks.Request().GetAsync();
            var planDetailsToCopy = await this.graphClient.Planner.Plans[planToCopyId].Details.Request().GetAsync();

            var plannerPlan = new PlannerPlan
            {
                Owner = incidentInput.RegionId.ToString(),
                Title = this.IncidentModelTempating(planToCopy.Title, incidentInput),
            };

            var createdPlan = await this.graphClient.Planner.Plans.Request().AddAsync(plannerPlan);

            var plannerPlanDetails = new PlannerPlanDetails
            {
                CategoryDescriptions = planDetailsToCopy.CategoryDescriptions,
            };

            var createdPlanDetails = await this.graphClient.Planner.Plans[createdPlan.Id].Details.Request().Select(x => x.Id).GetAsync();
            await this.graphClient.Planner.Plans[createdPlan.Id].Details.Request()
                .Header("Prefer", "return=representation")
                .Header("If-Match", createdPlanDetails.GetEtag())
                .UpdateAsync(plannerPlanDetails);

            foreach (var bucketToCopy in planToCopyBuckets.OrderByDescending(x => x.OrderHint))
            {
                var newBucket = await this.graphClient.Planner.Buckets.Request()
                    .AddAsync(new PlannerBucket
                    {
                        Name = this.IncidentModelTempating(bucketToCopy.Name, incidentInput),
                        PlanId = createdPlan.Id,
                        OrderHint = " !",
                    });
                var previousOrderHint = string.Empty;

                // Ordering is wrong for tasks
                // https://laurakokkarinen.com/how-to-sort-tasks-using-planner-order-hint-and-microsoft-graph/
                var tasks = planToCopyTasks.Where(x => x.BucketId == bucketToCopy.Id).OrderByDescending(x => x.OrderHint).ToList();
                foreach (var taskToCopy in tasks)
                {
                    var taskToCopyDetails = await this.graphClient.Planner.Tasks[taskToCopy.Id].Details.Request().GetAsync();
                    var plannerTask = new PlannerTask
                    {
                        PlanId = createdPlan.Id,
                        BucketId = newBucket.Id,
                        OrderHint = $"{previousOrderHint} !",
                        Title = this.IncidentModelTempating(taskToCopy.Title, incidentInput),
                        AppliedCategories = taskToCopy.AppliedCategories,
                    };

                    var newTask = await this.graphClient.Planner.Tasks.Request().AddAsync(plannerTask);
                    previousOrderHint = newTask.OrderHint;

                    PlannerChecklistItems checkList = new PlannerChecklistItems();
                    foreach (var item in taskToCopyDetails.Checklist)
                    {
                        checkList.AddChecklistItem(this.IncidentModelTempating(item.Value.Title, incidentInput));
                    }

                    var plannerTaskDetails = new PlannerTaskDetails
                    {
                        PreviewType = taskToCopyDetails.PreviewType,
                        Checklist = checkList,
                        Description = this.IncidentModelTempating(taskToCopyDetails.Description, incidentInput),
                    };
                    var newTaskDetails = await this.graphClient.Planner.Tasks[newTask.Id].Details.Request().Select(x => x.Id).GetAsync();

                    await this.graphClient.Planner.Tasks[newTask.Id].Details
                        .Request()
                        .Header("Prefer", "return=representation")
                        .Header("If-Match", newTaskDetails.GetEtag())
                        .UpdateAsync(plannerTaskDetails);
                }
            }

            return $"https://tasks.office.com/#/plantaskboard?groupId={incidentInput.RegionId}&planId={createdPlan.Id}";
        }

        public async Task<Guid[]> GetUsersByGroupId(string groupId)
        {
            var members = await this.graphClient.Groups[groupId].Members.Request().Select(x => x.Id).GetAsync();
            var owners = await this.graphClient.Groups[groupId].Owners.Request().Select(x => x.Id).GetAsync();
            var result = members.Select(x => new Guid(x.Id)).ToList();
            result.AddRange(owners.Select(x => new Guid(x.Id)).ToList());

            return result.ToArray();
        }

        private string IncidentModelTempating(string input, IncidentInputModel incidentInput)
        {
            return input
                .Replace("{IncidentTitle}", incidentInput.Title, StringComparison.OrdinalIgnoreCase)
                .Replace("{IncidentLocation}", incidentInput.Location, StringComparison.OrdinalIgnoreCase)
                .Replace("{IncidentDescription}", incidentInput.Description, StringComparison.OrdinalIgnoreCase);
        }
    }
}
