import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../../Services/users.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ForgotPasswordComponent {
  email = new FormControl('', [Validators.required, Validators.email]);

  formForgot = new FormGroup({
    email: this.email,
  });

  enviado = signal(false);
  enviando = signal(false);

  constructor(
    private _usersService: UsersService,
    private _snackBar: MatSnackBar
  ) {}

  enviar() {
    if (this.email.invalid) return;

    this.enviando.set(true);
    this._usersService.forgotPassword(this.email.value!).subscribe({
      next: () => {
        this.enviando.set(false);
        this.enviado.set(true);
      },
      error: () => {
        this.enviando.set(false);
        this._snackBar.open('Hubo un error, intentá de nuevo', 'Cerrar', { duration: 3000 });
      }
    });
  }
}
