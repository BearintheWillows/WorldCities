import {Component, OnInit} from '@angular/core';
import {AbstractControl, AsyncValidatorFn, FormControl, FormGroup, Validators} from "@angular/forms";
import {City} from "./city";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {environment} from "../../environments/environment";
import {Country} from "../countries/country";
import {map, Observable, Subscription} from "rxjs";
import {BaseFormComponent} from "../base-form.component";
import {CityService} from "./Services/city.services";

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent
  extends BaseFormComponent implements OnInit {

  // the view title
  title?: string;

  // the city object to edit or create
  city?: City;
  /*the city object id, as received from the active route
  It's NULL when we're ading a new city
  and not bull when we're editing*/

  id?: number;

  // The countries array for select
  countries?: Country[];

  // Activity Log (for debugging)
  activityLog: string = "";

  private subscription: Subscription = new Subscription();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService)
   {
    super()
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name     : new FormControl('', Validators.required),
      lat      : new FormControl('', [Validators.required,
        Validators.pattern(/^-?\d+(\.\d{1,4})?$/)
      ]),
      lon      : new FormControl('', [Validators.required,
        Validators.pattern(/^-?\d+(\.\d{1,4})?$/)
      ]),
      countryId: new FormControl('', Validators.required),
    }, null, this.isDupeCity())


    //react to form changes
    this.subscription.add(this.form.valueChanges.subscribe(() => {
      if(!this.form.dirty) {
        this.log("Form Model has been loaded");
      } else {
        this.log("Form updated by User")
      }
    }));

    //React to changes in form.name Control
    this.subscription.add(this.form.get("name")!.valueChanges.subscribe(() => {
      if (!this.form.dirty) {
      this.log("Name has been loaded with initial values");
    } else {
      this.log("Name updated by user");
    }
    }));

    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  log(str: string) {
    this.activityLog += "[" + new Date().toLocaleString() + "] " + str + "<br />";
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      return this.cityService.isDupeCity(city).pipe(map(result => {
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
      this.cityService.get(this.id).subscribe(result => {
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
        this.cityService.put(city).subscribe(result => {
          console.log(`City updated: ${city?.name}`);
          //go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      } else {
        //ADD Mode
       this.cityService.post(city).subscribe(result => {
          console.log(`City ${city?.name} has been created`);

          //go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      }
    }
  }

  loadCountries() {
    //Fet all countries from server
    this.cityService.getCountries(
      0,
      9999,
      "name",
      "asc",
      null,
      null
    ).subscribe(result => {
      this.countries = result.data;
  }, error => console.error(error));
}
}
