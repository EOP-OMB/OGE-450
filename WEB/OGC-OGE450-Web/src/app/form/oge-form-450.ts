import { ReportableInformation } from './reportable-information';
import { FormFlags } from '../common/constants';
import { Helper } from '../common/helper';

export class OGEForm450 {
    filer: string;
    id: number;
    year: number;
    dateReceivedByAgency: string;
    employeesName: string;
    emailAddress: string;
    positionTitle: string;
    grade: string;
    agency: string;
    branchUnitAndAddress: string;
    workPhone: string;
    reportingStatus: string;
    dateOfAppointment: string;
    isSpecialGovernmentEmployee: boolean;
    mailingAddress: string;

    hasAssetsOrIncome: boolean;
    hasLiabilities: boolean;
    hasOutsidePositions: boolean;
    hasAgreementsOrArrangements: boolean;
    hasGiftsOrTravelReimbursements: boolean;

    formStatus: string;
    employeeSignature: string;
    dateOfEmployeeSignature: string;

    reviewingOfficialSignature: string;
    dateOfReviewerSignature: string;
    commentsOfReviewingOfficial: string;

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

    dateOfSubstantiveReview: Date;
    substantiveReviewer: string;

    isSubmitting: boolean;

    closeAfterSaving: boolean;
    public get dueDateString(): string {
        return Helper.formatDate(Helper.getDate(this.dueDate));
    }

    public get dateSubmitted(): string {
        return Helper.formatDate(Helper.getDate(this.dateOfEmployeeSignature));
    }

    public get certifiedIn60Days(): string {
        return this.calcCertDays() <= 60 ? 'YES' : 'NO';
    }

    public get reviewedIn60Days(): string {
        return this.calcReviewDays() <= 60 ? 'YES' : 'NO';
    }

    public get daysToCertification() {
        var days = this.calcCertDays();

        if (days)
            return days.toString();
        else
            return '';
    }

    public calcCertDays(): number {
        var days: number;

        if (this.dateReceivedByAgency && this.dateOfReviewerSignature) {
            days = this.calcDaysBetween(new Date(this.dateReceivedByAgency), new Date(this.dateOfReviewerSignature));
        }

        return days;
    }

    public get daysToReview() {
        var days = this.calcReviewDays()

        if (days)
            return days.toString();
        else
            return '';
    }

    calcReviewDays(): number {
        var days: number;

        if (this.dateOfSubstantiveReview && this.dateOfReviewerSignature) {
            days = this.calcDaysBetween(new Date(this.dateOfSubstantiveReview), new Date(this.dateReceivedByAgency));
        }

        return days;
    }

    calcDaysBetween(date1: Date, date2: Date) {
        var diff = Math.abs(date1.getTime() - date2.getTime());
        var diffDays = Math.ceil(diff / (1000 * 3600 * 24));

        return diffDays;
    }
}
