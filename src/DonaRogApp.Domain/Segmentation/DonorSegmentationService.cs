using DonaRogApp.Domain.Donors.Entities;
using DonaRogApp.Domain.Segmentation.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace DonaRogApp.Domain.Segmentation
{
    /// <summary>
    /// Domain service for donor segmentation
    /// Evaluates segmentation rules and assigns donors to segments
    /// </summary>
    public class DonorSegmentationService : DomainService
    {
        private readonly IRepository<SegmentationRule, Guid> _ruleRepository;
        private readonly IRepository<DonorSegment> _donorSegmentRepository;
        private readonly IRepository<Donor, Guid> _donorRepository;

        public DonorSegmentationService(
            IRepository<SegmentationRule, Guid> ruleRepository,
            IRepository<DonorSegment> donorSegmentRepository,
            IRepository<Donor, Guid> donorRepository)
        {
            _ruleRepository = ruleRepository;
            _donorSegmentRepository = donorSegmentRepository;
            _donorRepository = donorRepository;
        }

        /// <summary>
        /// Evaluate all active rules for a donor and assign matching segments
        /// Returns list of segment IDs the donor was assigned to
        /// </summary>
        public async Task<List<Guid>> EvaluateAndAssignSegmentsAsync(Donor donor)
        {
            Logger.LogDebug("Evaluating segmentation rules for donor {DonorId}", donor.Id);

            // Get all active rules ordered by priority
            var activeRules = await _ruleRepository.GetListAsync(r => r.IsActive);
            var orderedRules = activeRules.OrderBy(r => r.Priority).ToList();

            Logger.LogDebug("Found {RuleCount} active segmentation rules", orderedRules.Count);

            // Get existing automatic segment assignments for this donor
            var existingAssignments = await _donorSegmentRepository.GetListAsync(
                ds => ds.DonorId == donor.Id && ds.IsAutomatic && !ds.RemovedAt.HasValue);

            var existingSegmentIds = existingAssignments.Select(ds => ds.SegmentId).ToHashSet();
            var matchedSegmentIds = new List<Guid>();

            // Evaluate each rule
            foreach (var rule in orderedRules)
            {
                if (rule.Matches(donor))
                {
                    matchedSegmentIds.Add(rule.SegmentId);
                    
                    // Check if donor is already in this segment
                    var existingAssignment = existingAssignments.FirstOrDefault(ds => ds.SegmentId == rule.SegmentId);
                    
                    if (existingAssignment == null)
                    {
                        // Create new assignment
                        var newAssignment = DonorSegment.CreateAutomatic(
                            donor.Id,
                            rule.SegmentId,
                            donor.TenantId,
                            automaticReason: $"Rule: {rule.Name}",
                            notes: $"Auto-assigned by rule '{rule.Name}' (Priority: {rule.Priority})");

                        await _donorSegmentRepository.InsertAsync(newAssignment);
                        
                        Logger.LogInformation(
                            "Donor {DonorId} assigned to segment {SegmentId} by rule '{RuleName}'",
                            donor.Id, rule.SegmentId, rule.Name);
                    }
                    else
                    {
                        Logger.LogDebug(
                            "Donor {DonorId} already in segment {SegmentId}, keeping assignment",
                            donor.Id, rule.SegmentId);
                    }
                }
            }

            // Remove assignments for segments that no longer match
            await RemoveObsoleteSegmentAssignmentsAsync(donor, matchedSegmentIds);

            return matchedSegmentIds;
        }

        /// <summary>
        /// Remove automatic segment assignments that no longer match any rules
        /// Only removes automatic assignments, manual assignments are preserved
        /// </summary>
        public async Task RemoveObsoleteSegmentAssignmentsAsync(Donor donor, List<Guid> currentMatchingSegmentIds)
        {
            // Get all active automatic assignments
            var automaticAssignments = await _donorSegmentRepository.GetListAsync(
                ds => ds.DonorId == donor.Id && ds.IsAutomatic && !ds.RemovedAt.HasValue);

            foreach (var assignment in automaticAssignments)
            {
                // If segment is not in current matching list, remove it
                if (!currentMatchingSegmentIds.Contains(assignment.SegmentId))
                {
                    assignment.Remove();
                    await _donorSegmentRepository.UpdateAsync(assignment);
                    
                    Logger.LogInformation(
                        "Removed donor {DonorId} from segment {SegmentId} (no longer matches rules)",
                        donor.Id, assignment.SegmentId);
                }
            }
        }

        /// <summary>
        /// Evaluate rules for all donors (batch processing)
        /// Returns statistics: donors processed, assignments created, assignments removed
        /// </summary>
        public async Task<SegmentationBatchResult> EvaluateAllDonorsAsync(int batchSize = 1000)
        {
            Logger.LogInformation("Starting batch segmentation for all donors (batch size: {BatchSize})", batchSize);

            var result = new SegmentationBatchResult
            {
                StartTime = DateTime.UtcNow
            };

            // Get all active rules (check if there are any)
            var activeRules = await _ruleRepository.GetListAsync(r => r.IsActive);
            if (!activeRules.Any())
            {
                Logger.LogWarning("No active segmentation rules found, skipping batch processing");
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            Logger.LogInformation("Found {RuleCount} active segmentation rules", activeRules.Count);

            // Process donors in batches
            var totalDonors = await _donorRepository.CountAsync();
            var processedCount = 0;

            Logger.LogInformation("Processing {TotalDonors} donors in batches", totalDonors);

            while (processedCount < totalDonors)
            {
                // Get next batch of donors using paged query
                var donorQuery = await _donorRepository.GetQueryableAsync();
                var donors = donorQuery
                    .Skip(processedCount)
                    .Take(batchSize)
                    .ToList();

                if (!donors.Any())
                    break;

                foreach (var donor in donors)
                {
                    try
                    {
                        // Track assignments before
                        var assignmentsBefore = await _donorSegmentRepository.CountAsync(
                            ds => ds.DonorId == donor.Id && ds.IsAutomatic && !ds.RemovedAt.HasValue);

                        // Evaluate and assign segments
                        var matchedSegments = await EvaluateAndAssignSegmentsAsync(donor);

                        // Track assignments after
                        var assignmentsAfter = await _donorSegmentRepository.CountAsync(
                            ds => ds.DonorId == donor.Id && ds.IsAutomatic && !ds.RemovedAt.HasValue);

                        // Update statistics
                        result.DonorsProcessed++;
                        
                        if (assignmentsAfter > assignmentsBefore)
                            result.AssignmentsCreated += (assignmentsAfter - assignmentsBefore);
                        else if (assignmentsAfter < assignmentsBefore)
                            result.AssignmentsRemoved += (assignmentsBefore - assignmentsAfter);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error processing donor {DonorId} in segmentation batch", donor.Id);
                        result.Errors++;
                    }
                }

                processedCount += donors.Count;
                
                Logger.LogInformation(
                    "Batch progress: {ProcessedCount}/{TotalDonors} donors processed",
                    processedCount, totalDonors);
            }

            result.EndTime = DateTime.UtcNow;
            
            Logger.LogInformation(
                "Batch segmentation completed: {DonorsProcessed} donors, {AssignmentsCreated} created, {AssignmentsRemoved} removed, {Errors} errors in {Duration}s",
                result.DonorsProcessed, result.AssignmentsCreated, result.AssignmentsRemoved, result.Errors, result.DurationSeconds);

            return result;
        }

        /// <summary>
        /// Evaluate a single rule and return matching donor count (for preview)
        /// </summary>
        public async Task<int> CountMatchingDonorsAsync(SegmentationRule rule)
        {
            Logger.LogDebug("Counting donors matching rule {RuleId} '{RuleName}'", rule.Id, rule.Name);

            var allDonors = await _donorRepository.GetListAsync();
            var matchingCount = allDonors.Count(donor => rule.Matches(donor));

            Logger.LogDebug("Rule '{RuleName}' matches {Count} donors", rule.Name, matchingCount);

            return matchingCount;
        }

        /// <summary>
        /// Get list of donors matching a rule (for preview, limited to first N)
        /// </summary>
        public async Task<List<Donor>> GetMatchingDonorsAsync(SegmentationRule rule, int maxResults = 100)
        {
            Logger.LogDebug("Getting donors matching rule {RuleId} '{RuleName}' (limit: {MaxResults})", 
                rule.Id, rule.Name, maxResults);

            var allDonors = await _donorRepository.GetListAsync();
            var matchingDonors = allDonors.Where(donor => rule.Matches(donor)).Take(maxResults).ToList();

            Logger.LogDebug("Rule '{RuleName}' matches {Count} donors (showing {Shown})", 
                rule.Name, matchingDonors.Count, Math.Min(matchingDonors.Count, maxResults));

            return matchingDonors;
        }

        /// <summary>
        /// Apply a single rule to all donors (manual execution)
        /// </summary>
        public async Task<int> ApplyRuleToAllDonorsAsync(Guid ruleId)
        {
            Logger.LogInformation("Manually applying rule {RuleId} to all donors", ruleId);

            var rule = await _ruleRepository.GetAsync(ruleId);
            
            if (!rule.IsActive)
            {
                Logger.LogWarning("Cannot apply inactive rule {RuleId}", ruleId);
                return 0;
            }

            var allDonors = await _donorRepository.GetListAsync();
            var assignedCount = 0;

            foreach (var donor in allDonors)
            {
                if (rule.Matches(donor))
                {
                    // Check if already assigned
                    var existingAssignment = await _donorSegmentRepository.FirstOrDefaultAsync(
                        ds => ds.DonorId == donor.Id && 
                              ds.SegmentId == rule.SegmentId && 
                              ds.IsAutomatic && 
                              !ds.RemovedAt.HasValue);

                    if (existingAssignment == null)
                    {
                        var newAssignment = DonorSegment.CreateAutomatic(
                            donor.Id,
                            rule.SegmentId,
                            donor.TenantId,
                            automaticReason: $"Manual application of rule: {rule.Name}",
                            notes: $"Manually applied rule '{rule.Name}'");

                        await _donorSegmentRepository.InsertAsync(newAssignment);
                        assignedCount++;
                    }
                }
            }

            Logger.LogInformation(
                "Rule {RuleId} '{RuleName}' applied: {AssignedCount} donors assigned",
                ruleId, rule.Name, assignedCount);

            return assignedCount;
        }
    }

    /// <summary>
    /// Result of batch segmentation processing
    /// </summary>
    public class SegmentationBatchResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DonorsProcessed { get; set; }
        public int AssignmentsCreated { get; set; }
        public int AssignmentsRemoved { get; set; }
        public int Errors { get; set; }
        
        public double DurationSeconds => (EndTime - StartTime).TotalSeconds;
    }
}
