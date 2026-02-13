namespace DonaRogApp.Enums.Campaigns;

public enum ResponseType
{
    /// <summary>
    /// Nessuna risposta
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Email/comunicazione aperta
    /// </summary>
    Opened = 1,
    
    /// <summary>
    /// Link cliccato
    /// </summary>
    Clicked = 2,
    
    /// <summary>
    /// Ha donato
    /// </summary>
    Donated = 3,
    
    /// <summary>
    /// Disiscritto
    /// </summary>
    Unsubscribed = 4,
    
    /// <summary>
    /// Email rimbalzata
    /// </summary>
    Bounced = 5
}
