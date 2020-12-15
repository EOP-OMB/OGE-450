// routing
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// components
import { HomeComponent } from './home/home.component';
import { AdminComponent } from './admin/admin.component';
import { LoginComponent } from "./security/login/login.component";
import { LogoutComponent } from "./security/logout/logout.component";
import { ErrorComponent } from './error/error.component';
import { PageNotFoundComponent } from './error/page-not-found.component';
import { HealthCheckComponent } from './healthcheck/health-check.component';
import { TrainingComponent } from './training/training.component';

// guards
import { LoggedInGuard } from "./security/logged-in.guard";
import { AdminGuard } from "./security/role.guard";

const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: HomeComponent,
        canActivate: [LoggedInGuard]
    },
    {
        path: 'admin',
        component: AdminComponent,
        canActivate: [LoggedInGuard, AdminGuard],
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'logout',
        component: LogoutComponent,
    },
    {
        path: 'error',
        component: ErrorComponent
    },
    {
        path: 'healthcheck',
        component: HealthCheckComponent
    },
    {
        path: 'training',
        component: TrainingComponent
    },
    {
        path: "**",
        component: PageNotFoundComponent,
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule],
    providers: [],
})

export class AppRoutingModule { }
