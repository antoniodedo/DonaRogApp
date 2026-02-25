# Frontend: Sistema Ringraziamenti - Guida Utente

## 🎯 Panoramica

Il sistema di gestione ringraziamenti è stato integrato nell'applicazione Angular con tre aree principali:

1. **Alert Duplicati** nel dettaglio donazione
2. **Batch di Stampa** per gestione lettere
3. **Regole Automatiche** per decisioni intelligenti

---

## 📝 Dettaglio Donazione - Verifica con Ringraziamenti

### Posizione
`/donations/:id/verify` o `/donations/create`

### Funzionalità

#### 1. Alert Duplicati Automatico
Quando si seleziona un donatore, il sistema controlla automaticamente se esistono comunicazioni recenti:

- **🔴 Errore (< 7 giorni)**: Lettera molto recente, attenzione!
- **🟡 Warning (7-15 giorni)**: Lettera recente, valuta se procedere
- **🔵 Info (15-30 giorni)**: Lettera nel periodo, per tua informazione

L'alert mostra:
- Messaggio principale
- Dettagli comunicazioni recenti
- Link per vedere lo storico completo

#### 2. Opzioni Ringraziamento
Sezione dedicata con 3 stati del checkbox:

- **✨ Auto (indeterminato)**: Il sistema valuta le regole automatiche
- **✓ Sì**: Crea ringraziamento (override manuale)
- **✗ No**: Non creare (con campo motivo opzionale)

**Quando "Sì" o "Auto":**
- Scelta canale: Auto / Lettera / Email
- Per lettere: checkbox "Stampa immediatamente"
  - ✓ Selezionato: Genera e stampa subito
  - ✗ Non selezionato: Aggiunge al batch per stampa successiva

**Quando "No":**
- Campo testuale per motivare la scelta (opzionale)

### Flusso Tipico
```
1. Operatore apre donazione da verificare
2. Seleziona donatore → Check automatico duplicati
3. Se alert presente → Valuta se procedere
4. Configura opzioni ringraziamento (o lascia Auto)
5. Completa verifica → Sistema crea comunicazione secondo regole
```

---

## 🖨️ Batch di Stampa

### Posizione
`/communications/print-batches`

### Funzionalità

#### Lista Batch
- Tabella con tutti i batch creati
- Filtri: nome/codice, stato, intervallo date
- Colonne: Codice, Nome, Stato, N° Lettere, Totale €, Dimensione PDF, Data

#### Stati Batch
- **Bozza**: Appena creato, modificabile
- **Pronto**: Pronto per generazione PDF
- **Generazione...**: PDF in corso
- **Scaricato**: PDF generato e scaricato
- **Stampato**: Stampa confermata (finale)
- **Annullato**: Batch annullato

#### Azioni Disponibili
Per ogni batch:

| Azione | Icona | Quando Disponibile | Effetto |
|--------|-------|-------------------|---------|
| Genera PDF | 📄 | Stato Bozza/Pronto + Lettere > 0 | Crea PDF unificato |
| Scarica PDF | ⬇️ | PDF disponibile | Download file |
| Marca Stampato | ✓ | PDF disponibile + non stampato | Finalizza batch |
| Annulla | ✗ | Non stampato | Libera lettere |
| Elimina | 🗑️ | Bozza/Pronto | Rimuove batch |

#### Creazione Nuovo Batch

**Step 1: Configura Filtri**
- Importo: min/max
- Progetti: selezione multipla
- Campagne: selezione multipla
- Data donazione: da/a
- Regione: testo libero
- Categoria donatore: Standard, VIP, Major, Corporate
- Opzioni:
  - ✓ Solo donazioni verificate
  - ✓ Escludi già stampate
  - ✓ Escludi già in altri batch

**Step 2: Anteprima**
Click su "Anteprima Batch" per vedere:
- N° lettere totali
- Importo totale donazioni
- Dimensione PDF stimata
- Breakdown per progetto
- Breakdown per regione

**Step 3: Creazione**
- Nome batch (opzionale)
- Note (opzionali)
- ✓ Genera PDF automaticamente

### Flusso Tipico
```
1. Operatore click "Nuovo Batch"
2. Imposta filtri (es: solo gennaio, progetto Africa)
3. Click "Anteprima" → Vede 150 lettere, 45.000€
4. Conferma creazione
5. (Opzionale) Genera PDF
6. Scarica PDF quando pronto
7. Stampa fisicamente le lettere
8. Marca batch come "Stampato"
```

---

## 🤖 Regole Automatiche Ringraziamenti

### Posizione
`/communications/thank-you-rules`

### Cos'è una Regola?

Una regola definisce:
- **Condizioni**: Quando applicare la regola
- **Azioni**: Cosa fare se la regola matcha

### Logica di Valutazione

Quando si verifica una donazione con opzione "Auto":
1. Sistema carica tutte le regole attive
2. Le ordina per priorità (numero più basso = più importante)
3. Controlla quale regola matcha le condizioni
4. Applica le azioni della prima regola che matcha
5. Se nessuna regola matcha → Default: Sì, Lettera, Template generico

### Condizioni Disponibili

| Condizione | Tipo | Descrizione |
|------------|------|-------------|
| Importo Min/Max | Numerico | Range importo donazione |
| Prima Donazione | Bool | Se è la prima del donatore |
| Categorie Donatori | Multi-select | Standard, VIP, Major, Corporate |
| Tipo Soggetto | Multi-select | Persona Fisica, Azienda, Ente |
| Progetti | Multi-select | Progetti specifici |
| Campagne | Multi-select | Campagne specifiche |

**Nota**: Condizioni vuote = "Tutte" (wildcard)

### Azioni Disponibili

| Azione | Opzioni | Descrizione |
|--------|---------|-------------|
| Crea Ringraziamento | Sì / No | Se creare comunicazione |
| Canale Suggerito | Auto / Lettera / Email | Come comunicare |
| Template Suggerito | Dropdown templates | Quale template usare |

### Priorità e Specificity

Le regole sono ordinate per:
1. **Priorità** (campo numerico 1-1000)
2. **Specificity** (calcolata automaticamente):
   - Più condizioni = più specifica
   - Importo esatto > range importo > nessun importo
   - Prima donazione specificata > non specificata

### Esempi di Regole

#### Esempio 1: Major Donors VIP
```
Nome: Ringraziamento VIP Personalizzato
Priorità: 10
Condizioni:
  - Importo >= 1000€
  - Categoria: VIP, Major
Azioni:
  - ✓ Crea ringraziamento
  - Canale: Lettera
  - Template: "Lettera Personalizzata VIP"
```

#### Esempio 2: Prima Donazione
```
Nome: Benvenuto Nuovi Donatori
Priorità: 20
Condizioni:
  - Prima donazione: Sì
Azioni:
  - ✓ Crea ringraziamento
  - Canale: Auto
  - Template: "Welcome Letter"
```

#### Esempio 3: Piccole Donazioni
```
Nome: No Ringraziamento Piccole Somme
Priorità: 100
Condizioni:
  - Importo < 10€
Azioni:
  - ✗ Non creare ringraziamento
```

### Gestione Regole

#### Lista Regole
- Visualizza tutte le regole ordinate per priorità
- Switch On/Off per attivare/disattivare rapidamente
- Pulsanti ↑↓ per riordinare
- Colonne: Priorità, Nome, Condizioni (summary), Azioni (summary), Stato

#### Creazione/Modifica
- Form completo con validazione
- Anteprima condizioni e azioni
- Salvataggio con feedback immediato

#### Riordinamento
- Click ↑ per aumentare priorità (numero più basso)
- Click ↓ per diminuire priorità (numero più alto)
- Riordinamento istantaneo con refresh automatico

### Best Practices

1. **Ordine delle Regole**: Metti prima le più specifiche
   ```
   Prio 10: VIP + >= 1000€ → Lettera personalizzata
   Prio 20: VIP → Lettera standard VIP
   Prio 50: >= 1000€ → Lettera major
   Prio 100: Prima donazione → Email benvenuto
   Prio 999: Default → Lettera standard
   ```

2. **Testa le Regole**: Usa la funzione "Valuta Regole" con donazioni test

3. **Documenta**: Usa il campo Descrizione per spiegare il "perché"

4. **Monitora**: Verifica periodicamente quali regole matchano più spesso

---

## 🔄 Flussi Completi

### Flusso 1: Verifica con Stampa Immediata
```
Operatore:
1. Apre donazione pending
2. Vede alert "Lettera stampata 5 giorni fa"
3. Decide di procedere comunque
4. Lascia opzione "Auto"
5. Regole suggeriscono: Lettera + Template VIP
6. Operatore seleziona "Stampa immediatamente"
7. Conferma verifica
8. Sistema genera PDF e apre finestra stampa
```

### Flusso 2: Batch Settimanale
```
Operatore ogni lunedì:
1. Va in "Batch di Stampa"
2. Click "Nuovo Batch"
3. Imposta filtri: Settimana scorsa + Escludi stampate
4. Anteprima: 45 lettere, 12.500€
5. Nome: "Batch Settimana 8 - 2026"
6. ✓ Genera PDF automaticamente
7. Crea batch
8. Sistema genera PDF in background
9. Quando pronto: Scarica PDF
10. Stampa fisicamente
11. Marca batch come "Stampato"
```

### Flusso 3: Override Manuale
```
Operatore per donatore speciale:
1. Verifica donazione
2. Vede suggerimento auto: Email
3. Decide di inviare lettera personalizzata invece
4. Cambia opzione a "Sì" (override)
5. Seleziona "Lettera"
6. Non seleziona "Stampa immediatamente"
7. Conferma → Lettera va nel prossimo batch
```

---

## 🎨 UI/UX Notes

### Design Consistency
- Gradient headers per sezioni principali
- Card elevation per contenuti
- Icon usage: Font Awesome via ng-zorro-antd
- Color coding:
  - Blu/Viola: Print Batches
  - Azzurro: Thank You Rules
  - Verde: Success/Completed
  - Rosso: Errors/Danger
  - Giallo/Arancio: Warnings

### Responsive Behavior
- Tables con scroll orizzontale su mobile
- Filtri collapsible su schermi piccoli
- Modal full-screen su mobile

### Loading States
- Skeleton loading per tabelle
- Spinner per azioni async
- Disabled buttons durante operazioni

### Error Handling
- Toast messages per feedback immediato
- Modal di conferma per azioni distruttive
- Validation inline nei form

---

## 🔧 Configurazione Tecnica

### Dipendenze Angular
- `ng-zorro-antd`: UI components
- `@abp/ng.core`: ABP framework integration
- `@abp/ng.theme.shared`: Shared theme components

### Services Utilizzati
```typescript
// Proxy services (auto-generati)
- PrintBatchService
- CommunicationService
- ThankYouRuleService

// Existing services
- DonationService
- DonorService
- LetterTemplateService
- ProjectService
- CampaignService
```

### Routing
```
/communications
  ├── /print-batches       → PrintBatchListComponent
  └── /thank-you-rules     → ThankYouRulesComponent

/donations/:id/verify      → DonationDetailComponent (enhanced)
```

---

## 📊 Future Enhancements (Fase 6)

- [ ] Dashboard statistiche batch
- [ ] Anteprima PDF inline (viewer)
- [ ] Export CSV delle lettere
- [ ] Grouping multiple donazioni per stesso donatore
- [ ] Calcolo costi stampa
- [ ] Test E2E automatici

---

## 🧪 Testing Manuale

### Checklist Test Alert Duplicati
- [ ] Apri verifica donazione per donatore senza lettere recenti → Nessun alert
- [ ] Apri verifica per donatore con lettera di 3 giorni fa → Alert rosso
- [ ] Apri verifica per donatore con lettera di 10 giorni fa → Alert giallo
- [ ] Click "Vedi storico" → Modale con tabella comunicazioni

### Checklist Test Batch
- [ ] Crea batch senza filtri → Vedi tutte le lettere pending
- [ ] Crea batch con filtro >= 100€ → Vedi solo lettere con donazioni >= 100€
- [ ] Genera PDF per batch < 50 lettere → PDF generato sync
- [ ] Genera PDF per batch > 50 lettere → Job background, controlla status
- [ ] Scarica PDF → File scaricato correttamente
- [ ] Marca come stampato → Status cambia, lettere marcate

### Checklist Test Regole
- [ ] Crea regola per prima donazione → Email benvenuto
- [ ] Crea regola per >= 500€ → Lettera VIP
- [ ] Riordina regole con ↑↓ → Priorità aggiornate
- [ ] Disattiva regola → Non applicata in valutazioni
- [ ] Verifica donazione che matcha regola → Opzioni pre-compilate secondo regola

---

## 💡 Tips per Operatori

1. **Usa Auto quando possibile**: Le regole sono progettate per semplificare il lavoro
2. **Controlla gli Alert**: Non ignorare gli alert rossi, potrebbero indicare errori
3. **Batch Regolari**: Crea batch settimanali o bi-settimanali per evitare accumuli
4. **Stampa Immediata**: Usa solo per casi urgenti o VIP
5. **Regole Semplici**: Meglio 10 regole chiare che 3 complesse

---

## 🆘 Troubleshooting

### "Nessuna lettera trovata"
- Verifica che ci siano donazioni verificate
- Controlla filtri batch (es: OnlyVerified = true)
- Verifica che le lettere non siano già in altri batch

### "PDF non si genera"
- Controlla log backend per errori template
- Verifica che tutte le lettere abbiano contenuto valido
- Per batch grandi (>100), attendi il job background

### "Alert duplicati non appare"
- Verifica che il donatore abbia comunicazioni recenti
- Controlla threshold days nelle regole (default 30 giorni)
- Verifica connessione backend

### "Regole non si applicano"
- Verifica che la regola sia attiva (switch On)
- Controlla priorità (più bassa = prima)
- Verifica che le condizioni matchino (check con anteprima)

---

**Ultima modifica**: 22 Febbraio 2026  
**Versione**: 1.0.0
