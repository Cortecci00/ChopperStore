import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { toDataURL } from 'qrcode';
import { Capacitor } from '@capacitor/core';
import { Browser } from '@capacitor/browser';
import { CheckoutResult } from '../../../Interfaces';
import { TransactionService } from '../../../Services/transaction.service';

const POLL_INTERVAL_MS = 3000;

@Component({
  selector: 'app-checkout-dialog',
  templateUrl: './checkout-dialog.component.html',
  styleUrl: './checkout-dialog.component.scss',
})
export class CheckoutDialogComponent implements OnInit, OnDestroy {
  qrDataUrl: string | null = null;
  status: 'pending' | 'approved' | 'rejected' = 'pending';
  esNativo = Capacitor.isNativePlatform();
  private pollHandle?: ReturnType<typeof setInterval>;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CheckoutResult,
    private _dialogRef: MatDialogRef<CheckoutDialogComponent>,
    private _transactionService: TransactionService
  ) {}

  ngOnInit() {
    if (!this.esNativo) {
      toDataURL(this.data.checkoutUrl).then((url) => (this.qrDataUrl = url));
    }
    this.pollHandle = setInterval(() => this.checkStatus(), POLL_INTERVAL_MS);
  }

  abrirMercadoPago() {
    Browser.open({ url: this.data.checkoutUrl });
  }

  ngOnDestroy() {
    if (this.pollHandle) clearInterval(this.pollHandle);
  }

  checkStatus() {
    this._transactionService.getById(this.data.transactionId).subscribe((r) => {
      if (r.result.paymentStatus !== 'pending') {
        this.status = r.result.paymentStatus;
        if (this.pollHandle) clearInterval(this.pollHandle);
      }
    });
  }

  close() {
    this._dialogRef.close(this.status);
  }
}
