import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, debounceTime } from 'rxjs/operators';

export interface NominatimResult {
  place_id: number;
  display_name: string;
  lat: string;
  lon: string;
  address: {
    house_number?: string;
    road?: string;
    suburb?: string;
    city?: string;
    town?: string;
    village?: string;
    municipality?: string;
    county?: string;
    state?: string;
    postcode?: string;
    country?: string;
    country_code?: string;
  };
}

export interface ParsedAddress {
  street: string;
  city: string;
  province: string;
  region: string;
  postalCode: string;
  country: string;
  countryCode: string;
  latitude: number;
  longitude: number;
  displayName: string;
}

@Injectable({
  providedIn: 'root'
})
export class NominatimService {
  private readonly baseUrl = 'https://nominatim.openstreetmap.org';

  constructor(private http: HttpClient) {}

  /**
   * Cerca indirizzi tramite Nominatim API
   * @param query - Testo da cercare
   * @param countryCode - Codice paese (es. 'it', 'de', 'fr') o vuoto per tutto il mondo
   */
  search(query: string, countryCode?: string): Observable<ParsedAddress[]> {
    if (!query || query.length < 3) {
      return of([]);
    }

    const params: Record<string, string> = {
      q: query,
      format: 'json',
      addressdetails: '1',
      limit: '8',
      'accept-language': 'it'
    };

    // Se specificato un paese, limita la ricerca
    if (countryCode) {
      params['countrycodes'] = countryCode;
    }

    return this.http.get<NominatimResult[]>(`${this.baseUrl}/search`, { params }).pipe(
      map(results => results.map(r => this.parseResult(r))),
      catchError(err => {
        console.error('Errore Nominatim:', err);
        return of([]);
      })
    );
  }

  /**
   * Geocoding inverso: da coordinate a indirizzo
   */
  reverse(lat: number, lon: number): Observable<ParsedAddress | null> {
    const params = {
      lat: lat.toString(),
      lon: lon.toString(),
      format: 'json',
      addressdetails: '1',
      'accept-language': 'it'
    };

    return this.http.get<NominatimResult>(`${this.baseUrl}/reverse`, { params }).pipe(
      map(result => this.parseResult(result)),
      catchError(() => of(null))
    );
  }

  private parseResult(result: NominatimResult): ParsedAddress {
    const addr = result.address;
    
    // Costruisci via con numero civico
    let street = '';
    if (addr.road) {
      street = addr.road;
      if (addr.house_number) {
        street += ', ' + addr.house_number;
      }
    }

    // Città: può essere city, town, village o municipality
    const city = addr.city || addr.town || addr.village || addr.municipality || '';

    // Provincia: county in Italia
    const province = this.extractProvinceCode(addr.county || '');

    // Regione: state in Italia
    const region = addr.state || '';

    return {
      street,
      city,
      province,
      region,
      postalCode: addr.postcode || '',
      country: addr.country || '',
      countryCode: (addr.country_code || '').toUpperCase(),
      latitude: parseFloat(result.lat),
      longitude: parseFloat(result.lon),
      displayName: result.display_name
    };
  }

  /**
   * Estrae il codice provincia (es. "Roma" -> "RM")
   */
  private extractProvinceCode(county: string): string {
    // Mappa province italiane comuni
    const provinceMap: Record<string, string> = {
      'roma': 'RM', 'milano': 'MI', 'napoli': 'NA', 'torino': 'TO',
      'palermo': 'PA', 'genova': 'GE', 'bologna': 'BO', 'firenze': 'FI',
      'bari': 'BA', 'catania': 'CT', 'venezia': 'VE', 'verona': 'VR',
      'messina': 'ME', 'padova': 'PD', 'trieste': 'TS', 'taranto': 'TA',
      'brescia': 'BS', 'parma': 'PR', 'modena': 'MO', 'reggio calabria': 'RC',
      'reggio emilia': 'RE', 'perugia': 'PG', 'livorno': 'LI', 'ravenna': 'RA',
      'cagliari': 'CA', 'foggia': 'FG', 'rimini': 'RN', 'salerno': 'SA',
      'ferrara': 'FE', 'sassari': 'SS', 'latina': 'LT', 'bergamo': 'BG',
      'forlì-cesena': 'FC', 'vicenza': 'VI', 'terni': 'TR', 'trento': 'TN',
      'novara': 'NO', 'piacenza': 'PC', 'ancona': 'AN', 'andria': 'BT',
      'udine': 'UD', 'arezzo': 'AR', 'la spezia': 'SP', 'monza': 'MB',
      'pescara': 'PE', 'lecce': 'LE', 'alessandria': 'AL', 'bolzano': 'BZ',
      'catanzaro': 'CZ', 'pistoia': 'PT', 'lucca': 'LU', 'pisa': 'PI',
      'brindisi': 'BR', 'como': 'CO', 'cosenza': 'CS', 'pesaro': 'PU',
      'varese': 'VA', 'grosseto': 'GR', 'prato': 'PO', 'ragusa': 'RG',
      'siracusa': 'SR', 'caserta': 'CE', 'l\'aquila': 'AQ', 'asti': 'AT',
      'cremona': 'CR', 'pavia': 'PV', 'mantova': 'MN', 'massa-carrara': 'MS',
      'cuneo': 'CN', 'teramo': 'TE', 'treviso': 'TV', 'siena': 'SI',
      'chieti': 'CH', 'belluno': 'BL', 'pordenone': 'PN', 'rovigo': 'RO',
      'avellino': 'AV', 'benevento': 'BN', 'campobasso': 'CB', 'enna': 'EN',
      'trapani': 'TP', 'agrigento': 'AG', 'caltanissetta': 'CL', 'frosinone': 'FR',
      'rieti': 'RI', 'viterbo': 'VT', 'savona': 'SV', 'imperia': 'IM',
      'biella': 'BI', 'vercelli': 'VC', 'verbania': 'VB', 'aosta': 'AO',
      'sondrio': 'SO', 'lecco': 'LC', 'lodi': 'LO', 'gorizia': 'GO',
      'macerata': 'MC', 'fermo': 'FM', 'ascoli piceno': 'AP', 'isernia': 'IS',
      'matera': 'MT', 'potenza': 'PZ', 'crotone': 'KR', 'vibo valentia': 'VV',
      'nuoro': 'NU', 'oristano': 'OR', 'sud sardegna': 'SU'
    };

    const normalized = county.toLowerCase().trim();
    
    // Prima cerca corrispondenza esatta
    if (provinceMap[normalized]) {
      return provinceMap[normalized];
    }

    // Cerca corrispondenza parziale
    for (const [name, code] of Object.entries(provinceMap)) {
      if (normalized.includes(name) || name.includes(normalized)) {
        return code;
      }
    }

    // Se non trova, ritorna le prime 2 lettere maiuscole
    return county.substring(0, 2).toUpperCase();
  }
}
