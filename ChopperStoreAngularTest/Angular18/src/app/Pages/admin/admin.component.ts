import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Category, Skin, SteamInventoryItem, Transaction } from '../../Interfaces';
import { CategoryService } from '../../Services/category.service';
import { SkinService } from '../../Services/skin.service';
import { SteamService } from '../../Services/steam.service';
import { TransactionService } from '../../Services/transaction.service';

const MAX_PHOTO_SIZE_BYTES = 2 * 1024 * 1024;

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements OnInit {
  categories: Category[] = [];
  skins: Skin[] = [];

  formCategory: FormGroup;
  formSkin: FormGroup;
  editingCategoryId: number | null = null;
  editingSkinId: number | null = null;
  skinPhotoPreview: string | null = null;

  selectedTabIndex = 0;
  steamItems: SteamInventoryItem[] = [];
  loadingSteamInventory = false;

  transactionsDataSource = new MatTableDataSource<Transaction>([]);
  transactionColumns = ['date', 'user', 'tradeUrl', 'items', 'total', 'status', 'actions'];

  @ViewChild('skinPhotoInput') skinPhotoInput!: ElementRef<HTMLInputElement>;

  constructor(
    private _fb: FormBuilder,
    private _categoryService: CategoryService,
    private _skinService: SkinService,
    private _steamService: SteamService,
    private _transactionService: TransactionService,
    private _snackBar: MatSnackBar
  ) {
    this.formCategory = this._fb.group({
      name: ['', Validators.required],
    });

    this.formSkin = this._fb.group({
      name: ['', Validators.required],
      skinFloat: [0, [Validators.required, Validators.min(0), Validators.max(1)]],
      pattern: [0, Validators.required],
      rarity: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null, Validators.required],
      photoUrl: [null],
      inspectLink: [null],
    });
  }

  ngOnInit() {
    this.loadCategories();
    this.loadSkins();
    this.loadTransactions();
  }

  loadCategories() {
    this._categoryService.getAll().subscribe((r) => (this.categories = r.result));
  }

  loadSkins() {
    this._skinService.getAll().subscribe((r) => (this.skins = r.result));
  }

  submitCategory() {
    if (this.formCategory.invalid) return;
    const obs = this.editingCategoryId
      ? this._categoryService.update(this.editingCategoryId, this.formCategory.value)
      : this._categoryService.create(this.formCategory.value);

    obs.subscribe({
      next: () => {
        this.resetCategoryForm();
        this.loadCategories();
      },
      error: this.onError,
    });
  }

  editCategory(c: Category) {
    this.editingCategoryId = c.id;
    this.formCategory.patchValue(c);
  }

  resetCategoryForm() {
    this.editingCategoryId = null;
    this.formCategory.reset();
  }

  deleteCategory(id: number) {
    this._categoryService.delete(id).subscribe({
      next: () => this.loadCategories(),
      error: this.onError,
    });
  }

  submitSkin() {
    if (this.formSkin.invalid) return;
    const obs = this.editingSkinId
      ? this._skinService.update(this.editingSkinId, this.formSkin.value)
      : this._skinService.create(this.formSkin.value);

    obs.subscribe({
      next: () => {
        this.resetSkinForm();
        this.loadSkins();
      },
      error: this.onError,
    });
  }

  editSkin(s: Skin) {
    this.editingSkinId = s.id;
    this.formSkin.patchValue({ ...s, categoryId: s.category?.id, inspectLink: s.inspectLink ?? null });
    this.skinPhotoPreview = s.photoUrl ?? null;
  }

  resetSkinForm() {
    this.editingSkinId = null;
    this.formSkin.reset();
    this.skinPhotoPreview = null;
  }

  triggerSkinPhotoInput() {
    this.skinPhotoInput.nativeElement.click();
  }

  onSkinPhotoSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    if (file.size > MAX_PHOTO_SIZE_BYTES) {
      this._snackBar.open('La foto no puede superar los 2MB', 'Cerrar', {
        duration: 3000,
        panelClass: ['snackbar-warning'],
      });
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string;
      this.skinPhotoPreview = base64;
      this.formSkin.patchValue({ photoUrl: base64 });
    };
    reader.readAsDataURL(file);
  }

  loadSteamInventory() {
    this.loadingSteamInventory = true;
    this._steamService.getInventory().subscribe({
      next: (r) => {
        this.steamItems = [...r.result].sort((a, b) => (b.tradable ? 1 : 0) - (a.tradable ? 1 : 0));
        this.loadingSteamInventory = false;
      },
      error: (err) => {
        this.loadingSteamInventory = false;
        this.onError(err);
      },
    });
  }

  useSteamItem(item: SteamInventoryItem) {
    if (!item.tradable) return;

    this.editingSkinId = null;
    this.formSkin.patchValue({
      name: item.name,
      rarity: item.rarity ?? '',
      skinFloat: item.skinFloat ?? 0,
      pattern: item.pattern ?? 0,
      photoUrl: item.photoUrl ?? null,
      inspectLink: item.inspectLink ?? null,
    });
    this.skinPhotoPreview = item.photoUrl ?? null;
    this.selectedTabIndex = 1;
  }

  loadTransactions() {
    this._transactionService.getAll().subscribe({
      next: (r) => (this.transactionsDataSource.data = r.result),
      error: this.onError,
    });
  }

  getTransactionStatus(t: Transaction): string {
    if (t.paymentStatus === 'rejected') return 'Rechazado';
    if (t.paymentStatus === 'pending') return 'Pago pendiente';
    if (t.deliveryStatus === 'delivered') return 'Entregado';
    return 'Esperando entrega';
  }

  getStatusColor(t: Transaction): string {
    if (t.paymentStatus === 'rejected') return '#c62828';
    if (t.paymentStatus === 'pending') return '#b8860b';
    if (t.deliveryStatus === 'delivered') return '#2e7d32';
    return '#1565c0';
  }

  markDelivered(t: Transaction) {
    this._transactionService.markDelivered(t.id).subscribe({
      next: () => {
        t.deliveryStatus = 'delivered';
        this._snackBar.open('Marcada como entregada', 'Cerrar', { duration: 2000, panelClass: ['snackbar-success'] });
      },
      error: this.onError,
    });
  }

  copyTradeUrl(url: string) {
    navigator.clipboard.writeText(url).then(() => {
      this._snackBar.open('Trade URL copiada', 'Cerrar', { duration: 2000 });
    });
  }

  getItemNames(t: Transaction): string {
    return t.items.map(i => i.skinName).join(', ');
  }

  deleteSkin(id: number) {
    this._skinService.delete(id).subscribe({
      next: () => this.loadSkins(),
      error: this.onError,
    });
  }

  private onError = (err: HttpErrorResponse) => {
    this._snackBar.open(err.error?.message ?? 'Ocurrió un error', 'Cerrar', {
      duration: 3000,
      panelClass: ['snackbar-error'],
    });
  };
}
