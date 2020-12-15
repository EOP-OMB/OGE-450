export class AppUser {
    upn: string
    displayName: string;
    email: string;
    isAdmin: boolean;
    isReviewer: boolean;
    isRestrictedAdmin: boolean;
    roles: string[];
    userProfileUrl: string;

    phoneNumber: string;
    branch: string;

    manager: string;
}
