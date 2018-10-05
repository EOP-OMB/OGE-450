const { version } = require('./package.json');
const { resolve, relative } = require('path');
const { writeFileSync } = require('fs-extra');

var appBuildInfo  = {
  changeSet: "",
  buildNumber: "",
  buildJob: "",
  buildAgent: "",
  buildTime: "" 
}

appBuildInfo.buildTime = (new Date()).toLocaleString("en-US", {
  hour12: false,
  month: "2-digit",
  day: "2-digit",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
  second: "2-digit"
}).replace(',', '');

if (typeof (process.env.TFS_CHANGESET) != 'undefined') {
  appBuildInfo.buildAgent = "Jenkins";
  appBuildInfo.changeSet = process.env.TFS_CHANGESET || "";
  appBuildInfo.buildNumber = process.env.BUILD_NUMBER || "";
  appBuildInfo.buildJob = process.env.JOB_BASE_NAME || "";
}
else if (typeof (process.env.BUILD_SOURCEVERSION) != 'undefined') {
  appBuildInfo.buildAgent = "TFS";
  appBuildInfo.changeSet = process.env.BUILD_SOURCEVERSION || "";
  appBuildInfo.buildNumber = process.env.BUILD_BUILDNUMBER || "";
  appBuildInfo.buildJob = process.env.BUILD_DEFINITIONNAME || "";
}
else {
  appBuildInfo.buildAgent = "Visual Studio";
}

const file = resolve(__dirname, '.', 'src', 'environments', 'version.ts');
writeFileSync(file,
  `// IMPORTANT: THIS FILE IS AUTO GENERATED! DO NOT MANUALLY EDIT OR CHECKIN!
/* tslint:disable */
export const VERSION = ${JSON.stringify(appBuildInfo, null, 4)};
/* tslint:enable */
`, { encoding: 'utf-8' });

console.log(`Wrote version info to ${relative(resolve(__dirname, '.'), file)}`);
