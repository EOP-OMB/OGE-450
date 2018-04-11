// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.

export const environment = {
    production: false,
    apiUrl: 'Insert URL TO API',
    clientId: 'Insert your Application Group ID, Relying Party Trust ID from ADFS]',
    base: '/',
    debug: true,
    idleTimeout: 300,
    idleCountdown: 60,
    portalUrl: 'Insert URL of Application Portal or Remove Portal Link from Banner if stand alone',
};
