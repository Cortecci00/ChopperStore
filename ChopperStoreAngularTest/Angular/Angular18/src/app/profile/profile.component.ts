import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
export interface Products {
  name: string;
  price: number;
  date: string;
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements AfterViewInit {
  displayedColumns: string[] = ['name', 'price','date'];
  dataSource = new MatTableDataSource<Products>(ELEMENT_DATA);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }
}

const ELEMENT_DATA: Products[] = [
  { name: 'Hydrogen', price: 1.0079, date: '10/06/24' },
  { name: 'Helium', price: 4.0026, date: '10/06/24' },
  { name: 'Lithium', price: 6.941, date: '10/06/24' },
  { name: 'Beryllium', price: 9.0122, date: '10/06/24' },
  { name: 'Boron', price: 10.811, date: '10/06/24' },
  { name: 'Hydrogen', price: 1.0079, date: '10/06/24' },
  { name: 'Helium', price: 4.0026, date: '10/06/24' },
  { name: 'Lithium', price: 6.941, date: '10/06/24' },
  { name: 'Beryllium', price: 9.0122, date: '10/06/24' },
  { name: 'Boron', price: 10.811, date: '10/06/24' },
  { name: 'Hydrogen', price: 1.0079, date: '10/06/24' },
  { name: 'Helium', price: 4.0026, date: '10/06/24' },
  { name: 'Lithium', price: 6.941, date: '10/06/24' },
  { name: 'Beryllium', price: 9.0122, date: '10/06/24' },
  { name: 'Boron', price: 10.811, date: '10/06/24' },
  { name: 'Hydrogen', price: 1.0079, date: '10/06/24' },
  { name: 'Helium', price: 4.0026, date: '10/06/24' },
  { name: 'Lithium', price: 6.941, date: '10/06/24' },
  { name: 'Beryllium', price: 9.0122, date: '10/06/24' },
  { name: 'Boron', price: 10.811, date: '10/06/24' },
];
