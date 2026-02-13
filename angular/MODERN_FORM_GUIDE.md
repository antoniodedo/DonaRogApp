# Guida allo Stile Moderno dei Form

Questa guida fornisce istruzioni per creare form con uno stile uniforme e moderno in tutta l'applicazione.

## 🎨 Caratteristiche dello Stile

- **Design Moderno**: Interfaccia pulita con ombreggiature e animazioni
- **Sezioni Organizzate**: Form divisi in sezioni logiche con titoli chiari
- **Icone Descrittive**: Icone accanto alle label per migliorare la comprensione
- **Responsive**: Layout ottimizzato per desktop e mobile
- **Feedback Visivo**: Stati hover, focus e errore ben definiti

## 📋 Template Base

```html
<form nz-form [formGroup]="form" nzLayout="vertical" class="modern-form">
  
  <!-- ============================================ -->
  <!-- SEZIONE 1: TITOLO SEZIONE -->
  <!-- ============================================ -->
  <div class="form-section">
    <h3 class="section-title">
      <span nz-icon nzType="file-text" nzTheme="outline"></span>
      Titolo Sezione
    </h3>
    
    <div nz-row [nzGutter]="16">
      <div nz-col [nzSpan]="12">
        <nz-form-item>
          <nz-form-label nzRequired>
            <span nz-icon nzType="tag" class="label-icon"></span>
            Nome Campo
          </nz-form-label>
          <nz-form-control nzErrorTip="Messaggio di errore">
            <input nz-input formControlName="campo" placeholder="Placeholder" />
          </nz-form-control>
        </nz-form-item>
      </div>
    </div>
  </div>

  <nz-divider></nz-divider>

  <!-- ============================================ -->
  <!-- ACTIONS -->
  <!-- ============================================ -->
  <div class="form-actions">
    <button nz-button nzType="default" nzSize="large" (click)="cancel()">
      <span nz-icon nzType="close"></span>
      Annulla
    </button>
    <button nz-button nzType="primary" nzSize="large" (click)="save()" [nzLoading]="loading">
      <span nz-icon nzType="save"></span>
      Salva
    </button>
  </div>
</form>
```

## 🏗️ Struttura dei Componenti

### 1. Container Form
```html
<form nz-form [formGroup]="form" nzLayout="vertical" class="modern-form">
```
- **Classe obbligatoria**: `modern-form`
- **Layout**: Sempre `vertical` per consistenza

### 2. Sezioni
```html
<div class="form-section">
  <h3 class="section-title">
    <span nz-icon nzType="[icona]" nzTheme="outline"></span>
    Titolo Sezione
  </h3>
  <!-- campi del form -->
</div>
```

**Icone consigliate per sezioni:**
- `file-text`: Informazioni generali
- `calendar`: Date e periodi
- `form`: Descrizioni e note
- `appstore`: Tipologia e categorizzazione
- `dollar`: Costi e budget
- `team`: Destinatari e contatti

### 3. Campi del Form

#### Input Testo
```html
<div nz-col [nzSpan]="12">
  <nz-form-item>
    <nz-form-label nzRequired>
      <span nz-icon nzType="tag" class="label-icon"></span>
      Nome Campo
    </nz-form-label>
    <nz-form-control nzErrorTip="Inserisci il valore" nzExtra="Testo di aiuto">
      <input nz-input formControlName="campo" placeholder="Es: Valore" />
    </nz-form-control>
  </nz-form-item>
</div>
```

#### Input Numero
```html
<nz-form-control nzErrorTip="Errore" nzExtra="Testo aiuto">
  <nz-input-number
    formControlName="numero"
    [nzMin]="0"
    [nzMax]="100"
    [nzStep]="1"
    style="width: 100%;"
    nzPlaceHolder="0">
  </nz-input-number>
</nz-form-control>
```

#### Date Picker
```html
<nz-form-control nzExtra="Seleziona la data">
  <nz-date-picker
    formControlName="data"
    nzFormat="dd/MM/yyyy"
    nzPlaceHolder="Seleziona data"
    style="width: 100%;">
  </nz-date-picker>
</nz-form-control>
```

#### Select
```html
<nz-form-control nzErrorTip="Seleziona un'opzione">
  <nz-select formControlName="select" nzPlaceHolder="Seleziona">
    <nz-option [nzValue]="1" nzLabel="Opzione 1"></nz-option>
    <nz-option [nzValue]="2" nzLabel="Opzione 2"></nz-option>
  </nz-select>
</nz-form-control>
```

#### Textarea
```html
<nz-form-control>
  <textarea
    nz-input
    formControlName="descrizione"
    placeholder="Inserisci descrizione..."
    [nzAutosize]="{ minRows: 4, maxRows: 8 }">
  </textarea>
</nz-form-control>
```

#### Radio Group (Orizzontale)
```html
<nz-form-control>
  <nz-radio-group formControlName="tipo" class="radio-group-horizontal">
    <label nz-radio [nzValue]="1">Opzione 1</label>
    <label nz-radio [nzValue]="2">Opzione 2</label>
    <label nz-radio [nzValue]="3">Opzione 3</label>
  </nz-radio-group>
</nz-form-control>
```

### 4. Box Informativi
```html
<div class="info-box">
  <span nz-icon nzType="info-circle"></span>
  <span>Messaggio informativo per l'utente</span>
</div>
```

### 5. Pulsanti di Azione
```html
<div class="form-actions">
  <button nz-button nzType="default" nzSize="large" (click)="cancel()">
    <span nz-icon nzType="close"></span>
    Annulla
  </button>
  <button nz-button nzType="primary" nzSize="large" (click)="save()" [nzLoading]="loading">
    <span nz-icon nzType="save"></span>
    {{ isEditMode ? 'Aggiorna' : 'Salva' }}
  </button>
</div>
```

## 🎯 Icone Consigliate per Label

| Campo | Icona | Type |
|-------|-------|------|
| Nome/Titolo | `tag` | outline |
| Codice | `barcode` | outline |
| Descrizione | `align-left` | outline |
| Data | `calendar` | outline |
| Ora | `clock-circle` | outline |
| Email | `mail` | outline |
| Telefono | `phone` | outline |
| Indirizzo | `environment` | outline |
| Importo | `dollar` | outline |
| Quantità | `number` | outline |
| Categoria | `folder` | outline |
| Stato | `info-circle` | outline |
| Note | `file-text` | outline |

## 📱 Layout Responsive

### Desktop (> 768px)
```html
<div nz-row [nzGutter]="16">
  <div nz-col [nzSpan]="12"><!-- Campo 1 --></div>
  <div nz-col [nzSpan]="12"><!-- Campo 2 --></div>
</div>
```

### Mobile (Automatico)
Lo stile `modern-form` gestisce automaticamente il layout mobile con:
- Padding ridotto
- Pulsanti full-width
- Gutter ridotto tra colonne

## 🔧 Customizzazione dei Colori

Per cambiare il colore principale (default: `#1890ff` - blu):

1. Nel componente SCSS, sovrascrivi le variabili:
```scss
.modern-form {
  .section-title i,
  .section-title [nz-icon] {
    color: #14b8a6; // Teal per esempio
  }

  .form-actions button[nzType="primary"] {
    background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
    box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
  }
}
```

## ✅ Checklist per Nuovo Form

- [ ] Aggiunta classe `modern-form` al form
- [ ] Form diviso in sezioni logiche con `form-section`
- [ ] Ogni sezione ha `section-title` con icona
- [ ] Tutte le label hanno `label-icon` appropriata
- [ ] Campi required hanno `nzRequired`
- [ ] Messaggi di errore con `nzErrorTip`
- [ ] Testi di aiuto con `nzExtra`
- [ ] Divider (`<nz-divider>`) tra le sezioni
- [ ] Pulsanti in `form-actions` alla fine
- [ ] Layout responsive con `nz-row` e `nz-col`

## 📚 Esempi Completi

Vedi i seguenti file per esempi completi:
- `src/app/admin/recurrences/recurrence-form/` - Form ricorrenze
- `src/app/admin/campaigns/campaign-form/` - Form campagne

## 🚀 Vantaggi

1. **Consistenza**: Tutti i form hanno lo stesso aspetto
2. **Manutenibilità**: Modifiche allo stile globale in un unico posto
3. **Accessibilità**: Icone e label migliorano la comprensione
4. **UX**: Feedback visivo chiaro e transizioni fluide
5. **Responsive**: Funziona perfettamente su tutti i dispositivi
