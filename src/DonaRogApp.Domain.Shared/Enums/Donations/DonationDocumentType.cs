namespace DonaRogApp.Enums.Donations
{
    /// <summary>
    /// Types of documents that can be attached to a donation
    /// </summary>
    public enum DonationDocumentType
    {
        /// <summary>
        /// Bank transfer receipt
        /// </summary>
        BankReceipt = 0,
        
        /// <summary>
        /// Postal order / money order receipt
        /// </summary>
        PostalReceipt = 1,
        
        /// <summary>
        /// PayPal receipt
        /// </summary>
        PayPalReceipt = 2,
        
        /// <summary>
        /// Check image (front/back)
        /// </summary>
        CheckImage = 3,
        
        /// <summary>
        /// Cash donation receipt
        /// </summary>
        CashReceipt = 4,
        
        /// <summary>
        /// Other supporting document
        /// </summary>
        Other = 99
    }
}
