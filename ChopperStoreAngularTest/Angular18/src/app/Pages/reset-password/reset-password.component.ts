import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../../Services/users.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordComponent {
  hide = signal(true);
  token: string | null = null;
  tokenPresente = signal(true);

  password = new FormControl('', [Validators.required, Validators.minLength(5)]);

  formReset = new FormGroup({
    password: this.password,
  });

  guardando = signal(false);

  constructor(
    private _usersService: UsersService,
    private _route: ActivatedRoute,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {
    this.token = this._route.snapshot.queryParamMap.get('token');
    this.tokenPresente.set(!!this.token);
  }

  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  guardar() {
    if (this.password.invalid || !this.token) return;

    this.guardando.set(true);
    this._usersService.resetPassword(this.token, this.password.value!).subscribe({
      next: (resp) => {
        this.guardando.set(false);
        if (resp.isSuccess) {
          this._snackBar.open('Contraseña actualizada, ya podés iniciar sesión', 'Cerrar', { duration: 4000 });
          this._router.navigate(['/login']);
        }
      },
      error: (err) => {
        this.guardando.set(false);
        this._snackBar.open(err?.error?.message ?? 'El link es inválido o expiró', 'Cerrar', { duration: 4000 });
      }
    });
  }
}
