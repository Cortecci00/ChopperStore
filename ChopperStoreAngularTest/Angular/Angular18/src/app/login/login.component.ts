import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { merge } from 'rxjs';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent {
  hide = signal(true);

  readonly email = new FormControl('', [Validators.required, Validators.email]);

  errorMessage = signal('');

  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  constructor() {
    merge(this.email.statusChanges, this.email.valueChanges)
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.updateErrorMessage());
  }

  updateErrorMessage() {
    if (this.email.hasError('required')) {
      this.errorMessage.set('Debes ingresar un valor');
    } else if (this.email.hasError('email')) {
      this.errorMessage.set('No es un email valido');
    } else {
      this.errorMessage.set('');
    }
  }

  async handleCredentialResponse(response: any) {
    // Here will be your response from Google.
    console.log(response);
  }

  ngOnInit() {
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: "864300664450-helt864neq6oqb3hcs8b6iso32rcm2fg.apps.googleusercontent.com",
      callback: this.handleCredentialResponse.bind(this),
      auto_select: false,
      cancel_on_tap_outside: true,

    });
    // @ts-ignore
    google.accounts.id.renderButton(
      // @ts-ignore
      document.getElementById("google-button"),
      { theme: "outline", height:"300", width: "300px" }
    );
    // @ts-ignore
    google.accounts.id.prompt((notification: PromptMomentNotification) => { });
  }  

}


