import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-recommendation-dialog',
  templateUrl: './recommendation-dialog.component.html',
  styleUrl: './recommendation-dialog.component.scss',
})
export class RecommendationDialogComponent {
  text = new FormControl('', [Validators.required, Validators.maxLength(900)]);

  constructor(private _dialogRef: MatDialogRef<RecommendationDialogComponent>) {}

  cancel() {
    this._dialogRef.close();
  }

  submit() {
    if (this.text.invalid) return;
    this._dialogRef.close(this.text.value);
  }
}
