import { ReportableInformation } from './reportable-information';
import { FormFlags } from '../common/constants';

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

    hasAssetsOrIncome: boolean;
    hasLiabilities: boolean;
    hasOutsidePositions: boolean;
    hasAgreementsOrArrangements: boolean;
    hasGiftsOrTravelReimbursements: boolean;

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
    isBlank: boolean;
    isUnchanged: boolean;

    extendedText: string;

    submittedPaperCopy: boolean;

    formFlags: string;
}