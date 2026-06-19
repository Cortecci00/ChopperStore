import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-payment-return',
  templateUrl: './payment-return.component.html',
  styleUrl: './payment-return.component.scss'
})
export class PaymentReturnComponent implements OnInit {
  mensaje = 'Procesando tu pago...';

  constructor(
    private _route: ActivatedRoute,
    private _router: Router
  ) {}

  ngOnInit(): void {
    const status = this._route.snapshot.queryParamMap.get('status');

    if (status === 'failure') {
      this.mensaje = 'El pago no se pudo completar.';
    } else {
      this.mensaje = '¡Listo! Revisá el estado de tu compra.';
    }

    setTimeout(() => {
      this._router.navigate([status === 'failure' ? '/cart' : '/profile']);
    }, 1500);
  }
}
