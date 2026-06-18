import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Recommendation } from '../../Interfaces';
import { RecommendationService } from '../../Services/recommendation.service';
import { AuthServiceTsService } from '../../Services/auth.service.ts.service';
import { RecommendationDialogComponent } from './recommendation-dialog/recommendation-dialog.component';

@Component({
  selector: 'app-recommendation',
  templateUrl: './recommendation.component.html',
  styleUrl: './recommendation.component.scss',
})
export class RecommendationComponent implements OnInit {
  recommendations: Recommendation[] = [];
  currentUserId: number | null;
  isAdmin: boolean;

  constructor(
    private _recommendationService: RecommendationService,
    private _dialog: MatDialog,
    private _authService: AuthServiceTsService,
    private _router: Router,
    private _snackBar: MatSnackBar
  ) {
    this.currentUserId = this._authService.getCurrentUserId();
    this.isAdmin = this._authService.getIsAdmin();
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this._recommendationService.getAll().subscribe((r) => (this.recommendations = r.result));
  }

  openCreateDialog() {
    if (!this._authService.isLoggedIn()) {
      this._router.navigate(['/login']);
      return;
    }

    const ref = this._dialog.open(RecommendationDialogComponent);
    ref.afterClosed().subscribe((text: string | undefined) => {
      if (!text) return;
      this._recommendationService.create(text).subscribe({
        next: () => {
          this.load();
          this._snackBar.open('Recomendación creada', 'Cerrar', {
            duration: 2000,
            panelClass: ['snackbar-success'],
          });
        },
        error: () =>
          this._snackBar.open('No se pudo crear la recomendación', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-error'],
          }),
      });
    });
  }

  canDelete(rec: Recommendation): boolean {
    return this.isAdmin || rec.usuario.id === this.currentUserId;
  }

  deleteRecommendation(id: number) {
    this._recommendationService.delete(id).subscribe(() => this.load());
  }
}
