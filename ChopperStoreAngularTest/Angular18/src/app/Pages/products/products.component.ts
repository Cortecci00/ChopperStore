import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Category, Skin } from '../../Interfaces';
import { CategoryService } from '../../Services/category.service';
import { SkinService } from '../../Services/skin.service';
import { ShoppingCartService } from '../../Services/shopping-cart.service';
import { AuthServiceTsService } from '../../Services/auth.service.ts.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss',
})
export class ProductsComponent implements OnInit {
  skins: Skin[] = [];
  categories: Category[] = [];
  selectedCategoryId: number | null = null;
  isLoading = false;

  constructor(
    private _skinService: SkinService,
    private _categoryService: CategoryService,
    private _cartService: ShoppingCartService,
    private _authService: AuthServiceTsService,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.loadCategories();
    this.loadSkins();
  }

  loadCategories() {
    this._categoryService.getAll().subscribe((r) => (this.categories = r.result));
  }

  loadSkins() {
    this.isLoading = true;
    const obs = this.selectedCategoryId
      ? this._skinService.getByCategory(this.selectedCategoryId, true)
      : this._skinService.getAll(true);

    obs.subscribe({
      next: (r) => {
        this.skins = r.result;
        this.isLoading = false;
      },
      error: () => (this.isLoading = false),
    });
  }

  onCategoryChange(id: number | null) {
    this.selectedCategoryId = id;
    this.loadSkins();
    window.scrollTo(0, 0);
  }

  goToDetail(id: number) {
    this._router.navigate(['/product', id]);
  }

  addToCart(skin: Skin, event: Event) {
    event.stopPropagation();
    if (!this._authService.isLoggedIn()) {
      this._router.navigate(['/login']);
      return;
    }
    this._cartService.addItem({ skinId: skin.id, quantity: 1 }).subscribe({
      next: () =>
        this._snackBar.open('Agregado al carrito', 'Cerrar', {
          duration: 2000,
          panelClass: ['snackbar-success'],
        }),
      error: (err) => {
        this._snackBar.open(err.error?.message ?? 'No se pudo agregar al carrito', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-error'],
        });
        this.loadSkins();
      },
    });
  }
}
