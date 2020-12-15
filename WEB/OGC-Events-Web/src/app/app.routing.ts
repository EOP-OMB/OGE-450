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

import { EventRequestComponent, PreventUnsavedChangesGuard } from './event-request/event-request.component';
import { EventRequestResolver } from './event-request/event-request.resolver';

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
    //{
    //    path: 'event',
    //    component: EventRequestComponent,
    //    canActivate: [LoggedInGuard]
    //},
    {
        path: 'event/:id',
        component: EventRequestComponent,
        resolve: {
            event: EventRequestResolver
        },
        canActivate: [LoggedInGuard],
        canDeactivate: [PreventUnsavedChangesGuard],
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
        path: "**",
        component: PageNotFoundComponent,
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule],
    providers: [EventRequestResolver],
})

export class AppRoutingModule { }