// modules
import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'; // <-- NgModel lives here
import { Http, HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthModule } from "./auth.module";
import { AppRoutingModule } from './app.routing';

// App modules
import { FileUploadModule } from 'ng2-file-upload';

// PrimeNG
import { DataTableModule, SharedModule, DropdownModule, CalendarModule, InputSwitchModule, EditorModule, AutoCompleteModule } from 'primeng/primeng';
//import { TableModule } from 'primeng/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// components
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { BannerComponent } from './banner/banner.component';
import { MaxBannerComponent } from './banner/max/max-banner.component';
import { WelcomeComponent } from './welcome/welcome.component';

// Common
import { TinyMceEditorComponent } from './common/tinymce-editor.component';
import { WidgetComponent } from './common/widget/widget.component';
import { TabsComponent } from './common/tabs/tabs.component';
import { TabComponent } from './common/tabs/tab.component';
import { HttpService } from './common/http.service';

// Security
import { LoginComponent } from './security/login/login.component';
import { LogoutComponent } from './security/logout/logout.component';
import { UserService } from './security/user.service';
import { LoggedInGuard } from "./security/logged-in.guard";
import { AdminGuard } from "./security/role.guard";

// Admin
import { AdminComponent } from './admin/admin.component';
import { EditTemplateComponent } from './admin/edit-templates/edit-template.component';
import { NotificationTemplateService } from './admin/edit-templates/notification-template.service';

// Health Check
import { HealthCheckComponent } from './healthcheck/health-check.component';
import { HealthCheckService } from './healthcheck/health-check.service';

// error handling
import { GlobalErrorHandler } from './error/global-error-handler';
import { ErrorComponent } from './error/error.component';
import { PageNotFoundComponent } from './error/page-not-found.component';

// Idle handling
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive'; // this includes the core NgIdleModule but includes keepalive providers for easy wireup
import { MomentModule } from 'angular2-moment'; // optional, provides moment-style pipes for date formatting

// App Components go here
import { EventRequestComponent, PreventUnsavedChangesGuard } from './event-request/event-request.component';
import { EventRequestDetailComponent } from './event-request/event-request-detail/event-request-detail.component';
import { UploadComponent } from './common/upload/upload.component';
import { EventGridComponent } from './event-request/event-grid/event-grid.component';

// App Services go here
import { DataService } from './home/data.service';
import { EventRequestService } from './event-request/event-request.service';
import { AttachmentService } from './event-request/event-request-detail/attachment.service';
import { AdminService } from './admin/admin.service';

export function httpServiceFactory(backend: XHRBackend, options: RequestOptions, router: Router) {
    return new HttpService(backend, options, router);
};

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        HttpClientModule,
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
        FileUploadModule,
        AutoCompleteModule,
        //TableModule,
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        BannerComponent,
        MaxBannerComponent,
        WelcomeComponent,
        AdminComponent,
        LoginComponent,
        LogoutComponent,
        ErrorComponent,
        PageNotFoundComponent,
        WidgetComponent,
        TabsComponent,
        TabComponent,
        HealthCheckComponent,
        EditTemplateComponent,
        TinyMceEditorComponent,
        // App components go here:
        EventRequestComponent,
        EventRequestDetailComponent,
        UploadComponent,
        EventGridComponent,
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
        HttpClient,
        LoggedInGuard,
        AdminGuard,
        HealthCheckService,
        NotificationTemplateService,
        // App servies go here
        DataService,
        EventRequestService,
        PreventUnsavedChangesGuard,
        AttachmentService,
        AdminService,
    ],
    bootstrap: [
        AppComponent
    ]
})

export class AppModule {
}
