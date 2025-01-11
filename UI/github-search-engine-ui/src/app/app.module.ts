import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { FormsModule } from '@angular/forms';
import { AuthGuard } from './guards/auth.guard';
import { HttpClientModule } from '@angular/common/http';
import { SearchComponent } from './components/search/search.component';
import { NavComponent } from './components/shared/nav/nav.component';
import { FavoritesComponent } from './components/favorites/favorites.component';
import { RegisterComponent } from './components/register/register.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SearchComponent,
    NavComponent,
    FavoritesComponent,
    RegisterComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, FormsModule, HttpClientModule],
  providers: [AuthGuard],
  bootstrap: [AppComponent],
})
export class AppModule {}
