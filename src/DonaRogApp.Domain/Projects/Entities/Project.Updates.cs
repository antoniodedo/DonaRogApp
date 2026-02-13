using DonaRogApp.Enums.Projects;
using System;
using Volo.Abp;

namespace DonaRogApp.Domain.Projects.Entities
{
    public partial class Project
    {
        /// <summary>
        /// Update project basic information
        /// </summary>
        public void UpdateInfo(
            string name,
            ProjectCategory category,
            string? description = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 200);
            Category = category;
            Description = description;

            VerifyInvariants();
        }

        /// <summary>
        /// Update project code
        /// </summary>
        public void UpdateCode(string code)
        {
            Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 50);
            VerifyInvariants();
        }

        /// <summary>
        /// Update project dates
        /// </summary>
        public void UpdateDates(DateTime startDate, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;

            VerifyInvariants();
        }

        /// <summary>
        /// Change project status
        /// </summary>
        public void ChangeStatus(ProjectStatus newStatus)
        {
            if (Status == newStatus)
                return;

            // Validate status transitions
            ValidateStatusTransition(Status, newStatus);

            var oldStatus = Status;
            Status = newStatus;

            // TODO: Add domain event when needed
            // AddDistributedEvent(new ProjectStatusChangedEvent(Id, oldStatus, newStatus));
        }

        /// <summary>
        /// Activate project
        /// </summary>
        public void Activate()
        {
            ChangeStatus(ProjectStatus.Active);
        }

        /// <summary>
        /// Deactivate project (pause)
        /// </summary>
        public void Deactivate()
        {
            ChangeStatus(ProjectStatus.Inactive);
        }

        /// <summary>
        /// Archive project
        /// </summary>
        public void Archive()
        {
            ChangeStatus(ProjectStatus.Archived);
        }

        /// <summary>
        /// Set target budget
        /// </summary>
        public void SetBudget(decimal? targetAmount, string? currency = null)
        {
            if (targetAmount.HasValue && targetAmount.Value < 0)
            {
                throw new BusinessException("DonaRog:ProjectNegativeTargetAmount")
                    .WithData("targetAmount", targetAmount.Value);
            }

            TargetAmount = targetAmount;
            
            if (!string.IsNullOrWhiteSpace(currency))
            {
                Currency = Check.NotNullOrWhiteSpace(currency, nameof(currency), maxLength: 3);
            }
        }

        /// <summary>
        /// Set responsible person
        /// </summary>
        public void SetResponsible(
            string? responsiblePerson = null,
            string? responsibleEmail = null,
            string? responsiblePhone = null)
        {
            ResponsiblePerson = responsiblePerson;
            ResponsibleEmail = responsibleEmail;
            ResponsiblePhone = responsiblePhone;

            VerifyInvariants();
        }

        /// <summary>
        /// Set main image
        /// </summary>
        public void SetMainImage(string? imageUrl, string? thumbnailUrl = null)
        {
            MainImageUrl = imageUrl;
            ThumbnailUrl = thumbnailUrl ?? imageUrl;
        }

        /// <summary>
        /// Set project location
        /// </summary>
        public void SetLocation(string? location, decimal? latitude = null, decimal? longitude = null)
        {
            Location = location;
            Latitude = latitude;
            Longitude = longitude;

            if (latitude.HasValue && (latitude.Value < -90 || latitude.Value > 90))
            {
                throw new BusinessException("DonaRog:ProjectInvalidLatitude")
                    .WithData("latitude", latitude.Value);
            }

            if (longitude.HasValue && (longitude.Value < -180 || longitude.Value > 180))
            {
                throw new BusinessException("DonaRog:ProjectInvalidLongitude")
                    .WithData("longitude", longitude.Value);
            }
        }

        /// <summary>
        /// Validate status transitions
        /// </summary>
        private void ValidateStatusTransition(ProjectStatus currentStatus, ProjectStatus newStatus)
        {
            // Valid transitions:
            // Active <-> Inactive
            // Active -> Archived
            // Inactive -> Archived
            // (Cannot go back from Archived to Active/Inactive)

            if (currentStatus == ProjectStatus.Archived && newStatus != ProjectStatus.Archived)
            {
                throw new BusinessException("DonaRog:ProjectCannotUnarchive")
                    .WithData("currentStatus", currentStatus)
                    .WithData("newStatus", newStatus);
            }

            // All other transitions are allowed
        }
    }
}
