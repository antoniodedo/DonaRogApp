namespace DonaRogApp.Enums.Projects;

/// <summary>
/// Project status
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Attivo - progetto in corso
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Inattivo - progetto temporaneamente sospeso
    /// </summary>
    Inactive = 1,
    
    /// <summary>
    /// Archiviato - progetto concluso o abbandonato
    /// </summary>
    Archived = 2
}
