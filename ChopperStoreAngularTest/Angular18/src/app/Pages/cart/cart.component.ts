import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Item, ShoppingCart } from '../../Interfaces';
import { ShoppingCartService } from '../../Services/shopping-cart.service';
import { TransactionService } from '../../Services/transaction.service';
import { CheckoutDialogComponent } from './checkout-dialog/checkout-dialog.component';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit, AfterViewInit {
  displayedColumns = ['skin', 'price', 'actions'];
  dataSource = new MatTableDataSource<Item>([]);
  cart: ShoppingCart | null = null;
  total = 0;
  isLoading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private _cartService: ShoppingCartService,
    private _transactionService: TransactionService,
    private _router: Router,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog
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
    this.total = this.cart?.items.reduce((acc, i) => acc + i.skin.price, 0) ?? 0;
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
      next: (r) => {
        const ref = this._dialog.open(CheckoutDialogComponent, {
          data: r.result,
          disableClose: true,
        });
        ref.afterClosed().subscribe((status) => {
          if (status === 'approved') {
            this._router.navigate(['/profile']);
          } else {
            this.loadCart();
          }
        });
      },
      error: () =>
        this._snackBar.open('No se pudo iniciar el pago', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-error'],
        }),
    });
  }
}
