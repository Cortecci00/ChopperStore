import { Component } from '@angular/core';
import { IconSetService } from '@coreui/icons-angular';
import { cilListNumbered, cilPaperPlane, brandSet } from '@coreui/icons';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ChopperStore';
  constructor(
    public iconSet: IconSetService
  ) {
    iconSet.icons = { cilListNumbered, cilPaperPlane, ...brandSet };
  }
}

