import {Component, OnInit} from '@angular/core';
import {AbstractControl, AsyncValidatorFn, FormControl, FormGroup, Validators} from "@angular/forms";
import {City} from "./city";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {environment} from "../../environments/environment";
import {Country} from "../countries/country";
import {map, Observable} from "rxjs";
import {BaseFormComponent} from "../base-form.component";

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent
  extends BaseFormComponent implements OnInit {

  // the view title
  title?: string;

  // the form model
  form!: FormGroup;

  // the city object to edit or create
  city?: City;
  /*the city object id, as received from the active route
  It's NULL when we're ading a new city
  and not bull when we're editing*/

  id?: number;

  // The countries array for select
  countries?: Country[];

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {
    super()
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', Validators.required),
      lon: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required),
    }, null, this.isDupeCity());

    this.loadData();
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      const url = environment.baseUrl + 'api/Cities/IsDupeCity';

      return this.http.post<boolean>(url, city).pipe(map(result => {
        return (result ? {isDupeCity: true} : null);
      }));

    }

  }

  loadData() {
    //Load countries
    this.loadCountries();

    //Retrieve the ID from the 'id' parameter
    let idParam = this.activatedRoute.snapshot.paramMap.get('id');

    //If we have an ID, we're in EDIT mode
    this.id = idParam ? +idParam : 0;
    if(this.id) {
      //EDIT Mode
      //fetch city from server
      let url = environment.baseUrl + 'api/Cities/' + this.id;
      this.http.get<City>(url).subscribe(result => {
        this.city = result;
        this.title = "Edit City " + this.city.name;

        //Update form with city value
        this.form.patchValue(this.city);
      }, error => console.error(error));
    } else {
      this.title = "Add City";
    }
  }

  onSubmit() {
    let city = (this.id) ? this.city : <City>{};
    console.log('Submitting form...');
    if(city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;
      //EDIT Mode
      if(this.id) {
        let url = environment.baseUrl + 'api/Cities/' + city.id;

        this.http.put<City>(url, city).subscribe(result => {
          console.log(`City {city.id} has been updated`);

          //go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      } else {
        //ADD Mode
        let url = environment.baseUrl + 'api/Cities';
        this.http.post<City>(url, city).subscribe(result => {
          console.log(`City ${city?.name} has been created`);

          //go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      }
    }
  }

  loadCountries() {
    //Fet all countries from server
    let url = environment.baseUrl + 'api/Countries';
    let params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name");
    this.http.get<any>(url, {params: params}).subscribe(result => {
      this.countries = result.data;
    }, error => console.error(error));
  }
}
