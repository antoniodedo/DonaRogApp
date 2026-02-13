namespace DonaRogApp.Enums.Campaigns;

public enum CampaignStatus
{
    /// <summary>
    /// Bozza
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// In preparazione (configurazione in corso)
    /// </summary>
    InPreparation = 1,
    
    /// <summary>
    /// Donatori estratti
    /// </summary>
    Extracted = 2,
    
    /// <summary>
    /// Spedita/inviata
    /// </summary>
    Dispatched = 3,
    
    /// <summary>
    /// Completata
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// Cancellata
    /// </summary>
    Cancelled = 5
}
