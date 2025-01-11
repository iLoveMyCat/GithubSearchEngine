import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { SearchComponent } from './components/search/search.component';
import { FavoritesComponent } from './components/favorites/favorites.component';
import { RegisterComponent } from './components/register/register.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'search', component: SearchComponent, canActivate: [AuthGuard] },
  {
    path: 'favorites',
    component: FavoritesComponent,
    canActivate: [AuthGuard],
  },
  { path: '', component: SearchComponent, canActivate: [AuthGuard] },
  { path: '**', component: SearchComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
