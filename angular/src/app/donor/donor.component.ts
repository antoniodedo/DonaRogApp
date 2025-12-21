/* eslint-disable @angular-eslint/prefer-standalone */
import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { DonorService} from '@proxy/donors';
import { DonorDto } from '@proxy/donors/dto';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-donor',
  standalone: false,
  templateUrl: './donor.component.html',
  styleUrls: ['./donor.component.scss'],
  providers:[ListService]
})
export class DonorComponent implements OnInit {
  donors = { items: [], totalCount: 0 } as PagedResultDto<DonorDto>;
  form: FormGroup;

  selectedDonor = {} as DonorDto; 
  isModalOpen = false; 

  constructor(
    public readonly list: ListService, 
    private donorService: DonorService, 
    private router: Router, 
    private fb: FormBuilder) {}

  ngOnInit() {
    const donorStreamCreator = (query) => this.donorService.getList(query);

    this.list.hookToQuery(donorStreamCreator).subscribe((response) => {
      this.donors = response;
    });
  }

  openDetail(id: string) {
    this.router.navigate(['/donors/donor-details', id]);
  }

  createDonor() {
    this.buildForm(); 
    this.isModalOpen = true;
  }

   editDonor(id: string) {
    this.donorService.get(id).subscribe((donor) => {
      this.selectedDonor = donor;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      firstName: [this.selectedDonor.firstName || '', Validators.required],
      lastName: [this.selectedDonor.lastName || '', Validators.required],
      rawAddress: [this.selectedDonor.rawAddress || '', Validators.required],
      rawComune: [this.selectedDonor.rawComune || '', Validators.required],
      rawCap: [this.selectedDonor.rawCap || '', Validators.required],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedDonor.id
      ? this.donorService.update(this.selectedDonor.id, this.form.value)
      : this.donorService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }
}

