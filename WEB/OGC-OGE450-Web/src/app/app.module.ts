// modules
import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'; // <-- NgModel lives here
import { Http, HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { Router } from '@angular/router';
import { AuthModule } from "./auth.module";
import { AppRoutingModule } from './app.routing';

// PrimeNG
import { DataTableModule, SharedModule, DropdownModule, CalendarModule, InputSwitchModule, EditorModule } from 'primeng/primeng';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// components
import { JWBootstrapSwitchModule } from 'jw-bootstrap-switch-ng2';
import { BannerComponent } from './banner/banner.component';
import { WelcomeComponent } from './welcome/welcome.component';

import { TinyMceEditorComponent } from './common/tinymce-editor.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
    import { MaintenanceComponent } from './home/maintenance/maintenance.component';
import { My450Component } from './my450/my450.component';
    import { WidgetComponent } from './common/widget.component';
    import { HelpfulLinksComponent } from './helpful-links/helpful-links.component';
    import { SupportContactsComponent } from './support-contacts/support-contacts.component';
    import { DashboardComponent } from './dashboard/dashboard.component';
import { FormComponent, PreventUnsavedChangesGuard } from './form/form.component';
    import { IntroComponent } from './form/intro/intro.component';
    import { ExamplesComponent } from './form/examples/examples.component';
    import { PageHeaderComponent } from './form/inserts/page-header.component';
import { FormsGridComponent } from './form/forms-grid/forms-grid.component';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';
import { ExtensionRequestComponent } from './extension-request/extension-request.component';
import { ExtensionGridComponent } from './extension-request/extension-grid/extension-grid.component';
import { AdminComponent } from './admin/admin.component';
    import { FormMiniComponent } from './admin/form-mini/form-mini.component';
    import { EditLinksComponent } from './admin/edit-links/edit-links.component';
    import { EditContactsComponent } from './admin/edit-contacts/edit-contacts.component';
    import { EditTemplateComponent } from './admin/edit-templates/edit-template.component';
import { EmployeeComponent } from './employees/employee.component';
import { EmployeesGridComponent } from './employees/employees-grid/employees-grid.component';

import { SettingsComponent } from './admin/settings/settings.component';

import { TabsComponent } from './tabs/tabs.component';
import { TabComponent } from './tabs/tab.component';

// models?
import { ExtensionRequest } from './extension-request/extension-request';

// services (other)
import { ExtensionRequestService } from './extension-request/extension-request.service';
import { OGEForm450Service } from './form/oge-form-450.service';

import { HealthCheckComponent } from './healthcheck/health-check.component';
import { HealthCheckService } from './healthcheck/health-check.service';

import { UserService } from './user/user.service';
import { HttpService } from './common/http.service';
import { HelpfulLinkService } from './helpful-links/helpful-links.service';
import { SupportContactService } from './support-contacts/support-contacts.service';
import { SettingsService } from './admin/settings/settings.service';
import { EmployeesService } from './employees/employees.service';
import { NotificationTemplateService } from './admin/edit-templates/notification-template.service';

// guards
import { LoggedInGuard } from "./security/logged-in.guard";
import { MaintenanceGuard } from "./security/maintenance.guard";
import { ReviewerGuard, AdminGuard } from "./security/reviewer.guard";

// error handling
import { GlobalErrorHandler } from './error/global-error-handler';
import { ErrorComponent } from './error/error.component';
import { PageNotFoundComponent } from './error/page-not-found.component';

// Idle handling
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive'; // this includes the core NgIdleModule but includes keepalive providers for easy wireup
import { MomentModule } from 'angular2-moment'; // optional, provides moment-style pipes for date formatting

export function httpServiceFactory(backend: XHRBackend, options: RequestOptions, router: Router) {
    return new HttpService(backend, options, router);
};

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        AppRoutingModule,
        AuthModule.forRoot(),
        MomentModule,
        NgIdleKeepaliveModule.forRoot(),
        // PrimeNG Modules
        DataTableModule,
        SharedModule,
        DropdownModule,
        CalendarModule,
        EditorModule,
        BrowserAnimationsModule,
        InputSwitchModule,
        JWBootstrapSwitchModule
    ],
    declarations: [
        AppComponent,
        BannerComponent,
        WelcomeComponent,
        HomeComponent,
        MaintenanceComponent,
        My450Component,
        ExtensionRequestComponent,
        DashboardComponent,
        AdminComponent,
        FormComponent,
        FormsGridComponent,
        IntroComponent,
        ExamplesComponent,
        LoginComponent,
        LogoutComponent,
        ErrorComponent,
        PageNotFoundComponent,
        HelpfulLinksComponent,
        WidgetComponent,
        TabsComponent,
        TabComponent,
        SupportContactsComponent,
        SettingsComponent,
        EmployeeComponent,
        EmployeesGridComponent,
        FormMiniComponent,
        ExtensionGridComponent,
        PageHeaderComponent,
        HealthCheckComponent,
        EditLinksComponent,
        EditContactsComponent,
        EditTemplateComponent,
        TinyMceEditorComponent
    ],
    providers: [
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler
        },
        {
            provide: Http,
            useFactory: httpServiceFactory,
            deps: [XHRBackend, RequestOptions, Router]
        },
        OGEForm450Service,
        LoggedInGuard,
        MaintenanceGuard,
        ReviewerGuard,
        AdminGuard,
        PreventUnsavedChangesGuard,
        HelpfulLinkService,
        ExtensionRequestService,
        SupportContactService,
        SettingsService,
        EmployeesService,
        HealthCheckService,
        NotificationTemplateService,
    ],
    bootstrap: [
        AppComponent 
    ]
})

export class AppModule {
}
