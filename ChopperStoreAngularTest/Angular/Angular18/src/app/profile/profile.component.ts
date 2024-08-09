import { Component } from '@angular/core';

export interface Products {
  name: string;
  price: number;
  date: string;
}

const ELEMENT_DATA: Products[] = [
  { name: 'Hydrogen', price: 1.0079, date: '10/06/24' },
  { name: 'Helium', price: 4.0026, date: '10/06/24' },
  { name: 'Lithium', price: 6.941, date: '10/06/24' },
  { name: 'Beryllium', price: 9.0122, date: '10/06/24' },
  { name: 'Boron', price: 10.811, date: '10/06/24' },
];

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  columns = [
    {
      columnDef: 'name',
      header: 'Name',
      cell: (element: Products) => `${element.name}`,
    },
    {
      columnDef: 'pricet',
      header: 'Price',
      cell: (element: Products) => `${element.price}`,
    },
    {
      columnDef: 'date',
      header: 'Date',
      cell: (element: Products) => `${element.date}`,
    },
  ];
  dataSource = ELEMENT_DATA;
  displayedColumns = this.columns.map(c => c.columnDef);

}
