# Fix per Troncamento Dropdown Ng-Zorro

## Problema
Gli elementi nei dropdown `nz-select` venivano troncati (specialmente l'ultimo elemento) quando c'erano pochi item (2-5 elementi).

## Causa
Ng-Zorro usa il **virtual scroll** di default per gestire grandi liste. Il virtual scroll viene attivato quando ci sono più di `nzOptionOverflowSize` elementi (default: **8**).

Tuttavia, anche con meno di 8 elementi, il meccanismo di virtual scroll può causare problemi di calcolo dell'altezza, portando al troncamento visivo degli ultimi elementi.

## Soluzione Ufficiale dalla Documentazione

Dalla [documentazione ufficiale di Ng-Zorro](https://ng.ant.design/components/select/en#api), abbiamo trovato la proprietà:

### `[nzOptionOverflowSize]`
- **Type**: `number`
- **Default**: `8`
- **Descrizione**: Max option size inside the dropdown, overflow when exceed the size

**Soluzione**: Aumentare questo valore a `20` o più per evitare che il virtual scroll si attivi su liste piccole.

---

## Implementazione

### 1. Configurazione Globale (Consigliata)

In `app.module.ts`:

```typescript
import { NZ_CONFIG, NzConfig } from 'ng-zorro-antd/core/config';

// Global Ng-Zorro configuration
const ngZorroConfig: NzConfig = {
  select: {
    nzOptionOverflowSize: 20  // Prevent virtual scroll truncation for small lists
  }
};

@NgModule({
  providers: [
    { provide: NZ_CONFIG, useValue: ngZorroConfig },
    // ...altri providers
  ]
})
```

**Vantaggi**:
- ✅ Applicato automaticamente a **tutti** i dropdown dell'app
- ✅ Nessuna modifica necessaria nei componenti esistenti
- ✅ Configurazione centralizzata e facile da mantenere

### 2. Configurazione Per-Componente (Opzionale)

Per dropdown specifici che necessitano di un valore diverso:

```html
<nz-select
  formControlName="myField"
  nzPlaceHolder="Seleziona..."
  [nzOptionOverflowSize]="20">
  <nz-option *ngFor="let item of items" [nzValue]="item.id" [nzLabel]="item.name"></nz-option>
</nz-select>
```

---

## Vantaggi della Soluzione

✅ **Soluzione ufficiale** - Usa le API documentate di Ng-Zorro
✅ **Zero CSS hack** - Nessun override di stili interni
✅ **Prestazioni migliori** - Il virtual scroll si attiva solo quando serve davvero
✅ **Manutenibile** - Soluzione robusta e compatibile con future versioni
✅ **Universale** - Risolve il problema per tutti i dropdown con < 20 elementi

---

## Quando il Virtual Scroll si Attiva

| Numero elementi | Default (size=8) | Con fix (size=20) |
|----------------|------------------|-------------------|
| 1-7 elementi   | ❌ Virtual scroll OFF | ❌ Virtual scroll OFF |
| 8-19 elementi  | ⚠️ Virtual scroll ON (può troncare) | ❌ Virtual scroll OFF |
| 20+ elementi   | ✅ Virtual scroll ON | ✅ Virtual scroll ON |

---

## Alternative Considerate (e perché non usate)

### ❌ CSS min-height / padding
**Problema**: Il virtual scroll calcola dinamicamente l'altezza in JavaScript, sovrascrivendo gli stili CSS.

### ❌ Native `<select>`
**Problema**: Non può avere lo stesso look & feel di Ng-Zorro (options custom, ricerca, ecc.)

### ❌ Radio buttons
**Problema**: Adatto solo per 2-3 opzioni, non scalabile, occupa più spazio verticale.

### ✅ nzOptionOverflowSize (SOLUZIONE SCELTA)
**Vantaggi**: Soluzione ufficiale, robusta, manutenibile, zero side effects.

---

## Riferimenti

- [Ng-Zorro Select API Documentation](https://ng.ant.design/components/select/en#api)
- [GitHub Issue #3497 - Virtual scroll feature request](https://github.com/NG-ZORRO/ng-zorro-antd/issues/3497)
- Documentazione applicata: 13 Febbraio 2026

---

## File Modificati

1. **app.module.ts** - Configurazione globale
2. **donation-detail.component.html** - Esempio per-componente (Template Lettera)
3. **campaigns-list.component.html** - Esempio per-componente (Tipo Campagna) x2

---

## Test

Dopo l'implementazione, testare:

1. ✅ Dropdown con 2 elementi (es: Tipo Campagna - Prospect/Archivio)
2. ✅ Dropdown con 3-5 elementi (es: Template Lettera)
3. ✅ Dropdown con 10+ elementi (es: Lista progetti)
4. ✅ Verifica che il virtual scroll si attivi ancora per liste grandi (20+)

---

## Conclusione

Il problema del troncamento era causato dall'attivazione prematura del virtual scroll su liste piccole. La soluzione ufficiale è aumentare `nzOptionOverflowSize` da 8 a 20, evitando così il virtual scroll su liste piccole e medie, mantenendolo attivo solo dove serve davvero (liste grandi 20+ elementi).
