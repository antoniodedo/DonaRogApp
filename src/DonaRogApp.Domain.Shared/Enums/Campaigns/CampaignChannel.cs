namespace DonaRogApp.Enums.Campaigns;

public enum CampaignChannel
{
    /// <summary>
    /// Campagna cartacea (posta)
    /// </summary>
    Postal = 0,
    
    /// <summary>
    /// Campagna email
    /// </summary>
    Email = 1,
    
    /// <summary>
    /// Campagna SMS
    /// </summary>
    SMS = 2,
    
    /// <summary>
    /// Campagna telefonica
    /// </summary>
    Phone = 3,
    
    /// <summary>
    /// Campagna mista (più canali)
    /// </summary>
    Mixed = 4
}
