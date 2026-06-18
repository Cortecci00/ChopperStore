import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { IconSetService } from '@coreui/icons-angular';
import { cilListNumbered, cilPaperPlane, brandSet } from '@coreui/icons';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { AuthServiceTsService } from './Services/auth.service.ts.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ChopperStore';
  usuarioLogueado = false;
  isAdmin = false;

  constructor(
    public iconSet: IconSetService,
    private _authService: AuthServiceTsService,
    private _router: Router,
    @Inject(PLATFORM_ID) private _platformId: Object
  ) {
    iconSet.icons = { cilListNumbered, cilPaperPlane, ...brandSet };
    this.refreshAuthState();

    this._router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe(() => {
      this.refreshAuthState();
      if (isPlatformBrowser(this._platformId)) {
        window.scrollTo(0, 0);
      }
    });
  }

  refreshAuthState() {
    this.usuarioLogueado = this._authService.isLoggedIn();
    this.isAdmin = this._authService.getIsAdmin();
  }

  logout() {
    this._authService.clearToken();
    this.usuarioLogueado = false;
    this.isAdmin = false;
    this._router.navigate(['/login']);
  }
}
