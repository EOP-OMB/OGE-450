export const ErrorCodes = {
    Key: "ErrorCode",
    ServiceUnavailable: "0",
    BadRequest: "400",
    UnexpectedError: "500",
    AccessRequired: "401",
    NoRouteDefined: "404",
};

export class ErrorInfo {
    public type: string;
    public status: string;
    public message: string;
    public action: string;
}

export const ErrorInfoList: Array<ErrorInfo> = [
    {
        type: 'Service Unavailable',
        status: ErrorCodes.ServiceUnavailable,
        message: 'A service that this application relies on is not running.',
        action: `
            <p>It is probable that this outage is temporary, please try again in a few moments.</p>
            <p>Try clearing your browser cache and launcing the application again. If the problem persists and you need immediate assistance, please send us an email instead.</p>
        `
    },
    {
        type: 'Bad Request',
        status: ErrorCodes.BadRequest,
        message: 'Invalid Request',
        action: `
            <p>There is no action to take, click the button above to return to the application.</p><p>If you received this error by clicking on a link in the application, please contact your application administrator.</p>
        `,
    },
    {
        type: 'Unexpected Error',
        status: ErrorCodes.UnexpectedError,
        message: 'An unexpected error occurred.',
        action: `
            <p>Please contact your application administrator or click the button above to return to the applicaiton and try again.</p>
        `,
    },
    {
        type: 'Access Required',
        status: ErrorCodes.AccessRequired,
        message: 'Access required!  You attempted to access a resource that you do not have permissions to.',
        action: `<p>If you believe you should have access to this resource, please contact the application administrator.</p>`
    },
    {
        type: 'No Route Defined',
        status: ErrorCodes.NoRouteDefined,
        message: 'URL not found.  You attempted to go to a URL that does exist, please check your URL and try again.',
        action: `<p>There is no action to take, click the button above to return to the application.</p><p>If you received this error by clicking on a link in the application, please contact your application administrator.</p>`
    },
];
