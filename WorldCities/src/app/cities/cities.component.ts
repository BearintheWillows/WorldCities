import {HttpClient, HttpParams} from '@angular/common/http';
import {Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import {environment} from 'src/environments/environment';
import {City} from './city';
import {MatSort} from "@angular/material/sort";
import {query} from "@angular/animations";

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})

export class CitiesComponent implements OnInit {

  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery?: string;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private http: HttpClient) {
  }

  ngOnInit(){
    this.loadData(null); // load data with default filter
  }


  loadData(query? : string) { // load data with filter
    var pageEvent = new PageEvent(); // create a new PageEvent
    pageEvent.pageIndex = this.defaultPageIndex; // set the pageIndex
    pageEvent.pageSize = this.defaultPageSize; // set the pageSize
    this.filterQuery = query; // set the filterQuery
    this.getData(pageEvent); // get the data
  }
  // get data from server
  getData(event: PageEvent) {
    var url = environment.baseUrl + 'api/Cities'; // base url
    var params = new HttpParams() // create a new HttpParams
      .set("pageIndex", event.pageIndex.toString()) // set the pageIndex
      .set("pageSize", event.pageSize.toString()) // set the pageSize
      .set("sortColumn", (this.sort) ? this.sort.active : this.defaultSortColumn) // set the sortColumn
      .set("sortOrder", (this.sort) ? this.sort.direction : this.defaultSortOrder); // set the sortOrder
    if (this.filterQuery) { // if filterQuery is not null
      params = params // if there is a filter query, add it to the params
        .set("filterColumn", this.defaultFilterColumn) // set the filterColumn
        .set("filterQuery", this.filterQuery) // set the filterQuery
    }

    // get the data from the server
    this.http.get<any>(url, {params})
      .subscribe(result => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.cities = new MatTableDataSource<City>(result.data);
      }, error => console.error(error));
  }

}
