// routing
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// components
import { HomeComponent } from './home/home.component';
import { MaintenanceComponent } from './home/maintenance/maintenance.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AdminComponent } from './admin/admin.component';
import { LoginComponent } from "./login/login.component";
import { LogoutComponent } from "./logout/logout.component";
import { ErrorComponent } from './error/error.component';
import { PageNotFoundComponent } from './error/page-not-found.component';
import { FormComponent, PreventUnsavedChangesGuard } from './form/form.component';
import { ExtensionRequestComponent } from './extension-request/extension-request.component';
import { HealthCheckComponent } from './healthcheck/health-check.component';

// resolvers
import { OGEForm450Resolver } from './form/resolvers.service';

// guards
import { LoggedInGuard } from "./security/logged-in.guard";
import { MaintenanceGuard } from "./security/maintenance.guard";
import { ReviewerGuard, AdminGuard } from "./security/reviewer.guard";

const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: HomeComponent,
        canActivate: [LoggedInGuard, MaintenanceGuard]
    },
    {
        path: 'maintenance',
        component: MaintenanceComponent,
        canActivate: [LoggedInGuard]
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [LoggedInGuard, ReviewerGuard, MaintenanceGuard],
    },
    {
        path: 'admin',
        component: AdminComponent,
        canActivate: [LoggedInGuard, AdminGuard, MaintenanceGuard],
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
        path: 'form/:id',
        component: FormComponent,
        resolve: {
            form: OGEForm450Resolver
        },
        canActivate: [LoggedInGuard, MaintenanceGuard],
        canDeactivate: [PreventUnsavedChangesGuard],
    },
    {
        path: 'extension/:id',
        component: ExtensionRequestComponent,
        resolve: {
            form: OGEForm450Resolver
        },
        canActivate: [LoggedInGuard, MaintenanceGuard],
    },
    {
        path: 'healthcheck',
        component: HealthCheckComponent
    },
    {
        path: "**",
        component: PageNotFoundComponent,
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule],
    providers: [OGEForm450Resolver],
})

export class AppRoutingModule { }