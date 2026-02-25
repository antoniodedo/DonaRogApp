# Configurazione File Storage

## Panoramica

Il sistema di gestione documenti delle donazioni supporta diversi provider di storage:
- **Local**: Storage su filesystem locale del server
- **AzureBlob**: Azure Blob Storage (cloud)
- **S3**: Amazon S3 o servizi compatibili (cloud)

## Configurazione

La configurazione si trova in `appsettings.json`:

```json
{
  "FileStorage": {
    "Provider": "Local",
    "BasePath": "D:/DonaRogApp/Files/Donations",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.gif",
    "UsePresignedUrls": false,
    "PresignedUrlExpirationMinutes": 60
  }
}
```

## Provider Locali

### Local Filesystem

```json
{
  "FileStorage": {
    "Provider": "Local",
    "BasePath": "D:/DonaRogApp/Files/Donations",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.gif"
  }
}
```

**Note:**
- `BasePath` deve essere un percorso assoluto accessibile dall'applicazione
- L'applicazione creerà automaticamente le sottocartelle per anno/mese
- Assicurarsi che l'applicazione abbia permessi di lettura/scrittura sulla directory

**Percorsi consigliati:**
- Windows: `D:/DonaRogApp/Files/Donations` o `C:/DonaRogApp/Files/Donations`
- Linux: `/var/donarog/files/donations` o `/opt/donarog/files/donations`

## Provider Cloud

### Azure Blob Storage

```json
{
  "FileStorage": {
    "Provider": "AzureBlob",
    "BasePath": "donations",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.gif",
    "AzureConnectionString": "DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net",
    "UsePresignedUrls": true,
    "PresignedUrlExpirationMinutes": 60
  }
}
```

**Note:**
- `BasePath` è il nome del container Azure
- `AzureConnectionString` si ottiene dal portale Azure
- `UsePresignedUrls=true` genera URL temporanei per l'accesso ai file

### Amazon S3

```json
{
  "FileStorage": {
    "Provider": "S3",
    "BasePath": "donarog-donations",
    "MaxFileSizeMB": 10,
    "AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.gif",
    "S3": {
      "AccessKey": "YOUR_ACCESS_KEY",
      "SecretKey": "YOUR_SECRET_KEY",
      "Region": "eu-south-1",
      "BucketName": "donarog-donations"
    },
    "UsePresignedUrls": true,
    "PresignedUrlExpirationMinutes": 60
  }
}
```

**Note:**
- `BasePath` è il nome del bucket S3
- Configurare le credenziali IAM con permessi S3
- `Region` deve corrispondere alla region del bucket

## Limitazioni File

### Dimensione Massima

```json
"MaxFileSizeMB": 10
```

Dimensione massima per file in megabyte. Default: 10MB

### Estensioni Permesse

```json
"AllowedExtensions": ".jpg,.jpeg,.png,.pdf,.gif"
```

Estensioni file permesse (separare con virgola).

**Tipi documento supportati:**
- Immagini: `.jpg`, `.jpeg`, `.png`, `.gif`
- Documenti: `.pdf`

## Presigned URLs (Cloud Storage)

Per storage cloud (Azure Blob, S3), è possibile utilizzare presigned URLs:

```json
"UsePresignedUrls": true,
"PresignedUrlExpirationMinutes": 60
```

**Vantaggi:**
- URL temporanei con scadenza automatica
- Non espone credenziali di storage
- Migliore sicurezza per file sensibili

**Limitazioni:**
- Richiede configurazione provider cloud
- Non disponibile per storage locale

## Struttura Directory

I file sono organizzati per anno/mese:

```
BasePath/
  └── 2024/
      ├── 01/
      │   ├── uuid1.pdf
      │   └── uuid2.jpg
      ├── 02/
      │   └── uuid3.pdf
      └── 03/
          └── uuid4.jpg
```

Ogni file ha un nome univoco (GUID) per evitare collisioni.

## Sicurezza

### Permessi Filesystem (Local)

Per storage locale, assicurarsi che:
1. L'applicazione abbia permessi di lettura/scrittura sulla directory `BasePath`
2. La directory non sia accessibile pubblicamente via web
3. Configurare backup regolari

### Cloud Storage

Per storage cloud:
1. Utilizzare credenziali IAM con privilegi minimi necessari
2. Abilitare encryption at rest
3. Configurare lifecycle policies per retention
4. Monitorare accessi sospetti

## Implementazione Futura

**Provider aggiuntivi da implementare:**
- Implementazione completa Azure Blob Storage
- Implementazione completa AWS S3
- Google Cloud Storage
- MinIO (S3-compatible)

**Features:**
- Drag & drop avanzato nel frontend
- Progress bar per upload
- Compressione automatica immagini
- Generazione thumbnail
- Watermarking automatico
