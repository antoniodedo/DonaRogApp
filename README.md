# DonaRog CRM

CRM per la gestione di donatori, donazioni, campagne e comunicazioni, sviluppato con **ABP Framework 9.2 / .NET 9** e **Angular 19**.

---

## Indice

1. [Panoramica](#panoramica)
2. [Stack tecnologico](#stack-tecnologico)
3. [Prerequisiti](#prerequisiti)
4. [Avvio rapido (Docker)](#avvio-rapido-docker)
5. [Configurazione ambiente (.env)](#configurazione-ambiente-env)
6. [Scenari di deploy](#scenari-di-deploy)
   - [DB locale in Docker](#scenario-a--db-locale-in-docker)
   - [DB esterno su altra macchina](#scenario-b--db-esterno-su-altra-macchina)
7. [Comandi utili](#comandi-utili)
8. [Sviluppo locale (senza Docker)](#sviluppo-locale-senza-docker)
9. [Architettura](#architettura)
10. [Funzionalità principali](#funzionalità-principali)
11. [Credenziali default](#credenziali-default)
12. [Risoluzione problemi](#risoluzione-problemi)

---

## Panoramica

DonaRog è un CRM nonprofit che gestisce l'intero ciclo di vita del rapporto con i donatori:

- Anagrafica donatori con indirizzi, contatti, email verificate e allegati
- Gestione donazioni con documenti e ricevute
- Campagne di raccolta fondi con tracciamento risposte
- Regole di ringraziamento automatiche con template letter
- Segmentazione automatica dei donatori tramite regole configurabili
- Stampa batch di lettere di ringraziamento
- Dashboard KPI con trend donazioni e analisi RFM

---

## Stack tecnologico

| Layer | Tecnologia |
|---|---|
| Backend | .NET 9 / ASP.NET Core / ABP Framework 9.2 |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 9 / Npgsql |
| Auth | OpenIddict (OAuth2 / Authorization Code Flow) |
| Frontend | Angular 19 / NG-Zorro Ant Design |
| PDF | DinkToPdf (wkhtmltopdf) |
| Container | Docker + Docker Compose |

---

## Prerequisiti

### Per il deploy con Docker (consigliato)

- **Docker Desktop** ≥ 26 (Windows/macOS) oppure **Docker Engine** ≥ 26 + **Docker Compose** v2 (Linux)
- Connessione internet per il primo download delle immagini base

### Per lo sviluppo locale (senza Docker)

- .NET SDK 9.0
- Node.js 18+ e Yarn
- PostgreSQL 16 (oppure Docker per il solo DB)
- wkhtmltopdf installato nel sistema

---

## Avvio rapido (Docker)

```bash
# 1. Clona il repository
git clone https://github.com/antoniodedo/DonaRogApp.git
cd DonaRogApp

# 2. Crea il file di configurazione
cp .env.example .env

# 3. Avvia tutto (DB locale + migrazione + API + frontend)
./deploy.sh            # Linux / macOS
# oppure su Windows:
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up --build -d
```

Al primo avvio Docker:
- scarica le immagini base (~1.5 GB totali, solo la prima volta)
- compila il backend .NET e il frontend Angular (~3-5 minuti)
- crea il database PostgreSQL
- applica tutte le migration EF Core
- esegue il seed dei dati iniziali (utente admin, permessi, OpenIddict)

**L'app è pronta quando tutti e tre i container sono `Up`:**

```
donarog-postgres   Up (healthy)
donarog-api        Up
donarog-frontend   Up
```

Apri il browser su **http://localhost:4200**

---

## Configurazione ambiente (.env)

Il file `.env` contiene tutte le variabili configurabili. **Non viene committato nel repository** (contiene credenziali).

```env
# --- Database ---
# DB locale Docker: DB_HOST=postgres  (usa con docker-compose.local-db.yml)
# DB esterno:       DB_HOST=192.168.1.100
DB_HOST=postgres
DB_PORT=5432
DB_NAME=donarogdb
DB_USER=donarog
DB_PASSWORD=DonaRog_Secret!

# --- Server ---
# IP o dominio del server dove l'app è raggiungibile
# localhost per sviluppo, IP/dominio per server remoto
SERVER_HOST=localhost

# --- Porte ---
API_PORT=44318
FRONTEND_PORT=4200

# --- Sicurezza ---
OPENIDDICT_CERT_PASSPHRASE=35c5ebd2-19a4-4b32-8b17-24d91bf11c47

# --- Ambiente ---
ASPNETCORE_ENVIRONMENT=Staging
```

> **Importante:** in un ambiente di produzione cambia `DB_PASSWORD` e `OPENIDDICT_CERT_PASSPHRASE` con valori sicuri.

---

## Scenari di deploy

### Scenario A — DB locale in Docker

Il database PostgreSQL gira nello stesso server come container Docker.

```bash
# Configura .env con DB_HOST=postgres e SERVER_HOST=IP-del-server
nano .env

# Avvia
./deploy.sh
# oppure esplicitamente:
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up -d
```

**Struttura container:**
```
┌─────────────────────────────────────────────┐
│ Server Linux                                │
│                                             │
│  ┌─────────────┐    ┌──────────────────┐   │
│  │  PostgreSQL │◄───│  DbMigrator      │   │
│  │  :5432      │    │  (esegue 1 sola  │   │
│  └─────────────┘    │   volta)         │   │
│         ▲           └──────────────────┘   │
│         │                                  │
│  ┌──────┴──────┐    ┌──────────────────┐   │
│  │  API        │    │  Frontend        │   │
│  │  :44318     │◄───│  Nginx :4200     │   │
│  └─────────────┘    └──────────────────┘   │
└─────────────────────────────────────────────┘
```

### Scenario B — DB esterno su altra macchina

Il database PostgreSQL è già in esecuzione su un server separato.

```bash
# In .env configura:
#   DB_HOST=192.168.1.100   (IP del server DB)
#   DB_HOST=db.miodominio.it (oppure dominio)
nano .env

# Avvia senza il compose del DB locale
./deploy.sh --ext-db
# oppure:
docker compose up -d
```

**Struttura container:**
```
┌────────────────────────┐     ┌───────────────────┐
│ Server App             │     │ Server DB         │
│                        │     │                   │
│  ┌──────────────────┐  │     │  ┌─────────────┐  │
│  │  DbMigrator      │──┼─────┼─►│  PostgreSQL │  │
│  └──────────────────┘  │     │  │  :5432      │  │
│  ┌──────────────────┐  │     │  └─────────────┘  │
│  │  API :44318      │──┼─────┘                   │
│  └──────────────────┘  │                         │
│  ┌──────────────────┐  │                         │
│  │  Frontend :4200  │  │                         │
│  └──────────────────┘  │                         │
└────────────────────────┘                         
```

---

## Comandi utili

```bash
# --- Avvio ---

# Prima volta (o dopo modifica al codice): build + avvio
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up --build -d

# Avvii successivi (immagini già buildate, solo DB locale)
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up -d

# Avvio con DB esterno
docker compose up -d


# --- Stato e log ---

# Stato container
docker compose ps

# Log in tempo reale di tutti i servizi
docker compose -f docker-compose.yml -f docker-compose.local-db.yml logs -f

# Log solo dell'API
docker compose logs -f api

# Log solo del migrator (utile per debug migration)
docker compose logs migrator


# --- Stop e reset ---

# Ferma tutto (mantiene i dati)
docker compose -f docker-compose.yml -f docker-compose.local-db.yml down

# Ferma tutto e cancella i dati (reset completo DB)
docker compose -f docker-compose.yml -f docker-compose.local-db.yml down -v


# --- Aggiornamento ---

# Dopo un git pull: rebuild e riavvio
git pull
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up --build -d


# --- Cambio configurazione (senza rebuild) ---

# Modifica .env, poi riavvia solo i container interessati
nano .env
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up -d
```

---

## Sviluppo locale (senza Docker)

### 1. Avvia solo il database in Docker

```bash
docker run -d \
  --name donarog-postgres-dev \
  -e POSTGRES_DB=donarogdb \
  -e POSTGRES_USER=donarog \
  -e POSTGRES_PASSWORD=DonaRog_Secret! \
  -p 5432:5432 \
  postgres:16-alpine
```

### 2. Configura le connection string

In `src/DonaRogApp.DbMigrator/appsettings.json` e `src/DonaRogApp.HttpApi.Host/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=donarogdb;Username=donarog;Password=DonaRog_Secret!"
  }
}
```

### 3. Applica le migration e avvia il backend

```bash
# Applica migration e seed dati
cd src/DonaRogApp.DbMigrator
dotnet run

# Avvia l'API (in un altro terminale)
cd src/DonaRogApp.HttpApi.Host
dotnet run
```

### 4. Avvia il frontend Angular

```bash
cd angular
yarn install
yarn start
# Frontend disponibile su http://localhost:4200
```

### 5. Aggiungi una nuova migration (dopo modifiche al modello)

```bash
cd src/DonaRogApp.EntityFrameworkCore
dotnet ef migrations add NomeMigration \
  --startup-project ../DonaRogApp.DbMigrator/DonaRogApp.DbMigrator.csproj
```

---

## Architettura

```
DonaRogApp/
├── src/
│   ├── DonaRogApp.Domain.Shared         # Enumerazioni, costanti, localizzazione
│   ├── DonaRogApp.Domain                # Entità, value objects, domain services
│   ├── DonaRogApp.Application.Contracts # DTOs, interfacce AppService
│   ├── DonaRogApp.Application           # Application services, AutoMapper, worker
│   ├── DonaRogApp.EntityFrameworkCore   # DbContext, migration, configurazione EF
│   ├── DonaRogApp.HttpApi               # Controller ASP.NET Core
│   ├── DonaRogApp.HttpApi.Host          # Host (startup, Swagger, OpenIddict)
│   └── DonaRogApp.DbMigrator            # Tool standalone per migration e seed
├── angular/                             # Frontend Angular 19
│   ├── src/app/
│   │   ├── donors/                      # Gestione donatori
│   │   ├── donations/                   # Gestione donazioni
│   │   ├── projects/                    # Gestione progetti
│   │   ├── campaigns/                   # Campagne (in admin/)
│   │   ├── communications/              # Stampa batch, regole ringraziamento
│   │   ├── letter-templates/            # Template lettere
│   │   └── admin/                       # Segmentazione, campagne, ruoli, utenti
│   ├── Dockerfile                       # Build multi-stage + Nginx
│   └── docker-entrypoint.sh            # Genera dynamic-env.json a runtime
├── test/                                # Test xUnit
├── docker-compose.yml                   # Servizi base (DB esterno)
├── docker-compose.local-db.yml         # Aggiunge PostgreSQL locale
├── .env.example                         # Template configurazione
├── deploy.sh                            # Script di deploy per Linux
└── README.md                            # Questo file
```

### Flusso di avvio Docker

```
postgres (healthcheck OK)
    └──► migrator  ──► crea DB + applica migration + seed
                            └──► api  ──► avvio ASP.NET Core
                                             └──► frontend  ──► Nginx serve Angular
```

---

## Funzionalità principali

### Donatori
- Anagrafica completa (persona fisica / azienda) con codice fiscale e P.IVA
- Indirizzi multipli con storico, contatti telefonici, email verificate
- Note storiche, tag, interessi, segmenti assegnati
- Allegati file (PDF, immagini)
- Storico donazioni e comunicazioni
- Statistiche RFM (Recency, Frequency, Monetary)

### Donazioni
- Registrazione con importo, canale, progetto destinatario
- Workflow di approvazione con documenti allegati
- Integrazione con campagne e ricevute

### Campagne
- Campagne multicanale (email, SMS, posta)
- Tracciamento aperture, click e risposte
- Estrazione liste donatori per campagna

### Regole di ringraziamento
- Regole configurabili per importo, canale, campagna, periodo
- Associazione a template letter con rotazione LRU
- Stampa batch PDF con wkhtmltopdf

### Segmentazione automatica
- Regole configurabili con criteri: importo totale donato, numero donazioni, ultima data, segmento corrente
- Esecuzione batch manuale o tramite background worker
- Anteprima live dei donatori che soddisfano una regola

### Template lettere
- Editor TinyMCE con placeholder dinamici (`{{donor.firstName}}`, ecc.)
- Supporto upload DOCX (conversione automatica tramite Mammoth)
- Allegati multipli per template
- Anteprima PDF

---

## Credenziali default

| Campo | Valore |
|---|---|
| Username | `admin` |
| Password | `1q2w3E*` |

> Cambia la password al primo accesso in un ambiente non locale.

---

## Risoluzione problemi

### Il migrator fallisce con "connection refused"

Il container del DB non è ancora pronto. Se usi il DB locale, verifica che `docker-compose.local-db.yml` sia incluso nel comando. Se usi un DB esterno, verifica che sia raggiungibile dalla macchina Docker.

```bash
# Testa la connessione al DB (sostituisci con i tuoi valori)
docker run --rm postgres:16-alpine \
  pg_isready -h TUO_DB_HOST -p 5432 -U donarog
```

### L'API non si avvia: "Signing Certificate couldn't found"

Il certificato OpenIddict non è stato generato. Ricostruisci l'immagine API:

```bash
docker compose build api
docker compose up -d api
```

### Il frontend mostra "Http failure response"

Gli URL nel `.env` non corrispondono a quelli dove il browser raggiunge l'API. Verifica che `SERVER_HOST` sia l'IP o dominio effettivamente raggiungibile dal browser (non `localhost` se accedi da un altro PC).

```bash
# Dopo aver modificato .env, ricrea solo il frontend (nessun rebuild necessario)
docker compose up -d --force-recreate frontend
```

### Reset completo (cancella tutti i dati)

```bash
docker compose -f docker-compose.yml -f docker-compose.local-db.yml down -v
docker compose -f docker-compose.yml -f docker-compose.local-db.yml up --build -d
```

### Vedere i log dettagliati

```bash
# Log migrator (migration e seed)
docker compose logs migrator

# Log API in tempo reale
docker compose logs -f api

# Log di tutti i servizi con timestamp
docker compose -f docker-compose.yml -f docker-compose.local-db.yml logs -f -t
```
