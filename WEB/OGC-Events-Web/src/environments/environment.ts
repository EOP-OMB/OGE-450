// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.

export const environment = {
    production: false,
    envName: 'dev',
    apiUrl: 'Url to Web Api',
    clientId: 'ADFS Client Id',
    base: '/',
    debug: true,
    idleTimeout: 840,
    idleCountdown: 60,
    sharePointUrl: 'SharePoint Site Collection Url',
    portalUrl: 'Url back to Portal (aka Training)',
};
