import { ReportableInformation } from './reportable-information';

export class OGEForm450 {
    filer: string;
    id: number;
    year: number;
    dateReceivedByAgency: string;
    employeesName : string;
    emailAddress : string;
    positionTitle : string;
    grade : string;
    agency : string;
    branchUnitAndAddress : string;
    workPhone : string;
    reportingStatus : string;
    dateOfAppointment: string;
    isSpecialGovernmentEmployee : boolean;
    mailingAddress: string;

    hasAssetsOrIncome : string;
    hasLiabilities: string;
    hasOutsidePositions: string;
    hasAgreementsOrArrangements: string;
    hasGiftsOrTravelReimbursements: string;

    formStatus : string;
    employeeSignature : string;
    dateOfEmployeeSignature: string;

    reviewingOfficialSignature : string;
    dateOfReviewerSignature: string;
    commentsOfReviewingOfficial : string;

    reportableInformationList: ReportableInformation[];

    dueDate: string;

    isRejected: boolean;
    rejectionNotes: string;

    daysExtended: number;

    isOverdue: boolean;

    extendedText: string;
}