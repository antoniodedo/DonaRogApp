# Guida: Native Select vs Ng-Zorro Select

## Quando usare Native Select

Usa `<select class="native-select">` invece di `<nz-select>` quando:

✅ **Lista piccola** (2-10 elementi)
✅ **Elementi statici** (non caricati dinamicamente)
✅ **Nessuna ricerca** necessaria
✅ **Nessun template custom** per le opzioni
✅ **Vuoi evitare** problemi di virtual scroll e troncamento

## Quando usare Ng-Zorro Select

Usa `<nz-select>` quando:

✅ **Lista grande** (10+ elementi)
✅ **Ricerca/filtro** necessaria (`nzShowSearch`)
✅ **Template custom** per le opzioni (icone, multi-linea, ecc.)
✅ **Multi-selezione** (`nzMode="multiple"` o `tags`)
✅ **Caricamento asincrono** o **lazy loading**

---

## Esempi di Utilizzo

### 1. Native Select - Basic

```html
<select class="native-select" [(ngModel)]="selectedValue">
  <option value="">Seleziona...</option>
  <option [value]="CampaignType.Prospect">Prospect</option>
  <option [value]="CampaignType.Archive">Archivio</option>
</select>
```

### 2. Native Select - Con *ngFor

```html
<select class="native-select" [(ngModel)]="selectedTemplateId">
  <option value="">Seleziona template</option>
  <option *ngFor="let template of templates" [value]="template.id">
    {{ template.name }}
  </option>
</select>
```

### 3. Native Select - In Form Reattivo

```html
<select class="native-select" formControlName="category">
  <option value="">Tutte</option>
  <option *ngFor="let cat of categories" [value]="cat.value">
    {{ cat.label }}
  </option>
</select>
```

### 4. Native Select - Dimensioni

```html
<!-- Small -->
<select class="native-select native-select-sm" [(ngModel)]="value">
  ...
</select>

<!-- Default -->
<select class="native-select" [(ngModel)]="value">
  ...
</select>

<!-- Large -->
<select class="native-select native-select-lg" [(ngModel)]="value">
  ...
</select>
```

### 5. Native Select - Disabled

```html
<select class="native-select" [(ngModel)]="value" [disabled]="true">
  <option value="">Non disponibile</option>
</select>
```

---

## Confronto Visivo

| Feature | Native Select | Ng-Zorro Select |
|---------|--------------|-----------------|
| Virtual Scroll | ❌ No | ✅ Sì |
| Troncamento elementi | ❌ Mai | ⚠️ Possibile |
| Ricerca | ❌ No | ✅ Sì |
| Template custom | ❌ No | ✅ Sì |
| Multi-select | ❌ No | ✅ Sì |
| Performance (piccole liste) | ✅ Ottima | ⚠️ Overhead |
| Stile consistente | ✅ Sì | ✅ Sì |

---

## Migrazione da nz-select a native-select

### Prima (Ng-Zorro):
```html
<nz-select 
  [(ngModel)]="quickFilters.campaignType" 
  nzAllowClear 
  nzPlaceHolder="Tutti">
  <nz-option 
    *ngFor="let opt of campaignTypeOptions" 
    [nzValue]="opt.value" 
    [nzLabel]="opt.key">
  </nz-option>
</nz-select>
```

### Dopo (Native):
```html
<select 
  class="native-select" 
  [(ngModel)]="quickFilters.campaignType">
  <option value="">Tutti</option>
  <option 
    *ngFor="let opt of campaignTypeOptions" 
    [value]="opt.value">
    {{ opt.key }}
  </option>
</select>
```

---

## Note Importanti

1. **nzAllowClear**: Con native select, usa `<option value="">Tutti</option>` come prima opzione
2. **nzPlaceHolder**: Con native select, usa `<option value="" disabled selected>Seleziona...</option>`
3. **Validazione**: Funziona normalmente con Angular Forms (sia template-driven che reactive)
4. **Accessibilità**: Native select è completamente accessibile di default

---

## Dropdown Consigliati per Native Select

Nel progetto DonaRogApp, questi dropdown sono ottimi candidati:

✅ Template Lettera (lista limitata)
✅ Tipo Campagna (2 opzioni: Prospect/Archivio)
✅ Stato (poche opzioni fisse)
✅ Lingua (4-5 opzioni)
✅ Genere (3-4 opzioni)
✅ Tipo Organizzazione (lista fissa)
✅ Tipo Comunicazione (Email/Lettera)

❌ NON usare per:
- Progetti (lista può crescere molto)
- Ricorrenze (lista può crescere molto)
- Tag (necessita ricerca)
- Donatori (lista grande + ricerca)
