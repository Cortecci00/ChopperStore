import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Item, ShoppingCart } from '../../Interfaces';
import { ShoppingCartService } from '../../Services/shopping-cart.service';
import { TransactionService } from '../../Services/transaction.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit, AfterViewInit {
  displayedColumns = ['skin', 'price', 'quantity', 'subtotal', 'actions'];
  dataSource = new MatTableDataSource<Item>([]);
  cart: ShoppingCart | null = null;
  total = 0;
  isLoading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private _cartService: ShoppingCartService,
    private _transactionService: TransactionService,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  loadCart() {
    this.isLoading = true;
    this._cartService.getMine().subscribe({
      next: (r) => {
        this.cart = r.result;
        this.dataSource.data = r.result.items;
        this.recalcTotal();
        this.isLoading = false;
      },
      error: () => (this.isLoading = false),
    });
  }

  recalcTotal() {
    this.total = this.cart?.items.reduce((acc, i) => acc + i.skin.price * i.quantity, 0) ?? 0;
  }

  updateQuantity(item: Item, quantity: number) {
    if (quantity < 1) return;
    this._cartService.updateItem(item.id, { quantity }).subscribe(() => this.loadCart());
  }

  removeItem(itemId: number) {
    this._cartService.removeItem(itemId).subscribe(() => this.loadCart());
  }

  clearCart() {
    this._cartService.clear().subscribe(() => this.loadCart());
  }

  checkout() {
    if (!this.cart || this.cart.items.length === 0) {
      this._snackBar.open('El carrito está vacío', 'Cerrar', {
        duration: 3000,
        panelClass: ['snackbar-warning'],
      });
      return;
    }

    const confirmado = window.confirm(`¿Confirmás la compra por $${this.total}?`);
    if (!confirmado) return;

    this._transactionService.checkout().subscribe({
      next: () => {
        this._snackBar.open('Compra realizada', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-success'],
        });
        this._router.navigate(['/profile']);
      },
      error: () =>
        this._snackBar.open('No se pudo completar la compra', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-error'],
        }),
    });
  }
}
