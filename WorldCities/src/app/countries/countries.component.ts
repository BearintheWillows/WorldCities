import {Component, OnInit, ViewChild} from '@angular/core';
import {Country} from "./country";
import {MatTableDataSource} from "@angular/material/table";
import {HttpClient, HttpParams} from "@angular/common/http";
import {MatSort} from "@angular/material/sort";
import {MatPaginator, PageEvent} from "@angular/material/paginator";
import {environment} from "../../environments/environment";
import {debounceTime, distinctUntilChanged, Subject} from "rxjs";
import {CountryService} from "./Services/country.service";



@Component({ // create a new Component
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss']
})

export class CountriesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3', 'totalCities']; // displayed columns
  public countries!: MatTableDataSource<Country>;
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private countryService: CountryService) {
  }

  ngOnInit(): void {
    this.loadData(); // load data with default filter
  }

  //debounce filter text changes
  onFilterTextChanged(filterText: string) {
    if(this.filterTextChanged.observers.length === 0) {
      this.filterTextChanged.pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }

  loadData(query?: string) { // load data with filter
    let pageEvent = new PageEvent(); // create a new PageEvent
    pageEvent.pageIndex = this.defaultPageIndex; // set the pageIndex
    pageEvent.pageSize = this.defaultPageSize; // set the pageSize
    this.filterQuery = query; // set the filterQuery
    this.getData(pageEvent); // get the data
  }

  getData(event: PageEvent) {
    var sortColumn = this.sort ? this.sort.active : this.defaultSortColumn;
    var sortOrder = this.sort ? this.sort.direction : this.defaultSortOrder;
    var filterColumn = this.filterQuery ? this.defaultFilterColumn : null;
    var filterQuery = this.filterQuery ? this.filterQuery : null;

    this.countryService.getData(event.pageIndex, event.pageSize, sortColumn, sortOrder, filterColumn, filterQuery)
      .subscribe(result => {
        this.countries = new MatTableDataSource(result.data); // set the data
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
    }, error => console.log(error));

  }

}
