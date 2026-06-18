import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Transaction, User } from '../../Interfaces';
import { UsersService } from '../../Services/users.service';
import { AuthServiceTsService } from '../../Services/auth.service.ts.service';
import { TransactionService } from '../../Services/transaction.service';

const MAX_PHOTO_SIZE_BYTES = 2 * 1024 * 1024;

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit, AfterViewInit {
  formProfile: FormGroup;
  formSteam: FormGroup;
  currentUser: User | null = null;
  photoPreview: string | null = null;
  isAdmin = false;

  displayedColumns: string[] = ['date', 'product', 'total', 'status'];
  dataSource = new MatTableDataSource<Transaction>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  constructor(
    private _fb: FormBuilder,
    private _usersService: UsersService,
    private _authService: AuthServiceTsService,
    private _transactionService: TransactionService,
    private _snackBar: MatSnackBar
  ) {
    this.formProfile = this._fb.group({
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
    });

    this.formSteam = this._fb.group({
      steamTradeUrl: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.isAdmin = this._authService.getIsAdmin();

    const id = this._authService.getCurrentUserId();
    if (!id) return;

    this._usersService.getUser(id).subscribe((r) => {
      this.currentUser = r.result;
      this.formProfile.patchValue(r.result);
      this.formSteam.patchValue({ steamTradeUrl: r.result.steamTradeUrl ?? '' });
      this.photoPreview = r.result.photoUrl ?? null;
    });

    this._transactionService.getMine().subscribe((r) => (this.dataSource.data = r.result));
  }

  getItemNames(transaction: Transaction): string {
    return transaction.items.map((i) => i.skinName).join(', ');
  }

  saveSteamTradeUrl() {
    if (this.formSteam.invalid || !this.currentUser) return;

    this._usersService
      .updateSteamTradeUrl(this.currentUser.id, this.formSteam.value.steamTradeUrl)
      .subscribe({
        next: (r) => {
          this.currentUser = r.result;
          this._snackBar.open('Trade URL actualizada', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-success'],
          });
        },
        error: (err) =>
          this._snackBar.open(err.error?.message ?? 'No se pudo actualizar la Trade URL', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-error'],
          }),
      });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  saveProfile() {
    if (this.formProfile.invalid || !this.currentUser) return;

    this._usersService.updateProfile(this.currentUser.id, this.formProfile.value).subscribe({
      next: (r) => {
        this.currentUser = r.result;
        this._snackBar.open('Perfil actualizado', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-success'],
        });
      },
      error: () =>
        this._snackBar.open('No se pudo actualizar el perfil', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-error'],
        }),
    });
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file || !this.currentUser) return;

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
      this.photoPreview = base64;
      this._usersService.updatePhoto(this.currentUser!.id, base64).subscribe({
        next: () =>
          this._snackBar.open('Foto actualizada', 'Cerrar', {
            duration: 2000,
            panelClass: ['snackbar-success'],
          }),
        error: () =>
          this._snackBar.open('No se pudo subir la foto', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-error'],
          }),
      });
    };
    reader.readAsDataURL(file);
  }
}
