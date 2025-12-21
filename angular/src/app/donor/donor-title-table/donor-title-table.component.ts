import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { DonorTitleService} from '@proxy/donors/donor-title.service';
import { DonorTitleDto } from '@proxy/donors/dto';
import {genderOptions} from '@proxy/enums';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-donor-title-table',
  standalone: false,
  templateUrl: './donor-title-table.component.html',
  styleUrl: './donor-title-table.component.scss',
  providers:[ListService]
})
export class DonorTitleTableComponent implements OnInit {
titles = { items: [], totalCount: 0 } as PagedResultDto<DonorTitleDto>;
  form: FormGroup;

  selectedTitle = {} as DonorTitleDto; 
  genders = genderOptions;
  isModalOpen = false; 

  constructor(
    public readonly list: ListService, 
    private titleService: DonorTitleService,  
    private fb: FormBuilder) {}

  ngOnInit() {
    const titleStreamCreator = (query) => this.titleService.getList(query);

    this.list.hookToQuery(titleStreamCreator).subscribe((response) => {
      this.titles = response;
    });
  }

  createDonor() {
    this.buildForm(); 
    this.isModalOpen = true;
  }

   editTitle(id: string) {
    this.titleService.get(id).subscribe((title) => {
      this.selectedTitle = title;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      title: [this.selectedTitle.title || '', Validators.required],
      gender: [this.selectedTitle.gender || '', Validators.required],
      isGroup: [this.selectedTitle.isGroup || false, Validators.required],
      isActive: [this.selectedTitle.isActive || true, Validators.required]
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedTitle.id
      ? this.titleService.update(this.selectedTitle.id, this.form.value)
      : this.titleService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }
}


