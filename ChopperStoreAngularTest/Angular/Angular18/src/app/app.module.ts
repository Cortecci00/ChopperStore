import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { FlexLayoutModule } from '@angular/flex-layout';
import { IconModule, IconSetService } from '@coreui/icons-angular';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProductsComponent } from './products/products.component';
import { ProductComponent } from './product/product.component';
import { CartComponent } from './cart/cart.component';
import { UsComponent } from './us/us.component';
import { ContactComponent } from './contact/contact.component';
import { RecommendationComponent } from './recommendation/recommendation.component';
import { ProfileComponent } from './profile/profile.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    ProductsComponent,
    ProductComponent,
    CartComponent,
    UsComponent,
    ContactComponent,
    RecommendationComponent,
    ProfileComponent,
  ],
  imports: [
    MatGridListModule,
    MatCardModule,
    ReactiveFormsModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule,
    IconModule,
    FlexLayoutModule,
    MatDividerModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    BrowserModule,
    AppRoutingModule
  ],
  providers: [
    IconSetService,
    provideClientHydration(),
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }


