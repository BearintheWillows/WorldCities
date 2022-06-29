import {Component, OnInit, ViewChild} from '@angular/core';
import {Country} from "./country";
import {MatTableDataSource} from "@angular/material/table";
import {HttpClient, HttpParams} from "@angular/common/http";
import {MatSort} from "@angular/material/sort";
import {MatPaginator, PageEvent} from "@angular/material/paginator";
import {environment} from "../../environments/environment";
import {debounceTime, distinctUntilChanged, Subject} from "rxjs";


@Component({ // create a new Component
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss']
})

export class CountriesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3'];
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

  constructor(private http: HttpClient) {
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
    const url = environment.baseUrl + 'api/Countries'; // base url
    let params = new HttpParams() // create a new HttpParams
      .set("pageIndex", event.pageIndex.toString()) // set the pageIndex
      .set("pageSize", event.pageSize.toString()) // set the pageSize
      .set("sortColumn", (this.sort) ? this.sort.active : this.defaultSortColumn) // set the sortColumn
      .set("sortOrder", (this.sort) ? this.sort.direction : this.defaultSortOrder); // set the sortOrder

    if(this.filterQuery) {
      params = params // if there is a filter query, add it to the params
        .set("filterColumn", this.defaultFilterColumn) // set the filterColumn
        .set("filterQuery", this.filterQuery) // set the filterQuery
    }

    this.http.get<any>(url, {params})
      .subscribe(result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.countries = new MatTableDataSource<Country>(result.data);
      }, error => console.error(error));
  }

}
