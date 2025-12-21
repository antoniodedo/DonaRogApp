import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DonorService} from '@proxy/donors';
import { DonorDto } from '@proxy/donors/dto';

@Component({
  selector: 'app-donor-details',
  standalone: false,
  templateUrl: './donor-details.component.html',
  styleUrl: './donor-details.component.scss'
})
export class DonorDetailsComponent implements OnInit {
  donorId: string;
 
  constructor(private route: ActivatedRoute,
            private donorService: DonorService) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.donorId = id;
  }
}
