import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DonorService } from '../../proxy/donors/donor.service';
import { DonorAddressDto, CreateDonorAddressDto } from '../../proxy/donors/dtos/models';
import { AddressType, addressTypeOptions } from '../../proxy/enums/shared/address-type.enum';
import { NominatimService, ParsedAddress } from '../../services/nominatim.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-donor-addresses',
  standalone: false,
  templateUrl: './donor-addresses.component.html',
  styleUrls: ['./donor-addresses.component.scss']
})
export class DonorAddressesComponent implements OnInit {
  @Input() donorId!: string;

  addresses: DonorAddressDto[] = [];
  loading = false;
  
  // Modal aggiungi indirizzo
  isAddModalVisible = false;
  addForm!: FormGroup;
  saving = false;

  // Autocomplete
  addressSuggestions: ParsedAddress[] = [];
  searchLoading = false;
  searchQuery = '';
  selectedCountryCode = ''; // Vuoto = ricerca globale
  private searchSubject = new Subject<string>();

  // Lista paesi comuni
  countries = [
    { code: '', label: 'Tutti i paesi' },
    { code: 'it', label: '🇮🇹 Italia' },
    { code: 'de', label: '🇩🇪 Germania' },
    { code: 'fr', label: '🇫🇷 Francia' },
    { code: 'es', label: '🇪🇸 Spagna' },
    { code: 'gb', label: '🇬🇧 Regno Unito' },
    { code: 'ch', label: '🇨🇭 Svizzera' },
    { code: 'at', label: '🇦🇹 Austria' },
    { code: 'be', label: '🇧🇪 Belgio' },
    { code: 'nl', label: '🇳🇱 Paesi Bassi' },
    { code: 'pt', label: '🇵🇹 Portogallo' },
    { code: 'us', label: '🇺🇸 Stati Uniti' },
    { code: 'ca', label: '🇨🇦 Canada' },
    { code: 'au', label: '🇦🇺 Australia' },
    { code: 'br', label: '🇧🇷 Brasile' },
    { code: 'ar', label: '🇦🇷 Argentina' }
  ];

  addressTypeOptions = addressTypeOptions;
  AddressType = AddressType;

  addressTypeLabels: Record<number, string> = {
    [AddressType.Home]: 'Residenza',
    [AddressType.Work]: 'Lavoro',
    [AddressType.Billing]: 'Fatturazione',
    [AddressType.Shipping]: 'Spedizione',
    [AddressType.Temporary]: 'Temporaneo',
    [AddressType.Other]: 'Altro'
  };

  constructor(
    private donorService: DonorService,
    private message: NzMessageService,
    private fb: FormBuilder,
    private nominatimService: NominatimService
  ) {}

  ngOnInit(): void {
    this.loadAddresses();
    this.initForm();
    this.setupAddressSearch();
  }

  private setupAddressSearch(): void {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      switchMap(query => {
        if (query.length < 3) {
          this.addressSuggestions = [];
          return [];
        }
        this.searchLoading = true;
        return this.nominatimService.search(query, this.selectedCountryCode || undefined);
      })
    ).subscribe({
      next: (results) => {
        this.addressSuggestions = results;
        this.searchLoading = false;
      },
      error: () => {
        this.searchLoading = false;
        this.addressSuggestions = [];
      }
    });
  }

  onAddressSearch(query: string): void {
    this.searchQuery = query;
    this.searchSubject.next(query);
  }

  onCountryChange(): void {
    // Ri-esegui la ricerca con il nuovo paese
    if (this.searchQuery.length >= 3) {
      this.searchSubject.next(this.searchQuery);
    }
  }

  selectAddress(address: ParsedAddress): void {
    this.addForm.patchValue({
      street: address.street,
      city: address.city,
      province: address.province,
      region: address.region,
      postalCode: address.postalCode,
      country: address.country
    });
    this.addressSuggestions = [];
    this.message.success('Indirizzo compilato automaticamente');
  }

  private initForm(): void {
    this.addForm = this.fb.group({
      street: ['', Validators.required],
      city: ['', Validators.required],
      province: [''],
      region: [''],
      postalCode: ['', Validators.required],
      country: ['Italia', Validators.required],
      type: [AddressType.Home],
      notes: ['']
    });
  }

  loadAddresses(): void {
    this.loading = true;
    this.donorService.getAddresses(this.donorId).subscribe({
      next: (addresses) => {
        this.addresses = addresses;
        this.loading = false;
      },
      error: (err) => {
        this.message.error('Errore nel caricamento degli indirizzi');
        this.loading = false;
        console.error(err);
      }
    });
  }

  openAddModal(): void {
    this.addForm.reset({ 
      type: AddressType.Home,
      country: 'Italia'
    });
    this.addressSuggestions = [];
    this.searchQuery = '';
    this.selectedCountryCode = 'it'; // Default Italia
    this.isAddModalVisible = true;
  }

  closeAddModal(): void {
    this.isAddModalVisible = false;
  }

  addAddress(): void {
    if (this.addForm.invalid) {
      Object.keys(this.addForm.controls).forEach(key => {
        const control = this.addForm.get(key);
        control?.markAsDirty();
        control?.updateValueAndValidity();
      });
      return;
    }

    this.saving = true;
    const dto: CreateDonorAddressDto = this.addForm.value;
    
    this.donorService.addAddress(this.donorId, dto).subscribe({
      next: () => {
        this.message.success('Indirizzo aggiunto');
        this.closeAddModal();
        this.loadAddresses();
        this.saving = false;
      },
      error: (err) => {
        this.message.error('Errore nell\'aggiunta dell\'indirizzo');
        this.saving = false;
        console.error(err);
      }
    });
  }

  setDefault(address: DonorAddressDto): void {
    this.donorService.setDefaultAddress(this.donorId, address.id!).subscribe({
      next: () => {
        this.message.success('Indirizzo predefinito impostato');
        this.loadAddresses();
      },
      error: () => this.message.error('Errore')
    });
  }

  endAddress(address: DonorAddressDto): void {
    this.donorService.endAddress(this.donorId, address.id!).subscribe({
      next: () => {
        this.message.success('Indirizzo terminato');
        this.loadAddresses();
      },
      error: () => this.message.error('Errore')
    });
  }

  getAddressTypeLabel(type: AddressType | undefined): string {
    if (type === undefined) return '-';
    return this.addressTypeLabels[type] || '-';
  }

  formatAddress(address: DonorAddressDto): string {
    const parts = [
      address.street,
      address.city,
      address.province ? `(${address.province})` : null,
      address.postalCode,
      address.country
    ].filter(Boolean);
    return parts.join(', ');
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('it-IT');
  }
}
