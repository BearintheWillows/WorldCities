import {HttpClient, HttpParams} from '@angular/common/http';
import {Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import {environment} from 'src/environments/environment';
import {City} from './city';
import {MatSort} from "@angular/material/sort";
import {debounceTime, distinctUntilChanged, Subject} from "rxjs";
import {CityService} from "./Services/city.services";

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})

export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon', 'countryName']; // columns to display
  public cities!: MatTableDataSource<City>;
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(private cityService: CityService) {
  }

  ngOnInit() {
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
    var pageEvent = new PageEvent(); // create a new PageEvent
    pageEvent.pageIndex = this.defaultPageIndex; // set the pageIndex
    pageEvent.pageSize = this.defaultPageSize; // set the pageSize
    this.filterQuery = query; // set the filterQuery
    this.getData(pageEvent); // get the data
  }

  // get data from server
  getData(event: PageEvent) {
    var sortColumn = this.sort ? this.sort.active : this.defaultSortColumn;
    var sortOrder = this.sort ? this.sort.direction : this.defaultSortOrder;
    var filterColumn = this.filterQuery ? this.defaultFilterColumn : null;
    var filterQuery = this.filterQuery ? this.filterQuery : null;
    this.cityService.getData(event.pageIndex, event.pageSize, sortColumn, sortOrder, filterColumn, filterQuery)
      .subscribe(result => {
        this.cities = new MatTableDataSource(result.data); // set the data to the data source
        this.paginator.length = result.totalCount; // set the total count to the paginator
        this.paginator.pageIndex = result.pageIndex; // set the page index to the paginator
        this.paginator.pageSize = result.pageSize; // set the page size to the paginator
      }, error => console.error(error));
  }
}
