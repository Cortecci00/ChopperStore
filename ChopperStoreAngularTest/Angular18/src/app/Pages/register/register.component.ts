import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { UsersService } from '../../Services/users.service';
import { CrearActualizar, User } from '../../Interfaces';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterComponent {
  private _activeRoute = inject(ActivatedRoute);
  private _route = inject(Router);
  private _usersService = inject(UsersService);
  private _fb = inject(FormBuilder);
  private _snackBar = inject(MatSnackBar);

  titulo = 'Crear nuevo usuario';
  isLoading = false;
  mensaje = 'Consultando los datos del usuario';
  public formUser: FormGroup;
  public submitted = false;
  public hidePassword = true;

  private crearUser?: CrearActualizar;
  private user?: User;

  constructor() {
    this.formUser = this._fb.group(
      {
        email: ['', [Validators.required, Validators.minLength(2)]],
        username: ['', [Validators.required, Validators.minLength(5)]],
        password: ['', [Validators.required, Validators.minLength(5)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  ngOnInit() {
    const id = this._activeRoute.snapshot.paramMap.get('id');
    if (id) {
      this.isLoading = true;
      this._usersService.getUser(id).subscribe(
        (resp) => {
          if (resp.isSuccess) {
            this.user = resp.result;
            this.titulo = 'Editar datos del usuario';
            this.formUser.patchValue(this.user);
            setTimeout(() => (this.isLoading = false), 1600);
          }
        },
        (error: HttpErrorResponse) => {
          this._showError(error.error.message, error.statusText);
          this.isLoading = false;
          this._route.navigate(['/home']);
        }
      );
    }
  }

  passwordMatchValidator(form: AbstractControl): null | object {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password !== confirmPassword ? { passwordMismatch: true } : null;
  }

  togglePassword(): void {
    this.hidePassword = !this.hidePassword;
  }

  onSubmit() {
    this.submitted = true;

    if (this.formUser.invalid) {
      if (this.formUser.errors?.['passwordMismatch']) {
        this._showWarning('Las contraseñas no coinciden');
      } else {
        this._showWarning('El formulario no es correcto');
      }
      return;
    }

    this.user?.id ? this.editarUser() : this.nuevoUser();
  }

  private nuevoUser() {
    this.crearUser = this.formUser.value;
    if (this.crearUser) {
      this._usersService.postUser(this.crearUser).subscribe(
        (resp) => {
          if (resp.isSuccess) {
            this._showSuccess(resp.message);
            this.formUser.reset();
            this._route.navigate(['/home']);
          }
        },
        (error: HttpErrorResponse) => {
          this._showError(error.error.message, error.statusText);
          this.formUser.reset();
          this._route.navigate(['/home']);
        }
      );
    }
  }

  private editarUser() {
    this.crearUser = this.formUser.value;
    if (this.crearUser) {
      this._usersService.putUser(this.user!.id, this.crearUser).subscribe(
        (resp) => {
          if (resp.isSuccess) {
            this._showSuccess(resp.message);
            this.formUser.reset();
            this._route.navigate(['/home']);
          }
        },
        (error: HttpErrorResponse) => {
          this._showError(error.error.message, error.statusText);
          this.formUser.reset();
          this._route.navigate(['/home']);
        }
      );
    }
  }

  // ✅ Métodos Snackbar
  private _showSuccess(message: string) {
    this._snackBar.open(message, 'Cerrar', {
      duration: 3000,
      panelClass: ['snackbar-success'],
    });
  }

  private _showError(message: string, action: string = 'Error') {
    this._snackBar.open(message, action, {
      duration: 5000,
      panelClass: ['snackbar-error'],
    });
  }

  private _showWarning(message: string) {
    this._snackBar.open(message, 'Atención', {
      duration: 3000,
      panelClass: ['snackbar-warning'],
    });
  }
}
