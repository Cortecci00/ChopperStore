import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ContactService } from '../../Services/contact.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss',
})
export class ContactComponent {
  formContact: FormGroup;
  submitted = false;

  constructor(
    private _fb: FormBuilder,
    private _contactService: ContactService,
    private _snackBar: MatSnackBar
  ) {
    this.formContact = this._fb.group({
      name: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      message: ['', [Validators.required, Validators.maxLength(900)]],
    });
  }

  send() {
    this.submitted = true;
    if (this.formContact.invalid) return;

    this._contactService.create(this.formContact.value).subscribe({
      next: () => {
        this._snackBar.open('Mensaje enviado correctamente', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-success'],
        });
        this.formContact.reset();
        this.submitted = false;
      },
      error: () =>
        this._snackBar.open('No se pudo enviar el mensaje', 'Cerrar', {
          duration: 3000,
          panelClass: ['snackbar-error'],
        }),
    });
  }
}
