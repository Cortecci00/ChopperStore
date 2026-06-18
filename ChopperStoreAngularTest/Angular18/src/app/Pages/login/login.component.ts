import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { merge } from 'rxjs';
import { FormControl, Validators, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../../Services/users.service';
import { AuthServiceTsService } from '../../Services/auth.service.ts.service';

declare const google: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  hide = signal(true);

  email = new FormControl('', [Validators.required, Validators.email]);
  password = new FormControl('', [Validators.required]);

  errorMessage = signal('');

  formLogin = new FormGroup({
    email: this.email,
    password: this.password,
  });

  constructor(
    private _usersService: UsersService,
    private _authService: AuthServiceTsService,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {
    merge(this.email.statusChanges, this.email.valueChanges)
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.updateErrorMessage());
  }

  // Mostrar/ocultar contraseña
  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  // Actualizar mensajes de error
  updateErrorMessage() {
    if (this.email.hasError('required')) {
      this.errorMessage.set('Debes ingresar un valor');
    } else if (this.email.hasError('email')) {
      this.errorMessage.set('No es un email valido');
    } else {
      this.errorMessage.set('');
    }
  }

  // Login con email y contraseña
  login() {
    if (this.email.invalid || this.password.invalid) return;

    this._usersService.loginUser({
      email: this.email.value!,
      password: this.password.value!
    }).subscribe({
      next: (resp) => {
        if (resp.isSuccess) {
          localStorage.setItem('user', JSON.stringify(resp.result.user));
          this._authService.setToken(resp.result.token);
          this._router.navigate(['/home']);
        }
      },
      error: () => {
        this._snackBar.open("Email o contraseña incorrectos", "Cerrar", { duration: 3000 });
      }
    });
  }

  // Login con Google
  async handleCredentialResponse(response: any) {
    try {
      const googleToken = response.credential;

      // Llamada al servicio para hacer login con Google
      this._usersService.loginWithGoogle(googleToken).subscribe({
        next: (resp) => {
          if (resp.isSuccess) {
            localStorage.setItem('user', JSON.stringify(resp.result.user));
            this._authService.setToken(resp.result.token);
            this._router.navigate(['/home']);
          }
        },
        error: () => {
          this._snackBar.open("Hubo un error al intentar iniciar sesión con Google", "Cerrar", { duration: 3000 });
        }
      });
    } catch (error) {
      this._snackBar.open("Error en la autenticación con Google", "Cerrar", { duration: 3000 });
    }
  }

  ngOnInit(): void {
    // Esperar que el DOM esté listo
    setTimeout(() => {
      google.accounts.id.initialize({
        client_id: "864300664450-helt864neq6oqb3hcs8b6iso32rcm2fg.apps.googleusercontent.com",
        callback: this.handleCredentialResponse.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true,
      });

      google.accounts.id.renderButton(
        document.getElementById("google-button"),
        { theme: "outline", size: "large" }
      );
    }, 0);
  }

}
