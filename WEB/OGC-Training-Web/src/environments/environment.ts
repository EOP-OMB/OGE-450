// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.

export const environment = {
    envName: 'dev',
    production: false,
    apiUrl: 'Url to Web API',
    oge450Url: 'Url to OGE 450 App',
    eventClearanceUrl: 'Url to Event Clearance App',
    clientId: 'ADFS Client Id',
    base: '/',
    debug: true,
    idleTimeout: 840,
    idleCountdown: 60,
    sharePointUrl: 'Url to SharePoint',
};
