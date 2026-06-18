import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Skin } from '../../Interfaces';
import { SkinService } from '../../Services/skin.service';
import { ShoppingCartService } from '../../Services/shopping-cart.service';
import { AuthServiceTsService } from '../../Services/auth.service.ts.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrl: './product.component.scss',
})
export class ProductComponent implements OnInit {
  skin: Skin | null = null;
  isLoading = true;
  quantity = new FormControl(1, [Validators.required, Validators.min(1)]);

  constructor(
    private _route: ActivatedRoute,
    private _skinService: SkinService,
    private _cartService: ShoppingCartService,
    private _authService: AuthServiceTsService,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    const id = Number(this._route.snapshot.paramMap.get('id'));
    this._skinService.getById(id).subscribe({
      next: (r) => {
        this.skin = r.result;
        this.isLoading = false;
      },
      error: () => this._router.navigate(['/products']),
    });
  }

  addToCart() {
    if (!this._authService.isLoggedIn()) {
      this._router.navigate(['/login']);
      return;
    }
    if (this.quantity.invalid || !this.skin) return;

    this._cartService
      .addItem({ skinId: this.skin.id, quantity: this.quantity.value! })
      .subscribe({
        next: () =>
          this._snackBar.open('Agregado al carrito', 'Cerrar', {
            duration: 2000,
            panelClass: ['snackbar-success'],
          }),
        error: () =>
          this._snackBar.open('No se pudo agregar al carrito', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-error'],
          }),
      });
  }
}
