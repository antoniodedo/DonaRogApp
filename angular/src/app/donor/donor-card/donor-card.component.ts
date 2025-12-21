/* eslint-disable @angular-eslint/prefer-standalone */
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DonorService, EmailService } from '@proxy/donors';
import { DonorDto, EmailDto } from '@proxy/donors/dto';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';


@Component({
  selector: 'app-donor-card',
  standalone: false,
  templateUrl: './donor-card.component.html',
  styleUrl: './donor-card.component.scss',
  providers: [ListService]
})

export class DonorCardComponent implements OnInit {
  @Input() donorId?: string;
    form: FormGroup;
    selectedEmail = {} as EmailDto; 
    isEmailModalOpen = false; 
  
  donor: DonorDto;
  emails = { items: [], totalCount: 0 } as PagedResultDto<EmailDto>;

  phones = [
    {id: 0, numero: '123456789'},
    {id: 1, numero: '123'}
  ];
  // This component will andle the details of a donor.
  // You can implement the logic to fetch and display donor details here.

  constructor(private route: ActivatedRoute,
            private donorService: DonorService,
            private emailService: EmailService,
            public readonly list: ListService,
            private fb: FormBuilder) {}


  ngOnInit() {

    if (this.donorId) {
      this.donorService.get(this.donorId).subscribe(result => {
        this.donor = result;
      });
      const donorStreamCreator = (query) => this.emailService.getListByDonor(this.donorId!, query);
  
      this.list.hookToQuery(donorStreamCreator).subscribe((response) => {
        this.emails = response;
      });
    }
    // Initialization logic can go here.
  }

createEmail() {
      this.buildFormEmail(); 
      this.isEmailModalOpen = true;
    }
  
     editEmail(id: string) {
      this.emailService.get(id).subscribe((email) => {
        this.selectedEmail = email;
        this.buildFormEmail();
        this.isEmailModalOpen = true;
      });
    }
  
    buildFormEmail() {
      this.form = this.fb.group({
        address: [this.selectedEmail.address || '', Validators.required],
        isPrimary: [this.selectedEmail.isPrimary || '', Validators.required],
      });
    }

  deleteEmail(id) {
  }

  saveEmail() {
      if (this.form.invalid) {
        return;
      }
  
      const request = this.selectedEmail.id
        ? this.emailService.updateByDonor(this.donorId, this.selectedEmail.id, this.form.value)
        : this.emailService.createByDonor(this.donorId, this.form.value);
  
      request.subscribe(() => {
        this.isEmailModalOpen = false;
        this.form.reset();
        this.list.get();
      });
    }

  addPhone() {
  }

  editPhone(id) {
  } 

  deletePhone(id) {
  }

  editDonor() {
    // Logic to edit donor details can be implemented here.
  }
}
