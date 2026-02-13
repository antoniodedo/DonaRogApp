using DonaRogApp.Enums.Projects;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Projects.Entities
{
    public partial class Project
    {
        /// <summary>
        /// Create a new charity project
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="code">Project code (unique per tenant)</param>
        /// <param name="name">Project name</param>
        /// <param name="category">Project category</param>
        /// <param name="startDate">Start date</param>
        /// <param name="description">Project description (optional)</param>
        /// <returns>New Project instance</returns>
        public static Project Create(
            Guid id,
            Guid? tenantId,
            string code,
            string name,
            ProjectCategory category,
            DateTime startDate,
            string? description = null)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return new Project(
                id,
                tenantId,
                code,
                name,
                category,
                startDate,
                description);
        }
    }
}
