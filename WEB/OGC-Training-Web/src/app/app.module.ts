// modules
import { NgModule, ErrorHandler } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'; // <-- NgModel lives here
import { Http, HttpModule, RequestOptions, XHRBackend } from '@angular/http';
import { Router } from '@angular/router';
import { AuthModule } from "./auth.module";
import { AppRoutingModule } from './app.routing';


// PrimeNG
import { DataTableModule, SharedModule, DropdownModule, CalendarModule, InputSwitchModule, EditorModule, AutoCompleteModule, ChartModule } from 'primeng/primeng';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// components
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { BannerComponent } from './banner/banner.component';
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
import { SettingsComponent } from './admin/settings/settings.component';
import { SettingsService } from './admin/settings/settings.service';
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
import { TrainingGridComponent } from './training/training-grid/training-grid.component';
import { EditTrainingComponent } from './home/edit-training/edit-training.component';
import { NotificationComponent } from './home/notification/notification.component';
import { TrainingComponent } from './training/training.component';

// App Services go here
import { TrainingService } from './training/training.service';
import { TimelineService } from './timeline/timeline.service';
import { NotificationService } from './notifications/notifications.service';
import { ChartService } from './admin/charts/chart.service';
import { TrainingChartService } from './training/training-chart/training-chart.service';
import { OGEForm450Service } from './oge-form-450/oge-form-450.service';
import { EventRequestService } from './event-request/event-request.service';
import { VideoService } from './training/video.service';
import { EthicsFormService } from './ethics-form/ethics-form.service';
import { EthicsTeamService } from './ethics-team/ethics-team.service';

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
        AutoCompleteModule,
        ChartModule,
        BrowserAnimationsModule,
        InputSwitchModule,
    ],
    declarations: [
        AppComponent,
        HomeComponent,
        BannerComponent,
        WelcomeComponent,
        AdminComponent,
        LoginComponent,
        LogoutComponent,
        ErrorComponent,
        PageNotFoundComponent,
        WidgetComponent,
        TabsComponent,
        TabComponent,
        SettingsComponent,
        HealthCheckComponent,
        EditTemplateComponent,
        TinyMceEditorComponent,
        // App components go here:
        TrainingGridComponent,
        EditTrainingComponent,
        NotificationComponent,
        TrainingComponent,
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
        LoggedInGuard,
        AdminGuard,
        SettingsService,
        HealthCheckService,
        NotificationTemplateService,
        // App services go here:
        TrainingService,
        TimelineService,
        NotificationService,
        TrainingChartService,
        OGEForm450Service,
        ChartService,
        EventRequestService,
        VideoService,
        EthicsFormService,
        EthicsTeamService
    ],
    bootstrap: [
        AppComponent
    ]
})

export class AppModule {
}
