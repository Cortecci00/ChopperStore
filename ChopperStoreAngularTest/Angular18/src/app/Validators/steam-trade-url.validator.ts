import { AbstractControl, ValidationErrors } from '@angular/forms';

const STEAM_TRADE_URL_REGEX =
  /^https:\/\/steamcommunity\.com\/tradeoffer\/new\/\?partner=\d+&token=[A-Za-z0-9_-]{8}$/;

export function steamTradeUrlValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null;
  return STEAM_TRADE_URL_REGEX.test(control.value) ? null : { invalidTradeUrl: true };
}
